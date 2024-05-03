namespace LVK.TopoSort.Tests;

public class TopoOrderedEnumerableTests
{
    [Test]
    public void Constructor_NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TopoOrderedEnumerable<int>(null!, EqualityComparer<int>.Default));
    }

    [Test]
    public void Constructor_NullComparer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TopoOrderedEnumerable<int>(new List<Constraint<int>>(), null!));
    }

    [Test]
    public void GetEnumerableItems_ConstraintsInputInReverseOrder_ProducesCorrectOrder()
    {
        Constraint<int>[] constraints = [new Constraint<int>(1, 2), new Constraint<int>(2, 3), new Constraint<int>(3, 4),];

        var output = constraints.Ordered().ToList();

        Assert.That(output, Is.EqualTo(new[]
            {
                1, 2, 3, 4,
            })
           .AsCollection);
    }

    [Test]
    public void GetEnumerableItems_ConstraintsContainsCycle_ThrowsTopologicalCycleException()
    {
        Constraint<int>[] constraints = [new Constraint<int>(1, 2), new Constraint<int>(2, 1),];

        Assert.Throws<TopologicalCycleException>(() => _ = constraints.Ordered().ToList());
    }

    [Test]
    public void GetEnumerableItems_ConstraintsContainsCycle_ThrowsTopologicalCycleExceptionWithListOfElements()
    {
        Constraint<int>[] constraints = [new Constraint<int>(1, 2), new Constraint<int>(2, 1),];

        TopologicalCycleException exception = Assert.Throws<TopologicalCycleException>(() => _ = constraints.Ordered().ToList());
        Assert.That(exception.Elements, Is.EquivalentTo(new[]
        {
            1, 2,
        }));
    }

    [Test]
    public void GetEnumerableItems_ConstraintsContainsCycle_DoesNotThrowIfNotEnumerated()
    {
        Constraint<int>[] constraints = [new Constraint<int>(1, 2), new Constraint<int>(2, 1),];

        constraints.Ordered();
        Assert.Pass();
    }

    [Test]
    public void GetEnumerableItems_SomeElementsDoesNotHaveConstraints_ReturnsAllSuchsElementsFirst()
    {
        Constraint<int>[] constraints = [
            Constraint.Create(2, 3),
            Constraint.Create(3, 4),
            Constraint.CreateUnconstrained(1),
            Constraint.CreateUnconstrained(5),
        ];

        var elements = constraints.Ordered().ToList();

        Assert.That(elements.Take(3), Is.EquivalentTo(new[]
        {
            1, 2, 5,
        }));

        Assert.That(elements.Skip(3), Is.EqualTo(new[]
        {
            3, 4,
        }));
    }
}