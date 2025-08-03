using System;
using System.Collections;
using Godot;
using Godot.Collections;

namespace Outrun;

public class PlayerController
{
	public Player player;
	public Ghost ghost;
	public Camera3D camera;
	public Vector3 cameraOffset = new(0, 4, 0);
	public Vector3 baseCameraRotationalOffset = new(-20, 0, 0);
	public Vector3 cameraRotationalOffset;
	public CpuParticles3D driftParticle;
	public CpuParticles3D boostParticle;
	public Color stage0DriftColor;
	public Color stage1DriftColor;
	public Color stage2DriftColor;
	public Color stage3DriftColor;
	public ArrayList snapshots = new();
	public int ghostDelay;
	private bool mouseLocked = true;
	private bool freelook = false;
	public float cameraZoom = 10;
	private bool drifting = false;
	private bool airborn = false;
	private float driftDirection = 0;
	private float driftStrength = 1;
	private float driftBoost = 0;
	private float driftBoostStage = 0;
	private float driftTime = 0;
	public float driftChargeLength;
	public float driftBoostPower;
	public float driftBoostDecay;
	private Vector2 angularControl;
	public float baseSpeed = 0.7f;
	private float speed;
	public float baseTurnRadius = 20;
	private float turnRadius = 20;
	private float turnSpeed;
	public float baseTraction;
	private float traction;
	private Vector3 angularVelocity = new();
	private Vector3 gravity;
	public float gravityStrength;
	public float baseFriction = 0.3f;
	public float friction;
	public float baseAirResistance = 0.01f;
	public float airResistance;
	public float angularAirResistance;
	public float cameraSmoothing;
	public float mass = 1000;
	public Array<Ability> abilities = new();
	public float cameraOffsetSmoothing = 0.2f;
	private readonly Dictionary<Key, bool> Inputs = new()
	{
		[Key.W] = false,
		[Key.A] = false,
		[Key.S] = false,
		[Key.D] = false,
		[Key.E] = false,
		[Key.Q] = false,
		[Key.C] = false,
		[Key.Space] = false,
		[Key.V] = false,
		[Key.Shift] = false
	};

	public void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		gravity = gravityStrength * (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
		cameraRotationalOffset = baseCameraRotationalOffset;
	}
	public void _Process(double delta)
	{
		if (mouseLocked && !freelook)
		{
			if (Inputs[Key.Shift])
			{
				cameraRotationalOffset = cameraRotationalOffset.LerpAngleDeg(baseCameraRotationalOffset + new Vector3(0, 180, 0), cameraOffsetSmoothing);
			}
			else
			{

				if (drifting)
				{
					cameraRotationalOffset = cameraRotationalOffset.LerpAngleDeg(baseCameraRotationalOffset + new Vector3(0, driftDirection * driftStrength * -15, 0), cameraOffsetSmoothing);

				}
				else
				{
					cameraRotationalOffset = cameraRotationalOffset.LerpAngleDeg(baseCameraRotationalOffset, cameraOffsetSmoothing);
				}
			}
			camera.RotationDegrees = camera.RotationDegrees.LerpAngleDeg(player.RotationDegrees + cameraRotationalOffset, 0.2f);
		}
		camera.Position = camera.Position.Lerp(player.Position + camera.GlobalBasis.Z * cameraZoom + camera.GlobalBasis.Z * cameraOffset.Z + camera.GlobalBasis.X * cameraOffset.X + camera.GlobalBasis.Y * cameraOffset.Y, cameraSmoothing);

		driftParticle.Emitting = false;
		boostParticle.Emitting = false;
		if (drifting)
		{
			driftParticle.Emitting = true;
			if (driftTime >= driftChargeLength)
			{
				((StandardMaterial3D)driftParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage3DriftColor;
			}
			else if (driftTime >= driftChargeLength * 2 / 3)
			{
				((StandardMaterial3D)driftParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage2DriftColor;
			}
			else if (driftTime >= driftChargeLength * 1 / 3)
			{
				((StandardMaterial3D)driftParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage1DriftColor;
			}
			else
			{
				((StandardMaterial3D)driftParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage0DriftColor;
			}
		}
		if (driftBoostStage > 0)
		{
			boostParticle.Emitting = true;
			if (driftBoostStage == 3)
			{
				((StandardMaterial3D)boostParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage3DriftColor;
			}
			else if (driftBoostStage == 2)
			{
				((StandardMaterial3D)boostParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage2DriftColor;
			}
			else if (driftBoostStage == 1)
			{
				((StandardMaterial3D)boostParticle.Mesh.SurfaceGetMaterial(0)).AlbedoColor = stage1DriftColor;
			}
		}
	}
	public void _PhysicsProcess(double delta)
	{
		turnRadius = baseTurnRadius;

		turnSpeed = player.Velocity.Length() / (Mathf.Pow(turnRadius, 2) * Mathf.Pi / 360) / (60);

		speed = baseSpeed * (1 + driftBoost);

		turnRadius = baseTurnRadius;

		if (Inputs[Key.W])
		{
			player.Velocity -= player.GlobalBasis.Z * speed;
		}
		if (Inputs[Key.S])
		{
			player.Velocity += player.GlobalBasis.Z * speed;
		}

		angularControl = new();
		if (Inputs[Key.A])
		{
			angularControl += new Vector2(1, 0);
		}
		if (Inputs[Key.D])
		{
			angularControl += new Vector2(-1, 0);
		}
		if (Inputs[Key.Q])
		{
			angularControl += new Vector2(1, 1);
		}
		if (Inputs[Key.E])
		{
			angularControl += new Vector2(-1, 1);
		}
		bool forward = true;
		if (player.Velocity.Normalized().Dot(player.GlobalBasis.Z) > 0.5)
		{
			forward = false;
		}
		if (angularControl.Length() == 0)
		{
			angularControl = new(0, 1);
		}
		if (drifting)
		{
			driftStrength = 1 + angularControl.X * driftDirection * 0.7f;
			angularVelocity = new Vector3(0, turnSpeed * driftStrength * driftDirection, 0);
		}
		else
		{
			angularVelocity = new Vector3(0, turnSpeed * angularControl.Normalized().X * (forward ? 1 : -1), 0);
		}


		player.Velocity += gravity;

		angularAirResistance = baseAirResistance * angularVelocity.Y;
		angularVelocity -= new Vector3(0, angularAirResistance, 0);

		airResistance = baseAirResistance * player.Velocity.Length();
		player.Velocity -= player.Velocity.Normalized() * airResistance;

		friction = baseFriction * gravityStrength;
		player.Velocity -= player.Velocity.Normalized() * Mathf.Clamp(friction, 0, player.Velocity.Length());

		player.RotationDegrees += angularVelocity;

		//traction

		if (!airborn)
		{
			float convertedMagnitude = player.Velocity.Length() * traction;
			player.Velocity *= (1 - traction);
			player.Velocity += player.GlobalBasis.Z * convertedMagnitude;
		}

		player.MoveAndSlide();

		//Handles pushables
		KinematicCollision3D collision = player.GetLastSlideCollision();
		if (collision is not null)
		{
			for (int i = 0; i < collision.GetCollisionCount(); i++)
			{
				Node3D _obstacle = (Node3D)((Node3D)collision.GetCollider(i)).GetParent();
				if (_obstacle.GetOutrunClass() == "Pushable")
				{
					Obstacle obstacle = (Obstacle)_obstacle;
					RigidBody3D body = (RigidBody3D)obstacle.body;
					body.ApplyCentralForce(collision.GetTravel() * mass);
				}
			}
		}
		airborn = !player.IsOnFloor();

		if (drifting)
		{
			driftTime += 1;
		}
		if (driftBoost > 0.01)
		{
			driftBoost *= 1 - driftBoostDecay;
		}
		else
		{
			driftBoost = 0;
			driftBoostStage = 0;
		}

		snapshots.Add(new Godot.Collections.Array() { player.Position, player.Rotation });
		if (snapshots.Count > ghostDelay)
		{
			ghost.Position = (Vector3)((Godot.Collections.Array)snapshots[0])[0];
			ghost.Rotation = (Vector3)((Godot.Collections.Array)snapshots[0])[1];
			snapshots.RemoveAt(0);
		}
	}
	public void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Pressed)
			{ 
				for (int i = 0; i < abilities.Count; i++)
				{
					Ability ability = abilities[i];
					if (keyEvent.Keycode == (Key.Key1 + i) && !ability.onCooldown)
					{
						ability.OnUse(player);
						ability.uses -= 1;
						if (ability.uses < 1)
						{
							abilities.Remove(ability);
							RefreshAbilityUi();
						}
						ability.TriggerCooldown();
						return;
					}
				}
			}
			if (Inputs.ContainsKey(keyEvent.Keycode))
			{
				Inputs[keyEvent.Keycode] = keyEvent.Pressed;
			}
			if (keyEvent.Keycode == Key.Escape && keyEvent.Pressed)
			{
				mouseLocked = !mouseLocked;
				if (mouseLocked)
				{
					Input.MouseMode = Input.MouseModeEnum.Captured;
				}
				else
				{
					Input.MouseMode = Input.MouseModeEnum.Visible;
				}
			}
			if (keyEvent.Keycode == Key.Space)
			{
				if (keyEvent.Pressed)
				{
					if (!drifting && !airborn)
					{
						player.Velocity += Vector3.Up * 15;
						if (angularControl.X != 0)
						{
							drifting = true;
							driftTime = 0;
							driftDirection = angularControl.X / Mathf.Abs(angularControl.X);
						}
					}
				}
				else
				{
					if (drifting)
					{
						if (driftTime >= driftChargeLength)
						{
							driftBoostStage = 3;
							driftBoost += driftBoostPower;
						}
						else if (driftTime >= driftChargeLength * 2 / 3)
						{
							driftBoostStage = 2;
							driftBoost += driftBoostPower / 3 * 2;
						}
						else if (driftTime >= driftChargeLength * 1 / 3)
						{
							driftBoostStage = 1;
							driftBoost += driftBoostPower / 3;
						}
					}
					drifting = false;
				}
			}
		}
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Right)
			{
				freelook = mouseButtonEvent.Pressed;
			}
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp)
			{
				cameraZoom *= 0.9f;
			}
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				cameraZoom *= 1.1f;
			}
		}
		if (@event is InputEventMouseMotion mouseEvent)
		{
			if (freelook)
			{
				camera.RotationDegrees += new Vector3(mouseEvent.ScreenRelative.Y * -0.1f, mouseEvent.ScreenRelative.X * -0.1f, 0);
			}
		}
	}

	public void AddAbility(Ability toAdd)
	{
		foreach (Ability ability in abilities)
		{
			if (ability.GetOutrunClass() == toAdd.GetOutrunClass())
			{
				ability.uses++;
				return;
			}
		}
		abilities.Add(toAdd);
		RefreshAbilityUi();
	}
	public void RefreshAbilityUi()
	{
		World.abilityUi.Clear();
		foreach (Ability ability in abilities)
		{
			World.abilityUi.AddItem(ability.uses + " " + ability.GetOutrunClass(), null, false);
		}

	}
}
