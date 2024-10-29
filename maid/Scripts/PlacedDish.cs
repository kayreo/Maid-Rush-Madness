using Godot;
using System;

public partial class PlacedDish : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}


	
	private void _OnVisibleOnScreenNotifier2DScreenExited() {
		GD.Print("Left screen");
		QueueFree();
	}
}
