namespace LVK.TopoSort.Tests;

public class TopoTests
{
    [TestCase(1, 2)]
    [TestCase(2, 3)]
    [TestCase(2, 1)]
    public void Create_PairOfElementsWithTestCases_CreatesCorrectConstraints(int first, int second)
    {
        Constraint<int> constraint = Topo.Constrained(first, second);

        Assert.That(constraint.FirstElement, Is.EqualTo(first));
        Assert.That(constraint.SecondElement, Is.EqualTo(second));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Create_SingleElementWithTestCases_CreatesCorrectConstraints(int element)
    {
        Constraint<int> constraint = Topo.Element(element);

        Assert.That(constraint.FirstElement, Is.EqualTo(element));
        Assert.That(constraint.SecondElement, Is.EqualTo(element));
    }
}