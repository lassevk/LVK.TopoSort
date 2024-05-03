using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LVK.TopoSort;

public class TopoOrderedGroupsEnumerable<T> : IEnumerable<T[]>
    where T : notnull
{
    private readonly IList<Constraint<T>> _source;
    private readonly IEqualityComparer<T> _equalityComparer;
    private readonly IComparer<T> _comparer;

    public TopoOrderedGroupsEnumerable(IList<Constraint<T>> source, IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(equalityComparer);
        ArgumentNullException.ThrowIfNull(comparer);

        _source = source;
        _equalityComparer = equalityComparer;
        _comparer = comparer;
    }

    public IEnumerator<T[]> GetEnumerator()
    {
        var incomingDependencies = new Dictionary<T, HashSet<T>>(_equalityComparer);
        var outgoingDependencies = new Dictionary<T, HashSet<T>>(_equalityComparer);
        var elementsWithNoIncomingDependencies = new HashSet<T>(_equalityComparer);

        InitializeDataStructures(incomingDependencies, outgoingDependencies, elementsWithNoIncomingDependencies);
        while (elementsWithNoIncomingDependencies.Any())
        {
            T[] group = elementsWithNoIncomingDependencies.ToArray();
            Array.Sort(group, _comparer);
            yield return group;

            RemoveBatchFromConstraints(incomingDependencies, outgoingDependencies, elementsWithNoIncomingDependencies);
        }

        if (outgoingDependencies.Any())
            throw new TopologicalCycleException("Topological sort contains cycles", outgoingDependencies.Keys.OfType<object>());
    }

    private void InitializeDataStructures(Dictionary<T, HashSet<T>> incomingDependencies, Dictionary<T, HashSet<T>> outgoingDependencies, HashSet<T> elementsWithNoIncomingDependencies)
    {
        void addToDictionary(Dictionary<T, HashSet<T>> dictionary, T key, T value)
        {
            if (!dictionary.TryGetValue(key, out HashSet<T>? set))
            {
                set = new();
                dictionary[key] = set;
            }

            set.Add(value);
        }

        foreach (Constraint<T> constraint in _source)
        {
            elementsWithNoIncomingDependencies.Add(constraint.FirstElement);
            elementsWithNoIncomingDependencies.Add(constraint.SecondElement);
        }

        foreach (Constraint<T> constraint in _source)
        {
            if (_equalityComparer.Equals(constraint.FirstElement, constraint.SecondElement))
                continue;

            addToDictionary(outgoingDependencies, constraint.FirstElement, constraint.SecondElement);
            addToDictionary(incomingDependencies, constraint.SecondElement, constraint.FirstElement);
        }

        elementsWithNoIncomingDependencies.RemoveWhere(incomingDependencies.ContainsKey);
    }

    private void RemoveBatchFromConstraints(Dictionary<T, HashSet<T>> incomingDependencies, Dictionary<T, HashSet<T>> outgoingDependencies, HashSet<T> elementsWithNoIncomingDependencies)
    {
        var batch = elementsWithNoIncomingDependencies.ToList();
        elementsWithNoIncomingDependencies.Clear();

        foreach (T element in batch)
        {
            if (!outgoingDependencies.Remove(element, out HashSet<T>? outgoingForElement))
                continue;

            foreach (T followingElement in outgoingForElement)
            {
                HashSet<T> incomingForFollowingElement = incomingDependencies[followingElement];
                incomingForFollowingElement.Remove(element);
                if (incomingForFollowingElement.Any())
                    continue;

                incomingDependencies.Remove(followingElement);
                elementsWithNoIncomingDependencies.Add(followingElement);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}