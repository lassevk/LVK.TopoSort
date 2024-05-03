using System.Collections;

namespace LVK.TopoSort;

public class TopoOrderedEnumerable<T> : IEnumerable<T>
{
    public TopoOrderedEnumerable(List<Constraint<T>> toList, IEqualityComparer<T> comparer)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}