using Godot;
using System;

public partial class Food : RigidBody2D
{

	public string name;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnArea2DEntered(Area2D area) {
		if (area.Name == "Plate") {
			//GD.Print("Hit sera");
			Sprite2D vis = (Sprite2D)GetNode("Visual");
			Serafina parent = (Serafina)area.GetParent();
			parent.EmitSignal(Serafina.SignalName.FoodObtained, name, vis.Texture);
			QueueFree();
		}
	}

	private void _OnVisibleOnScreenNotifier2DScreenExited() {
		//GD.Print("Left screen");
		QueueFree();
	}
}
