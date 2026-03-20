using Godot;
using System;

public partial class Missle : RigidBody3D
{
    Node3D target;
    [Export] float speed;
    [Export] public float turnspeed = 1.5f;
    [Export] float aoeRadius = 1.5f;
    [Export] CollisionObject3D collisionObject;
    [Export] PackedScene explosionScene;
    [Export] double lifetime = 10;
    double timer;
    [Export] float maxTrackingAngle = 60f;
    float maxTrackingDot;
    bool isTracking = true;
    public void Initialize(Node3D target, float speed)
    {
        this.target = target;
        this.speed = speed;
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        BodyEntered += OnBodyEntered;
        maxTrackingDot = Mathf.Cos(Mathf.DegToRad(maxTrackingAngle));
        //GD.Print("Missle max tracking dot: " + maxTrackingDot);
    }
    public void OnBodyEntered(Node body)
    {
        GD.Print("Missle hit: " + body.Name);
        var explosion = explosionScene.Instantiate() as GpuParticles3D;
        GetTree().Root.AddChild(explosion);
        explosion.GlobalPosition = GlobalPosition;
        explosion.Emitting = true;
        FpScontroller player = GetTree().GetFirstNodeInGroup("Player") as FpScontroller;
        if (GlobalPosition.DistanceTo(player.GlobalPosition) < aoeRadius)
        {
            player.isStunned = true;
            player.stunTimer = 0f;
                
        }
        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
    public override void _PhysicsProcess(double delta)
    {
        timer += delta;
        if(timer > lifetime)
        {
            var explosion = explosionScene.Instantiate() as GpuParticles3D;
            GetTree().Root.AddChild(explosion);
            explosion.GlobalPosition = GlobalPosition;
            explosion.Emitting = true;
            FpScontroller player = GetTree().GetFirstNodeInGroup("Player") as FpScontroller;
            if (GlobalPosition.DistanceTo(player.GlobalPosition) < aoeRadius)
            {
                player.isStunned = true;
                player.stunTimer = 0f;

            }
            QueueFree();
        }
        if (target == null || !IsInstanceValid(target))
        {
            QueueFree();
        }
        Vector3 toTarget;
        try
        {
            Vector3 i = new Vector3(target.GlobalPosition.X, target.GlobalPosition.Y + 1f, target.GlobalPosition.Z);
            toTarget = (i - GlobalPosition).Normalized();
        }
        catch (ObjectDisposedException)
        {
            QueueFree();
            return;
        }
        Vector3 forward = -GlobalTransform.Basis.Z;
        Vector3 toTargetDir = (new Vector3(target.GlobalPosition.X, target.GlobalPosition.Y + 1f, target.GlobalPosition.Z) - GlobalPosition).Normalized();

        float dot = forward.Dot(toTargetDir);
        //GD.Print("Missle dot: " + dot);
        isTracking = dot >= maxTrackingDot;

        if (isTracking)
        {
            Quaternion currentRotation = GlobalTransform.Basis.GetRotationQuaternion();
            Quaternion targetRotation = Basis.LookingAt(toTargetDir, Vector3.Up).GetRotationQuaternion();
            Quaternion newRotation = currentRotation.Slerp(targetRotation, turnspeed * (float)delta);

            GlobalTransform = new Transform3D(new Basis(newRotation), GlobalPosition);
        }
        forward = -GlobalTransform.Basis.Z;
        GlobalPosition += forward * speed * (float)delta;
    }
}

