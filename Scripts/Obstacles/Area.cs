using Godot;
namespace Outrun.Obstacles;

[GlobalClass]
public partial class Area : Obstacle
{
	protected override string outrunClass { get; set; } = "Area";
    protected Area3D _body;
    public override Node3D body { get => _body; set => _body = (Area3D)value; }
}