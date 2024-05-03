namespace LVK.TopoSort.Tests;

public class TopoOrderedGroupsEnumerableTests
{
    [Test]
    public void Constructor_NullSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TopoOrderedGroupsEnumerable<int>(null!, EqualityComparer<int>.Default, Comparer<int>.Default));
    }

    [Test]
    public void Constructor_NullEqualityComparer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TopoOrderedGroupsEnumerable<int>(new List<Constraint<int>>(), null!, Comparer<int>.Default));
    }

    [Test]
    public void Constructor_NullComparer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new TopoOrderedGroupsEnumerable<int>(new List<Constraint<int>>(), EqualityComparer<int>.Default, null!));
    }

    [Test]
    public void GetEnumerableItems_ConstraintsInputInReverseOrder_ProducesCorrectOrder()
    {
        Constraint<int>[] constraints = [new Constraint<int>(1, 2), new Constraint<int>(2, 3), new Constraint<int>(3, 4),];

        var output = constraints.OrderedGroups().ToList();

        Assert.That(output, Is.EqualTo(new int[][]
            {
                [1], [2], [3], [4],
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
        Constraint<int>[] constraints = [Constraint.Create(2, 3), Constraint.Create(3, 4), Constraint.CreateUnconstrained(1), Constraint.CreateUnconstrained(5),];

        var groups = constraints.OrderedGroups().ToList();

        Assert.That(groups, Is.EqualTo(new int[][]
        {
            [1, 2, 5], [3], [4],
        }).AsCollection);
    }

    [Test]
    public void ReadMeExample()
    {
        Constraint<string>[] constraints =
        [
            Constraint.Create("A", "B"), Constraint.Create("A", "C"), Constraint.Create("B", "D"), Constraint.Create("C", "D"), Constraint.Create("E", "F"), Constraint.CreateUnconstrained("G"),
            Constraint.CreateUnconstrained("H"),
        ];

        var groups = constraints.OrderedGroups().ToList();

        Assert.That(groups, Is.EqualTo(new string[][]
            {
                ["A", "E", "G", "H"], ["B", "C", "F"], ["D"],
            })
           .AsCollection);
    }
}