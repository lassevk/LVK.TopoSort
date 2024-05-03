using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LVK.TopoSort;

public class TopoOrderedEnumerable<T> : IEnumerable<T>
    where T : notnull
{
    private readonly IList<Constraint<T>> _source;
    private readonly IEqualityComparer<T> _comparer;

    public TopoOrderedEnumerable(IList<Constraint<T>> source, IEqualityComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(comparer);

        _source = source;
        _comparer = comparer;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var incomingDependencies = new Dictionary<T, HashSet<T>>(_comparer);
        var outgoingDependencies = new Dictionary<T, HashSet<T>>(_comparer);
        var elementsWithNoIncomingDependencies = new HashSet<T>(_comparer);

        InitializeDataStructures(incomingDependencies, outgoingDependencies, elementsWithNoIncomingDependencies);
        while (elementsWithNoIncomingDependencies.Any())
        {
            foreach (T element in elementsWithNoIncomingDependencies)
                yield return element;

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
            if (_comparer.Equals(constraint.FirstElement, constraint.SecondElement))
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
                if (!incomingDependencies.TryGetValue(followingElement, out HashSet<T>? incomingForFollowingElement))
                    continue;

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