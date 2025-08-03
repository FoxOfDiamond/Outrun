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
    public int Uses = 1;
    [Export]
    public double Cooldown = 0;

    public double Counter = 0;
    public bool OnCooldown = false;

    public void _Process(double delta)
    {
        if (Cooldown > 0 && Counter < Cooldown)
        {
            Counter += delta;
        }
        else
        {
            OnCooldown = false;
        }
    }

    public abstract void OnUse(Player target);
}
