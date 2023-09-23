using System;
using Godot;

public partial class main : Node2D
{
    public override void _Ready()
	{
		Utils.LoadGame();
	}

	public void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	public void OnPlayButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/world.tscn");
	}
}