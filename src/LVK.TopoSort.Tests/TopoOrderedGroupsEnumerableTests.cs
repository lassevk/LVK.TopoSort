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

        var groups = constraints.OrderedGroups().ToList();

        Assert.That(groups, Is.EqualTo(new int[][]
            {
                [1], [2], [3], [4],
            })
           .AsCollection);

        Verify.ConstraintsSatisfied(constraints, groups.SelectMany(e => e));
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
        Constraint<int>[] constraints = [Topo.Constrained(2, 3), Topo.Constrained(3, 4), Topo.Element(1), Topo.Element(5),];

        var groups = constraints.OrderedGroups().ToList();

        Assert.That(groups, Is.EqualTo(new int[][]
        {
            [1, 2, 5], [3], [4],
        }).AsCollection);

        Verify.ConstraintsSatisfied(constraints, groups.SelectMany(e => e));
    }

    [Test]
    public void ReadMeExample()
    {
        Constraint<string>[] constraints =
        [
            Topo.Constrained("A", "B"), Topo.Constrained("A", "C"), Topo.Constrained("B", "D"), Topo.Constrained("C", "D"), Topo.Constrained("E", "F"), Topo.Element("G"),
            Topo.Element("H"),
        ];

        var groups = constraints.OrderedGroups().ToList();

        Assert.That(groups, Is.EqualTo(new string[][]
            {
                ["A", "E", "G", "H"], ["B", "C", "F"], ["D"],
            })
           .AsCollection);

        Verify.ConstraintsSatisfied(constraints, groups.SelectMany(e => e));
    }

    [Test]
    public void ReadMeAdditionalExample()
    {
        Constraint<int>[] constraints = [
            1, 4, 10, 5,
            1.FollowedBy(10),
            10.FollowedBy(5),
            4.FollowedBy(1),
        ];

        var output = constraints.Ordered().ToList();

        Assert.That(output, Is.EqualTo(new int[]
            {
                4, 1, 10, 5,
            })
           .AsCollection);

        Verify.ConstraintsSatisfied(constraints, output);
    }

    [Test]
    public void GetEnumeratedElements_InterleavedConstraints_ProducesCorrectResult()
    {
        Constraint<int>[] constraints = [
            1.FollowedBy(3),    //    1 -> 3 -> 5 -> 7
            2.FollowedBy(4),    //   / \  / \  / \  /
            3.FollowedBy(5),    //  2 -> 4 -> 6 -> 8
            4.FollowedBy(6),
            5.FollowedBy(7),    // 2, 1, 4, 3, 6, 5, 8, 7
            6.FollowedBy(8),

            2.FollowedBy(1),
            4.FollowedBy(3),
            6.FollowedBy(5),
            8.FollowedBy(7),

            1.FollowedBy(4),
            3.FollowedBy(6),
            5.FollowedBy(8),
        ];

        var output = constraints.Ordered().ToList();

        Assert.That(output, Is.EqualTo(new[]
            {
                2, 1, 4, 3, 6, 5, 8, 7,
            })
           .AsCollection);

        Verify.ConstraintsSatisfied(constraints, output);
    }
}