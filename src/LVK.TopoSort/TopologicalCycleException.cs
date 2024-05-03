namespace LVK.TopoSort;

public class TopologicalCycleException : InvalidOperationException
{
    public TopologicalCycleException(string message, IEnumerable<object> elements)
        : base(message)
    {
        Elements = elements.ToList();
    }

    public List<object> Elements { get; }
}