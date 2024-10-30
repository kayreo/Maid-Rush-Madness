using Godot;
using System;

public partial class Table : Node2D
{
	public Sprite2D belt;
	public float interval = 0.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		belt = (Sprite2D)GetNode("ConveyorBelt");
		(belt.Material as ShaderMaterial).SetShaderParameter("interval", interval);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		interval += 0.01f * (float)delta;
		(belt.Material as ShaderMaterial).SetShaderParameter("interval", interval);
	}
}
