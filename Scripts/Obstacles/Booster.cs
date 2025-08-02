using Godot;
using System;

namespace Outrun;
[GlobalClass]
public partial class Booster : Obstacle
{
	protected override string outrunClass { get; set; } = "Booster";
	[Export]
	private float strength = 5f;
	private RigidBody3D _body;
	public override Node3D body { get => _body; set => _body = (RigidBody3D)value; }

	private MeshInstance3D mesh, mesh2;

	public bool helping = true;

	public Area3D detector { get; set; }

	public override void _Ready()
	{
		detector = body.GetNode<Area3D>("Detector");
		mesh = body.GetNode<MeshInstance3D>("Mesh");
		mesh2 = body.GetNode<MeshInstance3D>("Mesh2");

		detector.AreaEntered += OnAreaEntered;
		detector.AreaExited += OnAreaExited;
	}

	public void OnAreaEntered(Area3D area)
	{
		var target = area.GetParent<Player>();
		if (helping)
		{
<<<<<<< Updated upstream
			target.Velocity *= new Vector3(5, 0, 5);
		}
		else
		{
			target.Velocity *= new Vector3(-5, 0, -5);
=======
			target.Velocity *= strength;
		}
		else
		{
			target.Velocity *= 1/strength;
>>>>>>> Stashed changes
		}
		
	}

	public void OnAreaExited(Area3D area)
	{
		if (helping)
		{
			mesh.Visible = false;
			mesh2.Visible = true;
			helping = false;
		}
	}
}
