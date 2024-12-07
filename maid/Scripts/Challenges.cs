using Godot;
using System;

public partial class Challenges : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnLevel1Pressed() {
		GetParent().EmitSignal("GetChallenge", "ChallengeSera");
	}

	private void _OnLevel2Pressed() {
		GetParent().EmitSignal("GetChallenge", "ChallengeLetti");
	}

	private void _OnLevel3Pressed() {
		GetParent().EmitSignal("GetChallenge", "ChallengeGob");
	}

	private void _OnLevel4Pressed() {
		GetParent().EmitSignal("GetChallenge", "ChallengeDoll");
	}

	private void _OnLevel5Pressed() {
		GetParent().EmitSignal("GetChallenge", "ChallengeSphene");
	}
}
