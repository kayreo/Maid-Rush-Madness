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
using System.Security.Principal;


public partial class LevelManager : Node2D
{

	[Export]
	public PackedScene PlacedDish { get; set; }

	[Export]
	public AudioStream OrderDing { get; set; }

	[Export]
	public AudioStream ItemGet { get; set; }

	[Export]
	public AudioStream ItemPlace { get; set; }

	[Export]
	public AudioStream TimerUp { get; set; }

	private FoodTree foodTree;

	private Node2D dishes;

	private Godot.Collections.Dictionary Recipes;

	private Godot.Collections.Dictionary Sprites = new Godot.Collections.Dictionary();

	private Serafina player;

	private Node2D table;

	public Control requestUI;

	public Timer orderTimer;

	public Timer appearTimer;

	private Node2D Timer;

	private TextureProgressBar TimerFill;

	private Sprite2D TimerPoint;

	public TextureRect BG;

	public CanvasLayer PauseScreen;

	public Godot.Collections.Array curRecipes = new Godot.Collections.Array();

	private Json jsonLoader;

	[Signal]
	public delegate void FoodObtainedEventHandler(Godot.Collections.Array FoodToMerge);

	[Signal]
	public delegate void SetScenarioEventHandler(string name);

	[Signal]
	public delegate void PlaceDishEventHandler(Sprite2D DishToPlace, string DishName);

	[Signal]
	public delegate void PickRandomFoodEventHandler(Food newFood);

	[Signal]
	public delegate void GameOverEventHandler();

	[Signal]
	public delegate void UpdateIngredientsEventHandler(string food);

	public Godot.Collections.Dictionary FoodDict = new Godot.Collections.Dictionary();

	public Godot.Collections.Dictionary ChallengeDict = new Godot.Collections.Dictionary();

	public string CurChallenge = "";

	public Godot.Collections.Array CurRecipes;

	public Godot.Collections.Array CurIngredients = new Godot.Collections.Array();

	public Godot.Collections.Array RemainingIngredients = new Godot.Collections.Array();

	public Godot.Collections.Array RecipeKeys;

	public RandomNumberGenerator Random = new RandomNumberGenerator();

	public AudioStreamPlayer SFX;

	public AudioStreamPlayer TimerSFX;

	public bool paused = false;

	private string spriteFilePath = "res://Assets/items/";

	public string tgtRecipe;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FoodObtained += MergeFood;
		PlaceDish += OnPlaceDish;
		PickRandomFood += OnPickRandomFood;
		GameOver += EndGame;
		UpdateIngredients += UpdateRemainingIngredients;

		// Load food data
		jsonLoader = new Json();
		var dataFile = File.ReadAllText("Data/FoodData.json");
		jsonLoader.Parse(dataFile);
		Recipes = (Godot.Collections.Dictionary)jsonLoader.Data;
		RecipeKeys = (Godot.Collections.Array)Recipes.Keys;
		foodTree = new FoodTree(Recipes);

		// Load visual data
		var spriteFile = File.ReadAllText("Data/SpriteData.json");
		jsonLoader.Parse(spriteFile);
		//GD.Print(jsonLoader.Data);
		Godot.Collections.Dictionary tempDict = (Godot.Collections.Dictionary)jsonLoader.Data;
		foreach (string line in tempDict.Keys) {
			//GD.Print("Loading: ", line);
			Texture2D newTexture = (Texture2D)GD.Load(spriteFilePath + tempDict[line]);
			Sprites[line] = newTexture;
		}

		//GD.Print("Sprites now: ", Sprites);
		
		// Load challenge data
		var challengeFile = File.ReadAllText("Data/ChallengeData.json");
		jsonLoader.Parse(challengeFile);

		ChallengeDict = (Godot.Collections.Dictionary)jsonLoader.Data;
		player = (Serafina)GetNode("Serafina");
		table = (Node2D)GetNode("Table");
		dishes = (Node2D)GetNode("Dishes");
		requestUI = (Control)GetNode("RequestWindow");
		orderTimer = (Timer)GetNode("OrderTimer");
		appearTimer = (Timer)GetNode("AppearTimer");	
		BG = (TextureRect)GetNode("BG");
		Timer = (Node2D)GetNode("Timer");
		TimerFill = (TextureProgressBar)GetNode("Timer/TimerFill");
		TimerPoint = (Sprite2D)GetNode("Timer/TimerPoint");
		PauseScreen = (CanvasLayer)GetNode("PauseScreen");
		SFX = (AudioStreamPlayer)GetNode("SFX");
		TimerSFX = (AudioStreamPlayer)GetNode("TimerSFX");

		Label recipeLabel = (Label)requestUI.GetNode("Display/VBoxContainer/RecipeLabel");

		// Pick a challenge
		BG.EmitSignal("SetBG", CurChallenge);
		//CurChallenge = "ChallengeSera";
		// Get recipes for that challenge
		CurRecipes = (Godot.Collections.Array)ChallengeDict[CurChallenge];

		// Set time limit
		switch (CurChallenge) {
			case "ChallengeSera":
				appearTimer.WaitTime = 5;
				orderTimer.WaitTime = 36;
				TimerFill.MaxValue = 36;
				break;
			case "ChallengeLetti":
				orderTimer.WaitTime = 32;
				TimerFill.MaxValue = 32;
				break;
			case "ChallengeAnnieAlex":
				orderTimer.WaitTime = 28;
				TimerFill.MaxValue = 28;
				break;
			case "ChallengeGob":
				orderTimer.WaitTime = 24;
				TimerFill.MaxValue = 24;
				break;
			case "ChallengeDoll":
				appearTimer.WaitTime = 1;
				orderTimer.WaitTime = 16;
				TimerFill.MaxValue = 16;
				break;
			case "ChallengeSphene":
				orderTimer.WaitTime = 20;
				TimerFill.MaxValue = 20;
				break;
		}
		GD.Print("Recipes for ", CurChallenge, ": ", CurRecipes);
		OnPickRandomDish();
		orderTimer.Start();
		PopulateRandomFoodChoices();
		//GD.Print("I got: " + player.Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!paused) {
			appearTimer.Paused = false;
			orderTimer.Paused = false;
			Label timeLeftLabel = (Label)GetNode("TimerDisplay/HBoxContainer/TimeLeft");
			timeLeftLabel.Text = String.Format("{0:0}", orderTimer.TimeLeft);
			TimerFill.Value = orderTimer.WaitTime - orderTimer.TimeLeft;
			TimerPoint.RotationDegrees = (360.0f * (float)(TimerFill.Value / TimerFill.MaxValue));
		} else {
			appearTimer.Paused = true;
			orderTimer.Paused = true;
		}
		
		// Make timer tick when time almost up
		if (TimerFill.Value / TimerFill.MaxValue >= .75 && !TimerSFX.Playing) {
			TimerSFX.Play();
		} else if (TimerFill.Value / TimerFill.MaxValue <= 0.0 && TimerSFX.Playing) {
			TimerSFX.Stop();
		}
		if (Input.IsActionJustPressed("pause"))
		{
			if (PauseScreen.Visible) {
				paused = false;
				PauseScreen.Hide();
			}
			else {
				paused = true;
				PauseScreen.Show();
			}
		}
	}

	// Check if any ingredients can be merged
	private void MergeFood(Godot.Collections.Array FoodToMerge) {
		string recipe = foodTree.findRecipe(FoodToMerge);
		//GD.Print("Recipe found: ", recipe);
		// Did not find a recipe and hit max number of ingredients
		if (recipe == null) {
			recipe = "Slop";
		}
		player.EmitSignal(Serafina.SignalName.DishMerged, recipe, Sprites[recipe]);
	}

	private void UpdateRemainingIngredients(string food) {
		if (RemainingIngredients.Contains(food)) {
			RemainingIngredients.Remove(food);
		}
	}

	private void OnPlaceDish(Sprite2D dish, string dishName) {
		SFX.Stream = ItemPlace;
		SFX.Play();
		//GD.Print("Placing dish");
		int posX = (int)dish.GlobalPosition.X;
		CharacterBody2D newNode = (CharacterBody2D)PlacedDish.Instantiate();
		Sprite2D vis = (Sprite2D)newNode.GetNode("Sprite2D");
		vis.Texture = dish.Texture;
		vis.Scale = new Godot.Vector2(0.5f, 0.5f);
		newNode.Position = new Godot.Vector2(posX, table.Position.Y - 150);
		GetNode("Dishes").AddChild(newNode);
		newNode.Velocity = new Godot.Vector2(0, 400);
		if (dishName == tgtRecipe) {
			GD.Print("Correct Dish!");
			orderTimer.Stop();
			orderTimer.Start();
			if (CurChallenge != "ChallengeSphene") {
				// If not endless mode, remove recipe from pool
				CurRecipes.Remove(dishName);
			}
			GD.Print("Recipes now: ", CurRecipes);
			if (CurRecipes.Count <= 0) {
				GD.Print("No more recipes, game complete");
				GetParent().EmitSignal("EndGame", 1);
			} else {
				OnPickRandomDish();
			}
		} else {
			GD.Print("Incorrect dish");
			RemainingIngredients.Clear();
			Godot.Collections.Array recipeIngredients = (Godot.Collections.Array)Recipes[tgtRecipe];
			for (int i = 0; i < 3; i++) {
				RemainingIngredients.Add(recipeIngredients[i]);
			}
		}
	}

	private void OnPickRandomFood(Food newFood) {
		Random.Randomize();
		// Weight to spawn desired ingredients
		int tgtWeight = 5;
		int randomWeight = (int)Random.RandiRange(0, 10);
		// Set default
		int randomKey = (int)Random.RandiRange(0, CurIngredients.Count - 1);
		string randomSprite = (string)CurIngredients[randomKey];
		//GD.Print("Picking random: ", randomSprite);
		// See if weight changes info
		if (RemainingIngredients.Count > 0 && randomWeight <= tgtWeight) {
			randomKey = (int)Random.RandiRange(0, RemainingIngredients.Count - 1);
			randomSprite = (string)RemainingIngredients[randomKey];
		}
		// Set vis
		Texture2D randomTexture = (Texture2D)Sprites[randomSprite];
		Sprite2D newFoodVis = (Sprite2D)newFood.GetNode("Visual");
		newFoodVis.Texture = randomTexture;
		newFood.name = randomSprite;
	}

	private void PopulateRandomFoodChoices() {
		foreach (string recipe in RecipeKeys) {
			Godot.Collections.Array toLoadRecipe = (Godot.Collections.Array)Recipes[recipe];
			foreach (string ingredient in toLoadRecipe) {
				if (!CurIngredients.Contains(ingredient)) {
					CurIngredients.Add(ingredient);
				}
			}
		}
		//GD.Print("Populated: ", CurIngredients);
	}

	private void OnPickRandomDish() {
		RemainingIngredients.Clear();
		Random.Randomize();
		//GD.Print("Keys: ", RecipeKeys);
		int randomKey = (int)Random.RandiRange(0, CurRecipes.Count - 1);
		string randomDish = (string)CurRecipes[randomKey];
		//GD.Print("I got the dish: ", randomDish);
		Godot.Collections.Array recipeIngredients = (Godot.Collections.Array)Recipes[randomDish];
	
		Label recipeLabel = (Label)requestUI.GetNode("Display/VBoxContainer/RecipeLabel");
		recipeLabel.Text = randomDish;
		HBoxContainer ingredientsList = (HBoxContainer)requestUI.GetNode("Display/VBoxContainer/Ingredients");
		for (int i = 0; i < ingredientsList.GetChildCount(); i++) {
			TextureRect castedI = (TextureRect)ingredientsList.GetChild(i);
			Texture2D textI = (Texture2D)Sprites[recipeIngredients[i]];
			castedI.Texture = textI;
			RemainingIngredients.Add(recipeIngredients[i]);
		}
		tgtRecipe = randomDish;
		player.EmitSignal(Serafina.SignalName.ChangeDisplayOrder);
		ShowRequest();
	}

	private void _OnOrderTimeout() {
		//GD.Print("Order timed out, decreasing health and reshowing current order");
		ShowRequest();
		player.EmitSignal(Serafina.SignalName.TakeDamage);
	}
	private void _OnAppearTimeout() {
		//GD.Print("New order disappeared");
		CanvasLayer display = (CanvasLayer)requestUI.GetNode("Display");
		display.Hide();
		player.EmitSignal(Serafina.SignalName.ChangeDisplayHealth);
	}

	private void ShowRequest() {
		CanvasLayer display = (CanvasLayer)requestUI.GetNode("Display");
		SFX.Stream = OrderDing;
		SFX.Play();
		display.Show();
		appearTimer.Start();
	}

	private void EndGame() {
		GD.Print("Ending game");
		GetParent().EmitSignal("EndGame", 0);
	}

	public void OnSetScenario(string name) {
		GD.Print("Tgt challenge is now: ", name);
		CurChallenge = name;
	}

	private void _OnPauseButtonPressed() {
		GetParent().EmitSignal("PlayClick");
		paused = true;
		PauseScreen.Show();
	}

	private void _OnContinueButtonPressed() {
		GetParent().EmitSignal("PlayClick");
		paused = false;
		PauseScreen.Hide();
	}

	private void _OnExitButtonPressed() {
		GetParent().EmitSignal("PlayClick");
		GetParent().EmitSignal("ReturnToMenu");
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
			//GD.Print("Setting recipe");
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
		if (start >= ingredients.Count) {
			if (root.recipe != null) {
				//GD.Print("Found recipe");
				return root.recipe;
			}
			return null;
		}
		foreach (FoodNode child in root.children) {
			//GD.Print("At child: :", child.ingredient);
			if (child.ingredient == (string)ingredients[start]) {
				//GD.Print("Found a match");
				return traverseTree(child, ingredients, start + 1); //<-- recursive
			}
		}
		GD.Print("Recipe not found");
		return null;
	}
}