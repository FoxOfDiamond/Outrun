using Godot;
using Godot.Collections;

namespace Outrun;

public static class Util
{
    public static Node FindFirstChild(Node parent, string name)
    {
        Array<Node> children = parent.GetChildren();
        foreach (Node child in children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }
        return null;
    }
}