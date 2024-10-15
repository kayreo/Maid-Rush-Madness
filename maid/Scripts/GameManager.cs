using Godot;
using System;
using System.IO;
using System.Linq;


public partial class GameManager : Node2D
{

	private Godot.Collections.Dictionary Recipes;

	private Serafina player;

	private Json jsonLoader;

	[Signal]
	public delegate void FoodObtainedEventHandler(Godot.Collections.Array FoodToMerge);

	public Godot.Collections.Dictionary FoodDict = new Godot.Collections.Dictionary{

	};


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FoodObtained += MergeFood;
		jsonLoader = new Json();
		var file = File.ReadAllText("Data/FoodData.json");
		jsonLoader.Parse(file);
		Recipes = (Godot.Collections.Dictionary)jsonLoader.Data;
		player = (Serafina)GetNode("Serafina");
		GD.Print("I got: " + player.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	// Check if any ingredients can be merged
	private void MergeFood(Godot.Collections.Array FoodToMerge) {
		GD.Print("Moved to manager");
		string whichRecipe = "";
		bool foundRecipe = true;
		foreach (string recipe in Recipes.Keys) {
			GD.Print(Recipes[recipe]);
			foundRecipe = true;
			Godot.Collections.Array recipeToCheck = (Godot.Collections.Array)Recipes[recipe];
			if (recipeToCheck.Count == FoodToMerge.Count) {
				for (int ingredient = 0; ingredient < recipeToCheck.Count; ingredient++) {
					GD.Print("Checking list: " + FoodToMerge);
					GD.Print("Checking ingredient: " + (string)recipeToCheck[ingredient]);
					if (!FoodToMerge.Contains((string)recipeToCheck[ingredient])) {
						GD.Print("Not this recipe");
						foundRecipe = false;
						break;
					}
				}
			}
			if (foundRecipe) {
				GD.Print("This recipe");
				whichRecipe = recipe;
				break;
			}
		}
		GD.Print("Merging into: " + whichRecipe);
		player.EmitSignal(Serafina.SignalName.DishMerged, whichRecipe, 0);
	}
}
