using Godot;

namespace Outrun;

[GlobalClass]
public partial class Obstacle : Node3D, IOutrunClass
{
    protected virtual string outrunClass { get; set; } = "Obstacle";
    public virtual Node3D body { get; set; }
    public virtual Node3D init()
    {
        return (Node3D)Util.FindFirstChild(this, "Body");
    }
    public override void _EnterTree()
    {
        body = init();
        setMeta();
    }
    public void setMeta()
    {
        SetMeta("OutrunClass", outrunClass);
    }
}