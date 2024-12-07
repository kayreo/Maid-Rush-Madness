using Godot;
using System;

public partial class GameManager : Node2D
{

	[Signal]
	public delegate void GetChallengeEventHandler(string name);

	[Export]
	public PackedScene GameWorld { get; set; }

	private LevelManager WorldInst;

	[Export]
	public PackedScene GameOver { get; set; }

	CanvasLayer Credits;

	CanvasLayer MainMenu;

	CanvasLayer Challenges;

	AudioStreamPlayer BGMusic;

	string tgtChallenge = "ChallengeSera";


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		MainMenu = (CanvasLayer)GetNode("MainMenu");
		Credits = (CanvasLayer)GetNode("Credits");
		Challenges = (CanvasLayer)GetNode("Challenges");
		BGMusic = (AudioStreamPlayer)GetNode("BGMusic");

		GetChallenge += OnGetChallenge;

		WorldInst = (LevelManager)GameWorld.Instantiate();

		BGMusic.Play(1f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnStartButtonPressed() {
		//GD.Print("Changing scene");
		MainMenu.Hide();
		WorldInst.OnSetScenario(tgtChallenge);
		AddChild(WorldInst);
		//GetTree().ChangeSceneToFile("res://Scenes/GameWorld.tscn");
	}

	private void _OnModesPressed() {
		MainMenu.Hide();
		Challenges.Show();
	}

	private void _OnCreditsPressed() {
		//GD.Print("Credits pressed");
		MainMenu.Hide();
		Credits.Show();
	}

	private void _OnBackCreditsButtonPressed() {
		Credits.Hide();
		Challenges.Hide();
		MainMenu.Show();
	}

	private void OnGetChallenge(string name) {
		tgtChallenge = name;
		Challenges.Hide();
		_OnStartButtonPressed();
	}
}
