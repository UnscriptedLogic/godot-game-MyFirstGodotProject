using System;
using Godot;

public partial class main : Node2D
{
	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
		
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