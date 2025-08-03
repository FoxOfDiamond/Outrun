using Godot;
using Outrun;
using System;

public partial class World : Node3D
{
	public static Control ui;
	public static ItemList abilityUi;
	public static Player mainPlayer;
	public static int physicsFrame = 0;

	public override void _EnterTree()
	{
		ui = GetNode<Control>("UI");
		abilityUi = ui.GetNode<ItemList>("AbilityContainer");
	}
	public override void _PhysicsProcess(double delta)
	{
		physicsFrame++;
	}

}
