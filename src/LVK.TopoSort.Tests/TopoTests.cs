namespace LVK.TopoSort.Tests;

public class TopoTests
{
    [TestCase(1, 2)]
    [TestCase(2, 3)]
    [TestCase(2, 1)]
    public void Constraint_WithTestCases_CreatesCorrectConstraints(int first, int second)
    {
        Constraint<int> constraint = Topo.Constrained(first, second);

        Assert.That(constraint.FirstElement, Is.EqualTo(first));
        Assert.That(constraint.SecondElement, Is.EqualTo(second));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Element_WithTestCases_CreatesCorrectConstraints(int element)
    {
        Constraint<int> constraint = Topo.Element(element);

        Assert.That(constraint.FirstElement, Is.EqualTo(element));
        Assert.That(constraint.SecondElement, Is.EqualTo(element));
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    [TestCase(2, 1)]
    public void FollowedBy_WithTestCases_CreatesCorrectConstraints(int first, int second)
    {
        Constraint<int> constraint = first.FollowedBy(second);

        Assert.That(constraint.FirstElement, Is.EqualTo(first));
        Assert.That(constraint.SecondElement, Is.EqualTo(second));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void ConversionOperator_WithTestCases_CreatesCorrectConstraints(int element)
    {
        Constraint<int> constraint = element;

        Assert.That(constraint.FirstElement, Is.EqualTo(element));
        Assert.That(constraint.SecondElement, Is.EqualTo(element));
    }
}