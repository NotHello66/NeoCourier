using Godot;
using System;
using System.Collections.Generic;

public partial class PackageComponent : Node3D
{
	[Export] public bool hasPackage = false;
    [Export] Node3D objective;
    public override void _Ready()
    {
    }


    public override void _PhysicsProcess(double delta)
    {
        if(hasPackage == true)
        {
            objective.Visible = true;
        }
        else
        {
            objective.Visible = false;
        }
    }
}
