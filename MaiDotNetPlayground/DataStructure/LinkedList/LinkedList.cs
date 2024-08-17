using System.Text;

namespace MaiDotNetPlayground.DataStructure.LinkedList;

// Since there is already System.Collection.Generic.LinkedList<T>, 
// and to avoid confusion and collision, and keep things simple,
// I am not making this class generic. I will make the Value/Data "int" type only.
public class LinkedList
{
    public LinkedListNode? Head { get; private set; } = null;
    public int Count { get; private set; } = 0;

    private static readonly string _separator = " -> ";

    // use Append and Prepend (instead of Add and Insert) for more clarity.
    // .NET's LinkedList implements AddAfter, AddBefore, AddFirst, AddLast, Append, Contains, etc methods.
    public void Append(LinkedListNode node) // AddLast
    {
        if(null != node)
        {
            if( null == Head)
                Head = node;
            else
            {
                LinkedListNode? next = Head;

                while (null != next.Next)
                {
                    next = next.Next;
                }
                next.Next = node;
            }
            Count++;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if(Head != null)
            sb.Append(Head.Value);

        LinkedListNode? node = Head?.Next;
        
        while (null != node)
        {
            sb.Append(_separator).Append(node.Value);
            node = node.Next;
        }

        return sb.ToString();
    }
}
