using Godot;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

public partial class GameOver : CanvasLayer
{
	[Export]
	public AudioStream WonSFX;

	[Export]
	public AudioStream LostSFX;

	[Signal]
	public delegate void GetScenarioEventHandler(string scenario, int wonGame);

	private RichTextLabel Dialogue;	

	private AnimatedSprite2D Speaker;

	private AudioStreamPlayer SFX;

	private bool InProgress = true;

	private Json jsonLoader;

	private Godot.Collections.Dictionary DialogueScenarios;

	private Godot.Collections.Array DiaLine;

	int diaWonGame = 0;
	string diaScenario = "ChallengeSera";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Dialogue = (RichTextLabel)GetNode("DialogueMini/DialogueText");
		Speaker = (AnimatedSprite2D)GetNode("Control/Speaker");
		SFX = (AudioStreamPlayer)GetNode("SFX");

		GetScenario += OnGetScenario;

		jsonLoader = new Json();
		var file = File.ReadAllText("Data/GameOverData.json");
		jsonLoader.Parse(file);
		DialogueScenarios = (Godot.Collections.Dictionary)jsonLoader.Data;
		GD.Print("Data: ", DialogueScenarios);

		Godot.Collections.Dictionary Dia = (Godot.Collections.Dictionary)DialogueScenarios[diaScenario];
		DiaLine = (Godot.Collections.Array)Dia[diaWonGame.ToString()];
		Speaker.Animation = (string)DiaLine[0];
		Speaker.Frame = (int)DiaLine[1];
		Dialogue.Text = (string)DiaLine[2];

		if (diaWonGame != 1) {
			TextureButton ContinueButton = (TextureButton)GetNode("Buttons/Continue");
			ContinueButton.Hide();
			SFX.Stream = LostSFX;
		} else {
			SFX.Stream = WonSFX;
		}
		SFX.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (InProgress) {
			// scroll text
			if (Dialogue.VisibleCharacters < Dialogue.Text.Length) {
				Dialogue.VisibleCharacters += 1;
				if (Input.IsActionJustReleased("click")) {
					Dialogue.VisibleCharacters = -1;
					InProgress = false;
				}
			} 
			// text done scrolling, set visible chars to all
			else if (Dialogue.VisibleCharacters >= Dialogue.Text.Length) {
				Dialogue.VisibleCharacters = -1;
				InProgress = false;
			}
		}
	}

	private void _OnTryAgainButtonPressed() {
		GD.Print("Changing scene");
		GetParent().GetParent().EmitSignal("RestartGame");
	}

	private void _OnExitButtonPressed() {
		GetParent().GetParent().EmitSignal("ExitGame");
	}

	private void _OnContinueButtonPressed() {
		GetParent().GetParent().EmitSignal("Continue");
	}

	public void OnGetScenario(string scenario, int wonGame) {
		diaScenario = scenario;
		diaWonGame = wonGame;
	}
}
