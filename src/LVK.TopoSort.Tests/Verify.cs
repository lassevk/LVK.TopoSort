namespace LVK.TopoSort.Tests;

public static class Verify
{
    public static void ConstraintsSatisfied<T>(IEnumerable<Constraint<T>> constraints, IEnumerable<T> elements, IEqualityComparer<T>? equalityComparer = null)
        where T : notnull
    {
        equalityComparer ??= EqualityComparer<T>.Default;

        var list = elements.ToList();
        foreach (Constraint<T> constraint in constraints)
        {
            int i1 = list.FindIndex(element => equalityComparer.Equals(constraint.FirstElement, element));
            int i2 = list.FindIndex(element => equalityComparer.Equals(constraint.SecondElement, element));

            Assert.That(i1, Is.GreaterThanOrEqualTo(0));
            Assert.That(i2, Is.GreaterThanOrEqualTo(0));

            if (equalityComparer.Equals(constraint.FirstElement, constraint.SecondElement))
                continue;

            Assert.That(i2, Is.GreaterThan(i1));
        }
    }
}