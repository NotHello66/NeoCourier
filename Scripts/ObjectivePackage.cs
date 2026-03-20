using Godot;
using System;

public partial class ObjectivePackage : Node3D
{
	[Export] Node3D objective;
	[Export] float BobbingSpeed = 2f;
    [Export] float BobbingAmount = 0.5f;
	[Export] float RotationSpeed = 1f;
	Vector3 basePosition;
    float BobbingTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		basePosition = objective.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _PhysicsProcess(double delta)
    {
		ObjectiveBobing(delta);
    }
	public void ObjectiveBobing(double delta)
	{
		BobbingTimer += (float)delta * BobbingSpeed;
        float offsetY = Mathf.Sin(BobbingTimer) * BobbingAmount;
        objective.Position = basePosition + new Vector3(0f, offsetY, 0f);
		objective.RotateY((float) delta *  RotationSpeed);
    }
}
