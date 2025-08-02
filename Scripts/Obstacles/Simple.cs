using Godot;
namespace Outrun.Obstacles;

[GlobalClass]
public partial class Simple : Obstacle
{
	protected override string outrunClass { get; set; } = "Simple";
	private RigidBody3D _body;
	public override Node3D body { get => _body; set => _body = (RigidBody3D)value; }
}
