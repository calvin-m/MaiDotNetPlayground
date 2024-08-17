using MaiDotNetPlayground.DataStructure.LinkedList;
using Xunit.Abstractions;

namespace MaiDotNetPlayground.Tests.DataStructure;

public class LinkedListTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    public LinkedListTests(ITestOutputHelper output)
    {
        // Setup
        _output = output;
        
        _output.WriteLine("Global setup done.");
    }
    [Fact]
    public void Append3Nodes()
    {
        // Arrange
        var node1 = new LinkedListNode(1);
        var node2 = new LinkedListNode(2);
        var node3 = new LinkedListNode(3);

        LinkedList sut = new();

        // Act
        sut.Append(node1);
        sut.Append(node2);
        sut.Append(node3);

        // Assert
        _output.WriteLine(sut.ToString());
        Assert.Equal(3, sut.Count);
    }

    public void Dispose()
    {
        _output.WriteLine("Dispose/Teardown done.");
    }
}
