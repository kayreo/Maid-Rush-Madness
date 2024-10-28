using Godot;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


public partial class GameManager : Node2D
{
	private FoodTree foodTree;

	private Godot.Collections.Dictionary Recipes;

	private Serafina player;

	private Json jsonLoader;

	[Signal]
	public delegate void FoodObtainedEventHandler(Godot.Collections.Array FoodToMerge);

	[Signal]
	public delegate void PlaceDishEventHandler(string DishToPlace);

	public Godot.Collections.Dictionary FoodDict = new Godot.Collections.Dictionary{

	};


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FoodObtained += MergeFood;
		PlaceDish += OnPlaceDish;
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
		bool foundRecipe = false;
		foreach (string recipe in Recipes.Keys) {
			GD.Print(Recipes[recipe]);
			foundRecipe = false;
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
					foundRecipe = true;
				}
			}
			if (foundRecipe) {
				GD.Print("This recipe");
				whichRecipe = recipe;
				break;
			}
		}
		// Did not find a recipe and hit max number of ingredients
		if (!foundRecipe && FoodToMerge.Count >= 3) {
			whichRecipe = "slop";
			player.EmitSignal(Serafina.SignalName.DishMerged, whichRecipe, 0);
		} else {
			GD.Print("Merging into: " + whichRecipe);
			player.EmitSignal(Serafina.SignalName.DishMerged, whichRecipe, 0);
		}
	}
	private void OnPlaceDish(string dish) {
		GD.Print("Placing dish");

	}
}


public class FoodNode {
	public string ingredient;
	public string recipe;

	public FoodNode[] children;
	public FoodNode(string ingredientName) {
		ingredient = ingredientName;
	}
}

public class FoodTree {
	public FoodNode root = new FoodNode(null);

	public FoodTree(Godot.Collections.Dictionary Recipes) {
		foreach (string recipe in Recipes.Keys) {
			string recipeName = recipe;
			Godot.Collections.Array recipeToCheck = (Godot.Collections.Array)Recipes[recipe];
			GD.Print("Adding: ", recipeToCheck, recipeName);
			insert(root, )
		}
	}

	public void insert(FoodNode root, Godot.Collections.Array foods, string recipeName, int start) {
		if (start >= foods.Count) {
			root.recipe = recipeName;
			return;
		}
		string foodToAdd = (string)foods[start];
		FoodNode newNode = new FoodNode(foodToAdd);
		root.children.Append(newNode);
	}

	/*
        newNode = Node (val, val2)
        if (self.root == None):
            self.root = newNode
        else:
            current = self.root
            parent = self.root
            while (current != None):
                parent = current
                if (val < current.id):
                    current = current.lChild
                else:
                    current = current.rChild
            if (val < parent.id):
                parent.lChild = newNode
            else:
                parent.rChild = newNode

	*/


	/*
    def __init__(self, data):
        self.root = TrieNode()
        #TODO: Student
        for d in data:
            split = d.split(", ")
            self.insert(self.root, split[0].lower(), split[1], 0)

    def insert(self, root, word, url, start):
        if start >= len(word):
            root.set_url(url)
            return
        if root.children[ord(word[start]) - 97] is None:
            root.children[ord(word[start]) - 97] = TrieNode()
        self.insert(root.children[ord(word[start]) - 97], word, url, start + 1)

	*/
}