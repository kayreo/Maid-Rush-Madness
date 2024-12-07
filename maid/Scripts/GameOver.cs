using Godot;
using System;

public partial class GameOver : Control
{
	[Signal]
	public delegate void GetScenarioEventHandler(string scenario, Boolean wonGame);

	private RichTextLabel Dialogue;	

	private AnimatedSprite2D Speaker;

	private bool InProgress = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Dialogue = (RichTextLabel)GetNode("DialogueMini/DialogueText");
		Speaker = (AnimatedSprite2D)GetNode("Control/Speaker");

		GetScenario += OnGetScenario;

		Dialogue.Text = "Well... that's a game over...See you next time....";
		Speaker.Animation = "Doll";
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

	private void _onTryAgainButtonPressed() {
		GD.Print("Changing scene");
		GetTree().ChangeSceneToFile("res://Scenes/GameWorld.tscn");
	}

	private void OnGetScenario(string scenario, Boolean wonGame) {

	}
}
