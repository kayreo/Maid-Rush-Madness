using Godot;
using System;

public partial class Serafina : CharacterBody2D
{
	public const float Speed = 800.0f;

	public int health = 4;

	private bool madeDish = false;

	public AnimatedSprite2D AnimatedSprite;

	public CpuParticles2D ParticleEffect;

	private GameManager parent;

	public Area2D Plate;

	[Signal]
	public delegate void FoodObtainedEventHandler(string food, Texture2D spriteTexture);

	[Signal]
	public delegate void DishMergedEventHandler(string food, Texture2D spriteTexture);

	[Signal]
	public delegate void TakeDamageEventHandler();

	public string heldDish;

	public Godot.Collections.Array HeldFood = new Godot.Collections.Array();

	public Godot.Collections.Array currentRequests = new Godot.Collections.Array();

    public override void _Ready()
    {
        base._Ready();
		AnimatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		ParticleEffect = (CpuParticles2D)GetNode("CPUParticles2D");
		Plate = (Area2D)GetNode("Plate");
	 	parent = (GameManager)GetParent();
		//RigidBody2D dishBody = (RigidBody2D)Plate.GetNode("Dish");
		Sprite2D dishSprite = (Sprite2D)Plate.GetNode("DishSprite");
		dishSprite.Hide();

		AnimatedSprite.Animation = "health";
    	FoodObtained += GetFood;
		DishMerged += GetDish;
		TakeDamage += OnTakeDamage;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;


		// Movement
		if (Input.IsActionPressed("left"))
		{
			velocity.X = -1 * Speed;
			//AnimatedSprite.FlipH = true;
		}
		else if (Input.IsActionPressed("right"))
		{
			velocity.X = 1 * Speed;
			//AnimatedSprite.FlipH = false;
		}

		if (Input.IsActionPressed("place")) {
			HeldFood.Clear();
			ClearHolding();
			//GD.Print("Holding now: ", HeldFood);
			if (madeDish) {
				GD.Print("Can place on thing");
				madeDish = false;
				//RigidBody2D dishBody = (RigidBody2D)Plate.GetNode("Dish");
				Sprite2D dishSprite = (Sprite2D)Plate.GetNode("DishSprite");
				parent.EmitSignal(GameManager.SignalName.PlaceDish, dishSprite, heldDish);
				dishSprite.Texture = null;
				dishSprite.Hide();
				//dishCollision.Disabled = true;
				heldDish = null;
			} else {
				//GD.Print("Can't place on thing");
			}
		}

		// Friction
		velocity.X *= (float)0.95;
		Velocity = velocity;
		MoveAndSlide();
	}

	// Converts health (4-0) to respective frame number (0-3)
	public int convertToFrame() {
		int convertedFrame = 4 - health;
		return convertedFrame;
	}

	// Add food to held
	public void GetFood(string food, Texture2D spriteTexture) {
		// Check if food can be added
		if (HeldFood.Count < 3) {
			HeldFood.Add(food);
			int newLen = HeldFood.Count;
			int newInd = newLen - 1;
			string placePos = "FoodSprite" + newInd;
			GD.Print("Getting: ", placePos);
			Sprite2D foodSprite = (Sprite2D)Plate.GetNode(placePos);
			foodSprite.Texture = spriteTexture; 
			foodSprite.Visible = true;
			mergeFood();
			ParticleEffect.Texture = spriteTexture;
			ParticleEffect.Emitting = true;
		}
	}

	private void ClearHolding() {
		for (int i = 0; i < 3; i++) {
			string placePos = "FoodSprite" + i;
			//RigidBody2D foodBody = (RigidBody2D)Plate.GetNode(placePos);
			Sprite2D foodSprite = (Sprite2D)Plate.GetNode(placePos);
			foodSprite.Texture = null;
			//CollisionShape2D foodCollision = (CollisionShape2D)foodBody.GetNode("CollisionShape2D");
			foodSprite.Hide();
			//foodCollision.Disabled = true;
		}
	}

	private void GetDish(string dish, Texture2D spriteTexture) {
		ClearHolding();
		//RigidBody2D dishBody = (RigidBody2D)Plate.GetNode("Dish");
		Sprite2D dishSprite = (Sprite2D)Plate.GetNode("DishSprite");
		//CollisionShape2D dishCollision = (CollisionShape2D)dishBody.GetNode("CollisionShape2D");
		dishSprite.Show();
		//dishCollision.Disabled = false;
		dishSprite.Texture = spriteTexture;
		heldDish = dish;
		madeDish = true;
	}

	// Check if any ingredients can be merged
	private void mergeFood() {
		if (HeldFood.Count > 2) {
			GD.Print(GetParent().Name);
			parent.EmitSignal(GameManager.SignalName.FoodObtained, HeldFood);
		}
	}

	private void OnTakeDamage() {
		health--;
		if (health <= 0) {
			GD.Print("Game over!");
			parent.EmitSignal(GameManager.SignalName.GameOver);
		} else {
			AnimatedSprite.Frame = convertToFrame();
		}
	}
}
