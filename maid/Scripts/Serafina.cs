using Godot;
using System;

public partial class Serafina : CharacterBody2D
{
	public const float Speed = 800.0f;

	public int health = 4;

	public AnimatedSprite2D AnimatedSprite;


    public override void _Ready()
    {
        base._Ready();
		AnimatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		AnimatedSprite.Animation = "health";
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;


		// Movement
		if (Input.IsActionPressed("left"))
		{
			velocity.X = -1 * Speed;
			AnimatedSprite.FlipH = true;
		}
		else if (Input.IsActionPressed("right"))
		{
			velocity.X = 1 * Speed;
			AnimatedSprite.FlipH = false;
		}

		// Friction
		velocity.X *= (float)0.95;
		Velocity = velocity;
		MoveAndSlide();
	}

	// Converts health (4-0) to respective frame number (0-4)
	public int convertToFrame() {
		int convertedFrame = 4 - health;
		return convertedFrame;
	}
}
