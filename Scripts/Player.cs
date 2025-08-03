using System;
using Godot;
using Godot.Collections;

namespace Outrun;

[GlobalClass]
public partial class Player : CharacterBody3D
{
	[Export]
	Camera3D camera { get; set;}
	[Export]
	Vector3 cameraOffset { get; set; } = new(0, 4, 0);
	[Export]
	float cameraZoom { get; set; } = 10;
	[Export(PropertyHint.Range, "0.01,1,")]
	float cameraSmoothing = 0.2f;
	[Export]
	float gravityStrength { get; set; } = 9.8f;
	[Export]
	float speed { get; set; } = 1f;
	[Export]
	float turnRadius { get; set; } = 100f;
	[Export]
	float friction { get; set; } = 0.98f;
	[Export]
	float rollResistance { get; set; } = 2;
	[Export]
	float mass = 1000;
	private PlayerController controller;
	public override void _Ready()
	{
		controller = new()
		{
			player = this,
			camera = camera,
			cameraOffset = cameraOffset,
			cameraZoom = cameraZoom,
			gravityStrength = gravityStrength,
			baseSpeed = speed,
			baseTurnRadius = turnRadius,
			friction = friction,
			rollResistance = rollResistance,
			mass = mass,
			cameraSmoothing = cameraSmoothing
		};
		controller._Ready();
	}
	public override void _Process(double delta)
	{
		controller._Process(delta);
	}
	public override void _PhysicsProcess(double delta)
	{
		controller._PhysicsProcess(delta);
	}
	public override void _Input(InputEvent @event)
	{
		controller._Input(@event);
	}

}
