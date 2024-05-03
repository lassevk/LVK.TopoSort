using System.Diagnostics.CodeAnalysis;

namespace LVK.TopoSort;

[ExcludeFromCodeCoverage]
public record struct Constraint<T>(T FirstElement, T SecondElement);

public static class Constraint
{
    public static Constraint<T> Create<T>(T firstElement, T secondElement)
        where T : notnull
        => new(firstElement, secondElement);

    public static Constraint<T> CreateUnconstrained<T>(T element)
        where T : notnull
        => new(element, element);
}