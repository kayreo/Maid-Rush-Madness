using Godot;
using System;


public partial class FoodSpawner : Node2D
{
	[Export]
	public PackedScene Food { get; set; }

	public Timer SpawnTimer;

	public RandomNumberGenerator Random = new RandomNumberGenerator();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random.Randomize();
		SpawnTimer = (Timer)GetNode("Timer");
		SpawnTimer.WaitTime = Random.RandiRange(5, 20);
		SpawnTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	protected void _OnTimerTimeout() {
		Food newFood = (Food)Food.Instantiate();
		AnimatedSprite2D newFoodVis = (AnimatedSprite2D)newFood.GetNode("Visual");
		int randFood = Random.RandiRange(0, 7);
		newFoodVis.Frame = randFood;
		newFood.ID = randFood;
		AddChild(newFood);
		SpawnTimer.WaitTime = Random.RandiRange(5, 20);
	}
}
