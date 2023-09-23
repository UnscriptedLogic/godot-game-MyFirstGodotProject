using Godot;
using System;

public partial class HP_Label : Label
{
    public override void _Process(double delta)
    {
        Text = $"HP: {GameManager.PlayerHP}";
    }
}
