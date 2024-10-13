using Godot;
using System;

public partial class GameManager : Node2D
{

	[Signal]
	public delegate void FoodObtainedEventHandler(Godot.Collections.Array FoodToMerge);

	public Godot.Collections.Dictionary FoodDict = new Godot.Collections.Dictionary{

	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FoodObtained += MergeFood;
	    using var file = FileAccess.Open("res://Data/FoodData.csv", FileAccess.ModeFlags.Read);
		while (!file.EOFReached()) {
			var csv_rows = file.getCsvLine("\t");	
			GD.Print("Reading Data");	
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	// Check if any ingredients can be merged
	private void MergeFood(Godot.Collections.Array FoodToMerge) {
		GD.Print("Moved to manager");

	}
}
