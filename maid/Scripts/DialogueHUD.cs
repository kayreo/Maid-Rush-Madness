using Godot;
using System;
using System.IO;
using System.Linq;
public partial class DialogueHUD : CanvasLayer
{
	private TextureRect DialogueBox;

	private Json jsonLoader;

	private Godot.Collections.Dictionary DialogueScenarios;

	private Godot.Collections.Dictionary DialogueData;

	private RichTextLabel Dialogue;

	protected GameManager GameManager;

	private AnimatedSprite2D Speaker;

	private TextureRect BG;

	private string CurrentScenario;

	private int VisibleCharacters = 0;

	[Export]
	public string CurSpeaker = "0";

	[Export]
	public string Scenario = "None";

	private bool InProgress = false;

	private int CurrentLineIndex = 0;

	public bool DialogueStarted = true;

	[Signal]
	public delegate void EndFirstDialogueEventHandler();

	[Signal]
	public delegate void EndDialogueEventHandler();

	[Signal]
	public delegate void TriggerDialogueEventHandler(string DialogueText);

	public override void _Ready()
	{
		/*GameManager = GetNode<GameManager>("/root/GameWorld");*/
		EndDialogue += OnEndDialogue;/*
		EndFirstDialogue += GameManager.OnFirstEndDialogue;*/
		//DialogueBox = GetNode<TextureRect>("DialogueBox");

		jsonLoader = new Json();
		var file = File.ReadAllText("Data/DialogueData.json");
		jsonLoader.Parse(file);
		DialogueScenarios = (Godot.Collections.Dictionary)jsonLoader.Data;

		GD.Print("Scenarios: ", DialogueScenarios);

		TriggerDialogue += OnDialogueTrigger;
		Dialogue = GetNode<RichTextLabel>("Dialogue/DialogueText");
		Speaker = GetNode<AnimatedSprite2D>("Control/Speaker");
		BG = GetNode<TextureRect>("BG");
		GD.Print("Speaker got: ", Speaker.Name);
		EmitSignal("TriggerDialogue", Scenario);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (DialogueStarted) {
			//Printing out a line
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
			//Move to next line
			else {
				if (Input.IsActionJustReleased("click")) {
					ContinueDialogue();
				}
			}
		} else {
			Dialogue.Hide();
			DialogueStarted = false;
			CurrentLineIndex = 0;
			Speaker.Frame = 0;
		}
	}

	// Triggers to begin to dialogue and to continue dialogue
	// Loads secnario to be printed
	// DialogueLine parameter is the group of lines to grab from the JSON file
	private void OnDialogueTrigger(string Scenario) {
		//GD.Print("Received signal", Scenario);
		DialogueData = (Godot.Collections.Dictionary)DialogueScenarios[Scenario];
		CurrentScenario = Scenario;
		BG.EmitSignal("SetBG", CurrentScenario);
		//BoardManager.DialogueActive = true;
		ContinueDialogue();
	}

	private void ContinueDialogue() {
		if (DialogueData.ContainsKey(CurrentLineIndex.ToString())) {
			String[] DialogueLine = (String[])DialogueData[CurrentLineIndex.ToString()];
			////GD.Print("Dialogue: ", DialogueLine[0]);
			string Who = (string)DialogueLine[0];
			int Frame = DialogueLine[1].ToInt();
			//Speaker0.Animation = "player" + Who0[0];
			if (Who.Contains("none")) {
				Speaker.Hide();
			} else {
				Speaker.Animation = Who;
				Speaker.Frame = Frame;
				//Speaker.Frame = (int)Who[1] - '0';
			}

			Dialogue.Text = DialogueLine[2];
			Dialogue.VisibleCharacters = 0;
			InProgress = true;			
			CurrentLineIndex++;
			// //GD.Print("Visible: ", DialogueBox.VisibleCharacters);
			// //GD.Print("Total: ", DialogueBox.Text.Length);
		} else {
			// key not found, stop dialogue
			//BoardManager.DialogueActive = false;
			if (CurrentScenario.Contains("Start")) {

			} else {
				EmitSignal("EndDialogue");
			}
		}
	}

	// X: 0: Neutral, 1: Shocked, 2: Sad, 3: Happy
	// Y: 0: Alchemist, 1: Demon, 2: Fox
	private void ChangePortrait(Vector2I WhichPortrait) {
		//GD.Print("Chef");
		//GetNode<TileMap>("PortraitControl/Portrait").SetCell(0, new Vector2I(0,0), 0, WhichPortrait);
	}


	private void OnEndDialogue() {
		Hide();
	}

}
