using System.Diagnostics.CodeAnalysis;

namespace LVK.TopoSort;

[ExcludeFromCodeCoverage]
public record struct Constraint<T>(T FirstElement, T SecondElement)
{
    public static implicit operator Constraint<T>(T element) => new(element, element);
}