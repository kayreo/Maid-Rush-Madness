using Godot;
using System;

public partial class Food : RigidBody2D
{

	public enum FOOD_TYPES {
		Cupcake,
		BFlower,
		YFlower,
		PFlower,
		RFlower,
		Soda,
		Potion,
		Emet
	}


	[Export]
	public int ID { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("My id is: ", (FOOD_TYPES)ID);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnArea2DEntered(Area2D area) {
		if (area.Name == "Plate") {
			GD.Print("Hit sera");
		}
	}
}
