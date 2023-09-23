using Godot;
using System;

public partial class Coin_Label : Label
{
    public override void _Process(double delta)
    {
        Text = $"Gold: {GameManager.Gold}";
    }
}