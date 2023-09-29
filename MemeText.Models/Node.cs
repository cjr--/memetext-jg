namespace MemeText.Models;

public class Node
{
    public Node? Parent { get; private set; }
    public Node? FirstChild { get; private set; }
    public Node? LastChild { get; private set; }

    public Node? PreviousSibling { get; private set; }
    public Node? NextSibling { get; private set; }

    public IEnumerable<Node> Children
    {
        get
        {
            for (var node = FirstChild; node != null; node = node.NextSibling)            
                yield return node;            
        }
    }

    public IEnumerable<Node> Siblings
    { 
        get
        {            
            for (var node = NextSibling; node != null; node = node.NextSibling)
                yield return node;
        }
    }

    public Node AddChild(Node node)
    {
        if (FirstChild == null)
        {
            FirstChild = node;
            LastChild = node;
        }
        else
        {
            LastChild!.NextSibling = node;
            node.PreviousSibling = LastChild;
            LastChild = node;
            node.Parent = this;
        }

        return node;
    }

    public Node AddSibling(Node node)
    {
        if (NextSibling == null) 
        { 
            NextSibling = node;
        }
        else
        {
            NextSibling!.NextSibling = node;
            node.PreviousSibling = NextSibling;
            NextSibling = node;
        }

        return node;            
    }
}
