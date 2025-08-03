using Godot;
using System;

namespace Outrun.Obstacles;

[GlobalClass]
public partial class Pickupable : Area
{
	protected override string outrunClass { get; set; } = "Area";
	[Export]
	public Ability ability;

	public override void _Ready()
	{
		_body.BodyEntered += Pickup;
	}

	public void Pickup(Node3D body)
	{
		if (body.GetOutrunClass() == "Player")
		{
			Player player = (Player)body;
			player.AddAbility(ability);
			CallDeferred("queue_free");
		}
	}
}
