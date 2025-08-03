using Godot;
using System;

namespace Outrun;

public partial class Pickupable : Sprite3D
{
	public Area3D myDetector;
	[Export]
	public Ability toAdd;

	public override void _Ready()
	{
		myDetector = GetNode<Area3D>("Detector");
		myDetector.AreaEntered += Pickup;
	}

	public void Pickup(Area3D area)
	{
		Player source = GetParent<Player>();
		source.AddAbility(toAdd);
		GetParent().RemoveChild(this);
		QueueFree();
	}
}
