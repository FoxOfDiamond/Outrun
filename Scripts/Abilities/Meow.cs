using Godot;
using System;

namespace Outrun;

[GlobalClass]
public partial class Meow : Ability
{
	public override void OnUse(Player target)
	{
		GD.Print("Meow");
	}
}
