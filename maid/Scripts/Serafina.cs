using Godot;
using System;

public partial class Serafina : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

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


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		if (Input.IsActionPressed("left"))
		{
			velocity.X = -1 * Speed;
		}
		else if (Input.IsActionPressed("right"))
		{
			velocity.X = 1 * Speed;
		}

		velocity.X *= (float)0.95;
		Velocity = velocity;
		MoveAndSlide();
	}
}
