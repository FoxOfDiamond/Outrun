using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Outrun;

public class PlayerController
{
	public Player player;
	public Camera3D camera;
	public Vector3 cameraOffset = new(0, 4, 0);
	public Vector3 cameraRotationalOffset = new(-20, 0, 0);
	private bool mouseLocked = true;
	private bool freelook = false;
	public float cameraZoom = 10;
	private bool drifting = false;
	public float baseSpeed = 0.7f;
	private float speed;
	public float baseTurnRadius = 20;
	private float turnRadius = 20;
	private float turnSpeed;
	private Vector3 angularVelocity = new();
	private Vector3 gravity;
	public float gravityStrength;
	public float friction;
	public float rollResistance;
	public float cameraSmoothing;
	public float mass = 1000;
	public Array<Ability> abilities;
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
		[Key.Key1] = false,
		[Key.Key2] = false,
		[Key.Key3] = false
	};
	private Dictionary<Key, bool> InputJustPressed = [];

	public void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		gravity = gravityStrength * (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
		speed = baseSpeed;
		turnRadius = baseTurnRadius;

		foreach (var thisInput in Inputs)
		{
			InputJustPressed.Add(thisInput.Key, thisInput.Value);
		}

	}
	public void _Process(double delta)
	{
		if (mouseLocked && !freelook)
		{
			if (drifting)
			{
				camera.RotationDegrees = camera.RotationDegrees.LerpAngleDeg(player.RotationDegrees + cameraRotationalOffset + new Vector3(0, angularVelocity.Y > 0 ? -15 : 15, 0), cameraSmoothing);

			}
			else
			{
				camera.RotationDegrees = camera.RotationDegrees.LerpAngleDeg(player.RotationDegrees + cameraRotationalOffset, 0.2f);
			}
		}
		camera.Position = camera.Position.Lerp(player.Position + camera.GlobalBasis.Z * cameraZoom + camera.GlobalBasis.Z * cameraOffset.Z + camera.GlobalBasis.X * cameraOffset.X + camera.GlobalBasis.Y * cameraOffset.Y, cameraSmoothing);
	}
	public void _PhysicsProcess(double delta)
	{
		if (Inputs[Key.W])
		{
			player.Velocity -= player.GlobalBasis.Z * speed;
		}
		if (Inputs[Key.S])
		{
			player.Velocity += player.GlobalBasis.Z * speed;
		}
		if (drifting)
		{
			if (Mathf.Abs(angularVelocity.Y) < 3)
			{
				drifting = false;
			}
			turnRadius = baseTurnRadius * 0.7f;
		}
		else
		{
			turnRadius = baseTurnRadius;
		}
		turnSpeed = player.Velocity.Length() / (Mathf.Pow(turnRadius, 2) * Mathf.Pi / 360) / (60);
		if (Inputs[Key.A])
		{
			angularVelocity = new Vector3(0, turnSpeed, 0);
		}
		if (Inputs[Key.D])
		{
			angularVelocity = new Vector3(0, -turnSpeed, 0);
		}
		if (Inputs[Key.Q])
		{
			angularVelocity = new Vector3(0, turnSpeed * 0.6f, 0);
		}
		if (Inputs[Key.E])
		{
			angularVelocity = new Vector3(0, -turnSpeed * 0.6f, 0);
		}
		player.RotationDegrees += angularVelocity;
		angularVelocity *= friction;
		player.Velocity += gravity * 0.1f;
		player.Velocity *= friction;
		player.MoveAndSlide();

		//Abilities
		foreach (var thisAbility in abilities)
		{
			if (thisAbility.OnCooldown)
			{
				thisAbility._Process(delta);
			}
			else
			{
				if (InputJustPressed[thisAbility.myKey])
				{
					thisAbility.OnUse(player);
					thisAbility.Uses -= 1;
					if (thisAbility.Uses < 1)
					{
						abilities.Remove(thisAbility);
					}
					thisAbility.OnCooldown = true;
					thisAbility.Counter = 0;
				}
			}
		}

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

		//Reset inputs
		foreach (var thisKey in InputJustPressed.Keys)
		{
			InputJustPressed[thisKey] = false;
		}
	}
	public void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
			if (Inputs.ContainsKey(keyEvent.Keycode))
			{
				if (!Inputs[keyEvent.Keycode]) InputJustPressed[keyEvent.Keycode] = keyEvent.Pressed;
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
				if (keyEvent.Pressed && (player.IsOnFloor() || drifting))
				{
					if (!drifting)
					{
						player.Velocity += Vector3.Up * 15;
					}
					drifting = true;
				}
				else
				{
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
		foreach (var thisAbility in abilities)
		{
			if (thisAbility.uniqueName == toAdd.uniqueName)
			{
				thisAbility.Uses++;
				return;
			}
		}
		abilities.Add(toAdd);
	}
}
