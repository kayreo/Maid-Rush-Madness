using Godot;
using System;


public partial class FoodSpawner : Node2D
{
	[Export]
	public PackedScene Food { get; set; }

	public Timer SpawnTimer;

	public RandomNumberGenerator Random = new RandomNumberGenerator();

	private LevelManager Parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random.Randomize();
		SpawnTimer = (Timer)GetNode("Timer");
		Parent = (LevelManager)GetParent().GetParent();
		SpawnTimer.WaitTime = Random.RandiRange(1, 7);
		SpawnTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	protected void _OnTimerTimeout() {
		if (!Parent.paused) {
			Food newFood = (Food)Food.Instantiate();
			Sprite2D newFoodVis = (Sprite2D)newFood.GetNode("Visual");
			AddChild(newFood);
			GetParent().GetParent().EmitSignal("PickRandomFood", newFood);
			SpawnTimer.WaitTime = Random.RandiRange(1, 7);
		}
	}

}
