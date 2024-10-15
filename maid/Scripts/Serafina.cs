using Godot;
using System;

public partial class Serafina : CharacterBody2D
{
	public const float Speed = 800.0f;

	public int health = 4;

	public AnimatedSprite2D AnimatedSprite;

	public Area2D Plate;

	[Signal]
	public delegate void FoodObtainedEventHandler(string food, int foodIndex);

	[Signal]
	public delegate void DishMergedEventHandler(string dish, int dishIndex);

	public Godot.Collections.Array HeldFood = new Godot.Collections.Array{

	};

    public override void _Ready()
    {
        base._Ready();
		AnimatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		Plate = (Area2D)GetNode("Plate");

		RigidBody2D dishBody = (RigidBody2D)Plate.GetNode("Dish");
		AnimatedSprite2D dishSprite = (AnimatedSprite2D)dishBody.GetNode("DishSprite");
		dishSprite.Hide();

		AnimatedSprite.Animation = "health";
    	FoodObtained += GetFood;
		DishMerged += GetDish;
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

	// Add food to held
	public void GetFood(string food, int foodIndex) {
		// Check if food can be added
		if (HeldFood.Count < 3) {
			HeldFood.Add(food);
			int newLen = HeldFood.Count;
			int newInd = newLen - 1;
			string placePos = "Food" + newInd;
			RigidBody2D foodBody = (RigidBody2D)Plate.GetNode(placePos);
			AnimatedSprite2D foodSprite = (AnimatedSprite2D)foodBody.GetNode("FoodSprite");
			foodSprite.Frame = foodIndex;
			foodBody.Visible = true;
			mergeFood();
		}
	}

	private void GetDish(string dish, int dishIndex) {
		GD.Print("Found dish!");
		for (int i = 0; i < 3; i++) {
			string placePos = "Food" + i;
			RigidBody2D foodBody = (RigidBody2D)Plate.GetNode(placePos);
			AnimatedSprite2D foodSprite = (AnimatedSprite2D)foodBody.GetNode("FoodSprite");
			foodSprite.Hide();
		}
		RigidBody2D dishBody = (RigidBody2D)Plate.GetNode("Dish");
		AnimatedSprite2D dishSprite = (AnimatedSprite2D)dishBody.GetNode("DishSprite");
		dishSprite.Show();
		dishSprite.Frame = dishIndex;
	}

	// Check if any ingredients can be merged
	private void mergeFood() {
		if (HeldFood.Count >= 2) {
			GD.Print(GetParent().Name);
			GameManager parent = (GameManager)GetParent();
			parent.EmitSignal(GameManager.SignalName.FoodObtained, HeldFood);
		}
	}
}
