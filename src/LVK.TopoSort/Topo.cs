namespace LVK.TopoSort;

public static class Topo
{
    public static IEnumerable<T[]> OrderedGroups<T>(this IEnumerable<Constraint<T>> constraints, IEqualityComparer<T>? equalityComparer = null, IComparer<T>? comparer = null)
        where T : notnull
        => new TopoOrderedGroupsEnumerable<T>(constraints.ToList(), equalityComparer ?? EqualityComparer<T>.Default, comparer ?? Comparer<T>.Default);

    public static IEnumerable<T> Ordered<T>(this IEnumerable<Constraint<T>> constraints, IEqualityComparer<T>? equalityComparer = null, IComparer<T>? comparer = null)
        where T : notnull
        => OrderedGroups(constraints, equalityComparer, comparer).SelectMany(element => element);

    public static Constraint<T> Element<T>(T element)
        where T : notnull
        => new(element, element);

    public static Constraint<T> Constrained<T>(T firstElement, T secondElement)
        where T : notnull
        => new(firstElement, secondElement);

    public static Constraint<T> FollowedBy<T>(this T firstElement, T secondElement)
        where T : notnull
        => new(firstElement, secondElement);
}