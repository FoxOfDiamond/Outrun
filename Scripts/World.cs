using Godot;
using System;

public partial class World : Node3D
{
	public static Control controlLayer;

	public override void _EnterTree()
	{
		controlLayer = GetNode<Control>("Control");
	}
}
