using Godot;
using Outrun;
using System;

public partial class World : Node3D
{
	public static Control controlLayer;
	public static Player mainPlayer;
	public static int physicsFrame = 0;

	public override void _EnterTree()
	{
		controlLayer = GetNode<Control>("Control");
	}
	public override void _PhysicsProcess(double delta)
	{
		physicsFrame++;
    }

}
