using Godot;
using System;

public partial class BG : ParallaxBackground
{
	public float scrollingSpeed = 100f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 scrollOffset = ScrollOffset;
        scrollOffset.X -= scrollingSpeed * (float)delta;
        ScrollOffset = scrollOffset;
	}
}
