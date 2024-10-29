using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;


public partial class GameManager : Node2D
{

	[Export]
	public PackedScene PlacedDish { get; set; }

	private FoodTree foodTree;

	private Node2D dishes;

	private Godot.Collections.Dictionary Recipes;

	private Godot.Collections.Dictionary Sprites = new Godot.Collections.Dictionary();

	private Serafina player;

	private Node2D table;

	private Json jsonLoader;

	[Signal]
	public delegate void FoodObtainedEventHandler(Godot.Collections.Array FoodToMerge);

	[Signal]
	public delegate void PlaceDishEventHandler(Sprite2D DishToPlace);

	[Signal]
	public delegate void PickRandomFoodEventHandler(Food newFood);

	public Godot.Collections.Dictionary FoodDict = new Godot.Collections.Dictionary();

	public RandomNumberGenerator Random = new RandomNumberGenerator();

	private string spriteFilePath = "res://Assets/items/";


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FoodObtained += MergeFood;
		PlaceDish += OnPlaceDish;
		PickRandomFood += OnPickRandomFood;

		jsonLoader = new Json();
		var dataFile = File.ReadAllText("Data/FoodData.json");
		jsonLoader.Parse(dataFile);
		Recipes = (Godot.Collections.Dictionary)jsonLoader.Data;
		foodTree = new FoodTree(Recipes);

		var spriteFile = File.ReadAllText("Data/SpriteData.json");
		jsonLoader.Parse(spriteFile);
		GD.Print(jsonLoader.Data);
		Godot.Collections.Dictionary tempDict = (Godot.Collections.Dictionary)jsonLoader.Data;
		foreach (string line in tempDict.Keys) {
			GD.Print("Loading: ", line);
			Texture2D newTexture = (Texture2D)GD.Load(spriteFilePath + tempDict[line]);
			Sprites[line] = newTexture;
		}

		GD.Print("Sprites now: ", Sprites);

		player = (Serafina)GetNode("Serafina");
		table = (Node2D)GetNode("Table");
		dishes = (Node2D)GetNode("Dishes");
		//GD.Print("I got: " + player.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	// Check if any ingredients can be merged
	private void MergeFood(Godot.Collections.Array FoodToMerge) {
		string recipe = foodTree.findRecipe(FoodToMerge);
		GD.Print("Recipe found: ", recipe);
		// Did not find a recipe and hit max number of ingredients
		if (recipe == null) {
			recipe = "slop";
		}
		player.EmitSignal(Serafina.SignalName.DishMerged, recipe, Sprites[recipe]);
	}

	private void OnPlaceDish(Sprite2D dish) {
		GD.Print("Placing dish");
		int posX = (int)dish.GlobalPosition.X;
		CharacterBody2D newNode = (CharacterBody2D)PlacedDish.Instantiate();
		Sprite2D vis = (Sprite2D)newNode.GetNode("Sprite2D");
		vis.Texture = dish.Texture;
		newNode.Position = new Godot.Vector2(posX, table.Position.Y);
		GetNode("Dishes").AddChild(newNode);
		newNode.Velocity = new Godot.Vector2(-200, 0);
	}

	private void OnPickRandomFood(Food newFood) {
		Random.Randomize();
		Godot.Collections.Array keyArray = (Godot.Collections.Array)Sprites.Keys;
		int randomKey = (int)Random.RandiRange(0, keyArray.Count - 1);
		string randomSprite = (string)keyArray[randomKey];
		Texture2D randomTexture = (Texture2D)Sprites[randomSprite];
		Sprite2D newFoodVis = (Sprite2D)newFood.GetNode("Visual");
		newFoodVis.Texture = randomTexture;
		newFood.name = randomSprite;
	}
}


public class FoodNode {
	public string ingredient;
	public string recipe;

	public List<FoodNode> children;

	public FoodNode(string ingredientName) {
		ingredient = ingredientName;
		children = new List<FoodNode>();
	}

	public void setRecipe(string recipeName) {
		recipe = recipeName;
	}
}


public class FoodTree {
	public FoodNode root = new FoodNode(null);

	public FoodTree(Godot.Collections.Dictionary Recipes) {
		foreach (string recipe in Recipes.Keys) {
			string recipeName = recipe;
			Godot.Collections.Array recipeToCheck = (Godot.Collections.Array)Recipes[recipe];
			recipeToCheck.Sort();
			//GD.Print("Adding ing: ", recipeToCheck, " recipe: ", recipeName);
			insert(root, recipeToCheck, recipeName, 0);
		}
	}

	public void insert(FoodNode root, Godot.Collections.Array foods, string recipeName, int start) {
		if (start >= foods.Count) {
			GD.Print("Setting recipe");
			root.recipe = recipeName;
			return;
		}
		string foodToAdd = (string)foods[start];
		foreach (FoodNode child in root.children){
			if (child.ingredient == foodToAdd) {
				//GD.Print("Ingredient already exists in children");
				insert(child, foods, recipeName, start + 1);
				return;
			}
		}
		FoodNode newNode = new FoodNode(foodToAdd);
		root.children.Add(newNode);
		insert(newNode, foods, recipeName, start + 1);
		return;
	}

	public string findRecipe(Godot.Collections.Array ingredients) {
		ingredients.Sort();
		return traverseTree(root, ingredients, 0);
	}

	public string traverseTree(FoodNode root, Godot.Collections.Array ingredients, int start) {
		//GD.Print("Traversing");
		if (root.recipe != null) {
			//GD.Print("Found recipe");
			return root.recipe;
		}
		foreach (FoodNode child in root.children) {
			//GD.Print("At child: :", child.ingredient);
			if (child.ingredient == (string)ingredients[start]) {
				//GD.Print("Found a match");
				return traverseTree(child, ingredients, start + 1); //<-- recursive
			}
		}
		//GD.Print("Recipe not found");
		return null;
	}
}