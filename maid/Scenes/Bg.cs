using Godot;
using System;

public partial class Bg : TextureRect
{
	[Export]
	public Texture2D ChallengeSera;

	[Export]
	public Texture2D ChallengeLetti;

	[Export]
	public Texture2D ChallengeDoll;

	[Export]
	public Texture2D ChallengeShadow;

	[Export]
	public Texture2D ChallengeAnnieAlex;

	[Signal]
	public delegate void SetBGEventHandler(string tgt);

	// Stores string name of texture for bg loading
	public Godot.Collections.Dictionary BGDict;

	// Initializes BG dict that stores texture data with corresponding string name
	public void initBGDict()
	{
		BGDict = new Godot.Collections.Dictionary
			{
				{"ChallengeSera", ChallengeSera},
				{"ChallengeLetti", ChallengeLetti},
				{"ChallengeDoll", ChallengeDoll},
				{"ChallengeShadow", ChallengeShadow},
				{"ChallengeAnnieAlex", ChallengeAnnieAlex}
			};
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		initBGDict();
		SetBG += OnSetBG;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Changes BG to tgt scenario
	private void OnSetBG(string tgt) {
		Texture = (Texture2D)BGDict[tgt];
	}
}
