using Godot;
using System;

public partial class GameManager : Node2D
{

	[Signal]
	public delegate void GetChallengeEventHandler(string name);

	[Signal]
	public delegate void BeginGameEventHandler();

	[Signal]
	public delegate void EndGameEventHandler(int won);

	[Signal]
	public delegate void RestartGameEventHandler();

	[Signal]
	public delegate void ExitGameEventHandler();

	[Signal]
	public delegate void ContinueEventHandler();

	[Signal]
	public delegate void ReturnToMenuEventHandler();

	[Export]
	public PackedScene GameWorld { get; set; }

	private LevelManager WorldInst;

	[Export]
	public PackedScene GameOver { get; set; }

	private GameOver GamInst;

	[Export]
	public PackedScene Dialogue { get; set; }

	private DialogueHUD DiaInst;

	CanvasLayer Credits;

	CanvasLayer MainMenu;

	CanvasLayer Challenges;

	AudioStreamPlayer BGMusic;

	AnimationPlayer Fade;

	Node2D Game;


	[Export]
	string tgtChallenge = "ChallengeDoll";

	string tgtScreen = "MainMenu";


	// Stores order of challenges
	public Godot.Collections.Array Progress;

	public void initProgressDict()
	{
		Progress = new Godot.Collections.Array
			{
				"ChallengeSera",
				"ChallengeLetti",
				"ChallengeDoll",
				"ChallengeGob",
				"ChallengeAnnieAlex"
			};
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MainMenu = (CanvasLayer)GetNode("MainMenu");
		Credits = (CanvasLayer)GetNode("Credits");
		Challenges = (CanvasLayer)GetNode("Challenges");
		BGMusic = (AudioStreamPlayer)GetNode("BGMusic");
		Fade = (AnimationPlayer)GetNode("AnimationPlayer");
		Game = (Node2D)GetNode("Game");

		GetChallenge += OnGetChallenge;
		BeginGame += OnBeginGame;
		EndGame += OnEndGame;
		RestartGame += OnRestartGame;
		ExitGame += _OnBackGameOverButtonPressed;
		Continue += OnContinueGame;
		ReturnToMenu += OnReturn;

		BGMusic.Play(1f);
		initProgressDict();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnStartButtonPressed() {
		tgtScreen = "Game";
		Fade.Play("FadeIn");
	}

	private void _OnModesPressed() {
		tgtScreen = "Modes";
		Fade.Play("FadeIn");
	}

	private void _OnCreditsPressed() {
		tgtScreen = "Credits";
		Fade.Play("FadeIn");
		//GD.Print("Credits pressed");
	}

	private void _OnExitPressed() {
		GetTree().Quit();
	}

	private void _OnBackCreditsButtonPressed() {
		tgtScreen = "MainMenu";
		Fade.Play("FadeIn");
	}

	private void _OnBackGameOverButtonPressed() {
		tgtScreen = "Restart";
		Fade.Play("FadeIn");
	}

	private void OnGetChallenge(string name) {
		tgtScreen = "Challenge";
		tgtChallenge = name;
		Fade.Play("FadeIn");
	}

	private void OnBeginGame() {
		WorldInst = (LevelManager)GameWorld.Instantiate();
		WorldInst.OnSetScenario(tgtChallenge);
		AddChild(WorldInst);
		Game.GetNode("DialogueHUD").QueueFree();
	}

	private void OnEndGame(int won) {
		GetNode("GameWorld").QueueFree();
		GamInst = (GameOver)GameOver.Instantiate();
		// Set dialogue before adding
		GamInst.OnGetScenario(tgtChallenge, won);
		Game.AddChild(GamInst);
	}

	private void OnRestartGame() {
		Game.GetNode("GameOver").QueueFree();
		WorldInst = (LevelManager)GameWorld.Instantiate();
		WorldInst.OnSetScenario(tgtChallenge);
		AddChild(WorldInst);
	}

	private void OnContinueGame() {
		tgtScreen = "Continue";
		int chalInd = Progress.IndexOf(tgtChallenge) + 1;
		if (chalInd < Progress.Count) {
			tgtChallenge = (string)Progress[chalInd];
		} else {
			CompleteGame();
		}
		Fade.Play("FadeIn");
	}

	private void OnReturn() {
		tgtScreen = "Return";
		int chalInd = Progress.IndexOf(tgtChallenge) + 1;
		if (chalInd < Progress.Count) {
			tgtChallenge = (string)Progress[chalInd];
		} else {
			CompleteGame();
		}
		Fade.Play("FadeIn");
	}

	private void CompleteGame() {
		GD.Print("Reached end of scenario");
	}

	private void _OnAnimationPlayerAnimationFinished(StringName animName) {
		switch(animName) {
			case "FadeIn":
				switch(tgtScreen) 
				{
				case "Game":
					MainMenu.Hide();
					DiaInst = (DialogueHUD)Dialogue.Instantiate();
					DiaInst.OnSetScenario(tgtChallenge);
					Game.AddChild(DiaInst);
					break;
				case "Modes":
					MainMenu.Hide();
					Challenges.Show();
					break;
				case "Credits":
					MainMenu.Hide();
					Credits.Show();
					break;
				case "Challenge":
					Challenges.Hide();
					if (tgtChallenge != "ChallengeSphene") {
						DiaInst = (DialogueHUD)Dialogue.Instantiate();
						DiaInst.OnSetScenario(tgtChallenge);
						Game.AddChild(DiaInst);
					} else {
						// Endless mode
						WorldInst = (LevelManager)GameWorld.Instantiate();
						WorldInst.OnSetScenario(tgtChallenge);
						AddChild(WorldInst);
					}
					break;
				case "Restart":
					Game.GetNode("GameOver").QueueFree();
					MainMenu.Show();
					break;
				case "Continue":
					Game.GetNode("GameOver").QueueFree();
					DiaInst = (DialogueHUD)Dialogue.Instantiate();
					DiaInst.OnSetScenario(tgtChallenge);
					Game.AddChild(DiaInst);
					break;
				case "Return":
					GetNode("GameWorld").QueueFree();
					MainMenu.Show();
					break;
				default:
					Credits.Hide();
					Challenges.Hide();
					MainMenu.Show();
					break;
				}
				Fade.Play("FadeOut");
				break;
		}
	}
}
