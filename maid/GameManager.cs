using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export]
	public PackedScene GameWorld { get; set; }

	[Export]
	public PackedScene GameOver { get; set; }

	CanvasLayer Credits;

	CanvasLayer MainMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MainMenu = (CanvasLayer)GetNode("MainMenu");
		Credits = (CanvasLayer)GetNode("Credits");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnStartButtonPressed() {
		//GD.Print("Changing scene");
		GetTree().ChangeSceneToFile("res://Scenes/GameWorld.tscn");
	}

	private void _OnModesPressed() {

	}

	private void _OnCreditsPressed() {
		//GD.Print("Credits pressed");
		MainMenu.Hide();
		Credits.Show();
	}

	private void _OnBackCreditsButtonPressed() {
		Credits.Hide();
		MainMenu.Show();
	}
}
