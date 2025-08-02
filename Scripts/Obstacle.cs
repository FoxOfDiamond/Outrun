using Godot;

namespace Outrun;

[GlobalClass]
public partial class Obstacle : Node3D
{
    public virtual Node3D body { get; set; }
}