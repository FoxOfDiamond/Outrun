using System;
using Godot;
using Godot.Collections;

namespace Outrun;

[GlobalClass]
public partial class Player : CharacterBody3D
{
	[Export]
	Camera3D camera { get; set; }
	[Export]
	Ghost ghost { get; set;}
	[Export]
	CpuParticles3D driftParticle { get; set; }
	[Export]
	CpuParticles3D boostParticle { get; set; }
	[Export]
	int ghostDelay = 300;
	[Export]
	float driftChargeLength { get; set; } = 300;
	[Export]
	float driftBoost { get; set; } = 0.7f;
	[Export]
	float driftBoostDecay { get; set; } = 0.01f;
	[Export]
	Color stage0DriftColor { get; set; } = new(1,1,0);
	Color stage1DriftColor { get; set; } = new(0,0.8f,1);
	[Export]
	Color stage2DriftColor { get; set; } = new(1f,0.45f,0);
	[Export]
	Color stage3DriftColor { get; set; } = new(0.9f,0,0.9f);
	[Export]
	Vector3 cameraOffset { get; set; } = new(0, 4, 0);
	[Export]
	float cameraZoom { get; set; } = 10;
	[Export(PropertyHint.Range, "0.01,1,")]
	float cameraSmoothing = 0.2f;
	[Export]
	float gravityStrength { get; set; } = 0.98f;
	[Export]
	float speed { get; set; } = 1f;
	[Export]
	float turnRadius { get; set; } = 10;
	[Export]
	public float friction { get; set; } = 0.2f;
	[Export]
	public float traction { get; set; } = 0.9f;
	Array<Ability> abilities = [];
	float airResistance { get; set; } = 0.01f;
	[Export]
	float mass = 1000; //Only affects pushables
	private PlayerController controller;
	protected string outrunClass { get; set; } = "Player";
	public override void _EnterTree()
	{
		setMeta();
	}
	public override void _Ready()
	{
		controller = new()
		{
			player = this,
			ghost = ghost,
			boostParticle = boostParticle,
			driftParticle = driftParticle,
			ghostDelay = ghostDelay,
			driftChargeLength = driftChargeLength,
			driftBoostPower = driftBoost,
			driftBoostDecay = driftBoostDecay,
			stage0DriftColor = stage0DriftColor,
			stage1DriftColor = stage1DriftColor,
			stage2DriftColor = stage2DriftColor,
			stage3DriftColor = stage3DriftColor,
			camera = camera,
			cameraOffset = cameraOffset,
			cameraZoom = cameraZoom,
			gravityStrength = gravityStrength,
			baseSpeed = speed,
			baseTraction = traction,
			baseTurnRadius = turnRadius,
			baseFriction = friction,
			baseAirResistance = airResistance,
			mass = mass,
			cameraSmoothing = cameraSmoothing,
			abilities = abilities
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
	public void AddAbility(Ability toAdd)
	{
		controller.AddAbility(toAdd);
	}
	public void setMeta()
	{
		SetMeta("OutrunClass", outrunClass);
	}

}
