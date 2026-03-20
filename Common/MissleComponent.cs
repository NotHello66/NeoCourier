using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

public partial class MissleComponent : Node3D
{
	FpScontroller player;
	[Export] PackedScene missle;
	List<Node3D> markers = new List<Node3D>();
	Random rng = new Random();

    [Export] float speed = 20f;

    [Export] float cooldown = 1.0f;

    bool canShoot = false;
    float timer = 0.0f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Defer player lookup until the whole scene is ready
        CallDeferred(MethodName.FindPlayer);

        foreach (Node child in GetChildren())
        {
            if (child is Node3D marker)
                markers.Add(marker);
        }
    }

    private void FindPlayer()
    {
        var nodes = GetTree().GetNodesInGroup("Player");
        if (nodes.Count > 0)
        {
            player = nodes[0] as FpScontroller;
        }

        if (player == null)
        {
            GD.PrintErr("MissleComponent: Player not found! Is FpScontroller in the 'Player' group?");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}
	public override void _PhysicsProcess(double delta)
	{
        timer += (float)delta;

        var spaceState = GetWorld3D().DirectSpaceState;
        Vector3 j = new Vector3(player.GlobalPosition.X, player.GlobalPosition.Y + 1f, player.GlobalPosition.Z);
        var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, j);
        query.CollideWithBodies = true;
        query.Exclude = new Godot.Collections.Array<Rid> { GetParent<StaticBody3D>().GetRid() };
        var intersect = spaceState.IntersectRay(query);
        if (intersect.Count == 0) return;
        var collider = (Node3D)intersect["collider"];
        //GD.Print("Turret sees: " + collider.Name);
        if (collider.IsInGroup("Player"))
        {
            //GD.Print("Turret sees player");
            Node3D parent = GetParent<StaticBody3D>();
            Vector3 lookTarget = new Vector3(player.GlobalPosition.X, parent.GlobalPosition.Y, player.GlobalPosition.Z);
            GetParent<StaticBody3D>().LookAt(lookTarget, Vector3.Up);
            //foreach (Node3D marker in markers)
            //{
            //    lookTarget.Y = marker.GlobalPosition.Y;
            //    marker.LookAt(lookTarget, Vector3.Up);
            //}

            if (timer > cooldown)
            {            
                int i = rng.Next() % markers.Count;
                var missleInstance = missle.Instantiate() as Missle;
                missleInstance.Initialize(player, speed);   
                GetTree().Root.AddChild(missleInstance);
                missleInstance.GlobalPosition = markers[i].GlobalPosition;
                missleInstance.GlobalRotation = GetParent<Node3D>().GlobalRotation;
                missleInstance.LookAt(player.GlobalPosition, Vector3.Up);
                //GD.Print("Turret sees player");
                timer = 0.0f;
            }
            
        }

    }
}
