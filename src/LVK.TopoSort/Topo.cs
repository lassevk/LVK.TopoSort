namespace LVK.TopoSort;

public static class Topo
{
    public static IEnumerable<T> Ordered<T>(this IEnumerable<Constraint<T>> constraints, IEqualityComparer<T>? comparer = null) => new TopoOrderedEnumerable<T>(constraints.ToList(), comparer ?? EqualityComparer<T>.Default);
}