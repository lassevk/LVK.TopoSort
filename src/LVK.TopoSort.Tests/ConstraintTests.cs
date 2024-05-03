namespace LVK.TopoSort.Tests;

public class ConstraintTests
{
    [TestCase(1, 2)]
    [TestCase(2, 3)]
    [TestCase(2, 1)]
    public void Create_PairOfElementsWithTestCases_CreatesCorrectConstraints(int first, int second)
    {
        var constraint = Constraint.Create(first, second);

        Assert.That(constraint.FirstElement, Is.EqualTo(first));
        Assert.That(constraint.SecondElement, Is.EqualTo(second));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Create_SingleElementWithTestCases_CreatesCorrectConstraints(int element)
    {
        var constraint = Constraint.CreateUnconstrained(element);

        Assert.That(constraint.FirstElement, Is.EqualTo(element));
        Assert.That(constraint.SecondElement, Is.EqualTo(element));
    }
}