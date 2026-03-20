using Godot;
using System;

public partial class Speedometer : Label
{
	[Export] CharacterBody3D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}
	public override void _PhysicsProcess(double delta)
	{
		Text = "Speed: " + player.Velocity.Length().ToString("F3");
	}
}
