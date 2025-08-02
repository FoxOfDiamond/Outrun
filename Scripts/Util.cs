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

    public static T FindFirstChild<T>(Node parent, string name) where T : Node
    {
        Array<Node> children = parent.GetChildren();
        foreach (Node child in children)
        {
            if (child.Name == name)
            {
                return (T)child;
            }
        }
        return null;
    }

    public static Vector3 LerpAngleDeg(this Vector3 thisVec, Vector3 to, float weight)
    {
        var newVec = thisVec;
        newVec.X = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(newVec.X), Mathf.DegToRad(to.X), weight));
        newVec.Y = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(newVec.Y), Mathf.DegToRad(to.Y), weight));
        newVec.Z = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(newVec.Z), Mathf.DegToRad(to.Z), weight));
        return newVec;
    }
}