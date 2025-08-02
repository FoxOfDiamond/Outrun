using Godot;
namespace Outrun.Obstacles;

public partial class Simple : Obstacle
{
    public RigidBody3D _body;
    public override Node3D body { get => _body; set => _body = (RigidBody3D)value; }
}