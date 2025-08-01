using System;
using Godot;
using Godot.Collections;

namespace Outrun;

public partial class Player : CharacterBody3D
{
	public Camera3D Camera;
	private Vector3 CameraOffset = new(0,4,0);
	private Vector3 CameraRotationalOffset = new(-20,0,0);
	private bool mouseLocked = true;
	private bool freelook = false;
	private float zoom = 10;
	private bool drifting = false;
	private float baseSpeed = 0.7f;
	private float baseTurnSpeed = 0.3f;
	private float speed;
	private float turnSpeed;
	private Vector3 AngularVelocity = new();
	private Vector3 gravity;
	private readonly Dictionary<Key, bool> Inputs = new Dictionary<Key, bool>()
	{
		[Key.W] = false,
		[Key.A] = false,
		[Key.S] = false,
		[Key.D] = false,
		[Key.E] = false,
		[Key.Q] = false,
		[Key.C] = false,
		[Key.Space] = false
	};

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>("../Camera");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * (Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
		speed = baseSpeed;
		turnSpeed = baseTurnSpeed;

	}
	public override void _Process(double delta)
	{
		if (mouseLocked && !freelook)
		{
			if (drifting)
			{
				Camera.RotationDegrees = RotationDegrees + CameraRotationalOffset + new Vector3(0, AngularVelocity.Y > 0 ? -15 : 15, 0);

			}
			else
			{
				Camera.RotationDegrees = RotationDegrees + CameraRotationalOffset;
			}
		}
		Camera.Position = Position + Camera.GlobalBasis.Z * zoom + Camera.GlobalBasis.Z * CameraOffset.Z + Camera.GlobalBasis.X * CameraOffset.X + Camera.GlobalBasis.Y * CameraOffset.Y;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Inputs[Key.W])
		{
			Velocity -= GlobalBasis.Z * speed;
		}
		if (Inputs[Key.S])
		{
			Velocity += GlobalBasis.Z * speed;
		}
		if (drifting)
		{
			if (Mathf.Abs(AngularVelocity.Y) < 3)
			{
				drifting = false;
			}
			turnSpeed = baseTurnSpeed * 0.7f;
		}
		else
		{
			turnSpeed = baseTurnSpeed;
		}
		if (Inputs[Key.A])
		{
			AngularVelocity += new Vector3(0, turnSpeed, 0);
		}
		if (Inputs[Key.D])
		{
			AngularVelocity -= new Vector3(0,turnSpeed,0);
		}
		RotationDegrees += AngularVelocity;
		AngularVelocity *= 0.9f;
		Velocity += gravity * 0.1f;
		Velocity *= 0.95f;
		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventKey keyEvent)
		{
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
				if (keyEvent.Pressed && (IsOnFloor() || drifting))
				{
					if (!drifting)
					{  
						Velocity += Vector3.Up * 15;
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
				zoom *= 0.9f;
			}
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				zoom *= 1.1f;
			}
		}
		if (@event is InputEventMouseMotion mouseEvent)
		{
			if (freelook)
			{
				Camera.RotationDegrees += new Vector3(mouseEvent.ScreenRelative.Y * -0.1f, mouseEvent.ScreenRelative.X * -0.1f, 0);
			}
		}
	}

}
