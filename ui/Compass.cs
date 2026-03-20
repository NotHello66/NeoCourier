using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;

public partial class Compass : TextureRect
{
    [Export] FpScontroller player;
    Node PMC;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PMC = GetTree().GetFirstNodeInGroup("PMC");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _PhysicsProcess(double delta)
    {
        Node3D activeSource = PMC.Get("activePackageSource").As<Node3D>();
        Node3D activeDest = PMC.Get("activePackageDestination").As<Node3D>();
        if (activeDest == null)
        {
            GD.Print("No active package destination");
            return;
        }
        if (!player.hasPackage)
        {
            Vector3 playerPos = player.GlobalPosition;
            Vector3 dir = activeSource.GlobalPosition  - playerPos;
            //GD.Print("Player pos: " + playerPos);
           //GD.Print("Target pos: " + player.PackageController.activePackageSource.GlobalPosition);
            float targetYaw = Mathf.Atan2(dir.Z, dir.X);
            float playerRotY = player.GlobalRotation.Y;
            float angle = targetYaw + playerRotY+ Mathf.Pi / 2;
            angle = Mathf.Wrap(angle, -Mathf.Pi, Mathf.Pi);
            Rotation = angle;
            //GD.Print("Rotation towards package: " + Rotation);
        }
        else
        {
            Vector3 playerPos = player.GlobalPosition;
            Vector3 dir = activeDest.GlobalPosition - playerPos;
            float targetYaw = Mathf.Atan2(dir.Z, dir.X);
            float playerRotY = player.GlobalRotation.Y;
            float angle = targetYaw + playerRotY+ Mathf.Pi / 2;
            angle = Mathf.Wrap(angle, -Mathf.Pi, Mathf.Pi);
            Rotation = angle;
            //GD.Print("Rotation towards destination: " + Rotation);
        }
    }

}
