using Godot;
using System;

namespace Outrun;

[GlobalClass]
public partial class Meow : Ability
{
    protected override string outrunClass { get; set; } = "Meow";
	public override void OnUse(Player target)
	{
		GD.Print("Meow");
	}
}
