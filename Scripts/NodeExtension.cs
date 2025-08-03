namespace Godot;

public static class NodeExtension
{
    public static string GetOutrunClass(this Node3D node)
    {
        return (string)node.GetMeta("OutrunClass", node.GetClass());
    }
    public static string GetOutrunFriction(this Node3D node)
    { 
        return (string)node.GetMeta("OutrunFriction",0);
    }
}