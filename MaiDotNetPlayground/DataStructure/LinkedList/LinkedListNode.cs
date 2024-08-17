namespace MaiDotNetPlayground.DataStructure.LinkedList;

// See NOTES in LinkedList.cs
public class LinkedListNode
{
    public int Value { get; private set; }
    public LinkedListNode? Next { get; internal set; }

    private LinkedListNode(){}

    public LinkedListNode(int value)
    {
        Value = value;
        Next = null;
    }
}
