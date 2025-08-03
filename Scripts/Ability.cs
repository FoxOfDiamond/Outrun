using Godot;
using System;

namespace Outrun;

[GlobalClass]
public abstract partial class Ability : Resource
{
    [Export]
    public string uniqueName;
    [Export]
    public Key myKey;
    [Export]
    public int uses = 1;
    [Export]
    public double cooldown = 0;

    protected double lastUse = 0;
    public bool onCooldown
    {
        get
        {
            return World.physicsFrame >= lastUse + cooldown;
        }
        set
        {
            throw new Exception("Use TriggetCooldown()");
        }
    }

    public void _Process(double delta)
    {
    }

    public abstract void OnUse(Player target);
    public void TriggerCooldown()
    {
        lastUse = World.physicsFrame;
    }
}
