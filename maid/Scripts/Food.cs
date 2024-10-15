using Godot;
using System;

public partial class Food : RigidBody2D
{

	public Godot.Collections.Array FoodTypes = new Godot.Collections.Array{
		"Cupcake",
		"BFlower",
		"YFlower",
		"PFlower",
		"RFlower",
		"Soda",
		"Potion",
		"Emet"
	};


	[Export]
	public int ID { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnArea2DEntered(Area2D area) {
		if (area.Name == "Plate") {
			GD.Print("Hit sera");
			Serafina parent = (Serafina)area.GetParent();
			parent.EmitSignal(Serafina.SignalName.FoodObtained, FoodTypes[ID], ID);
		}
	}
}
