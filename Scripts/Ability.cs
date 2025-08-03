using Godot;
using System;

namespace Outrun;

[GlobalClass]
public abstract partial class Ability : Resource
{
    protected virtual string outrunClass { get; set; } = "Ability";
    [Export]
    public int uses = 1;
    [Export]
    public int cooldown = 0;
    //Counted in physics ticks
    protected int lastUse = 0;
    public bool onCooldown
    {
        get
        {
            return World.physicsFrame < (lastUse + cooldown);
        }
        set
        {
            throw new Exception("Use TriggerCooldown()");
        }
    }
    public string GetOutrunClass()
    {
        return outrunClass;
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
