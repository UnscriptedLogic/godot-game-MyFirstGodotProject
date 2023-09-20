using Godot;
using System;

public partial class Frog : CharacterBody2D
{
    public const float JumpVelocity = -400.0f;

    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D animatedSprite;

    private Node2D player;

    public override void _Ready()
    {
        animatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        velocity.Y += gravity * (float)delta;
        Velocity = velocity;

        if (player != null)
        {
            Vector2 direction = (player.Position - Position).Normalized();

            if (direction.X > 0)
            {
                //Right
                animatedSprite.FlipH = true;
            }
            else
            {
                //Left
                animatedSprite.FlipH = false;
            }
        }

        MoveAndSlide();
    }

    public void Jump()
    {
        if (IsOnFloor())
        {
            velocity.Y = JumpVelocity;

        }
    }

    public void OnBodyEntered(Node2D node)
    {
        if (node.Name == "Player")
        {
            player = (Node2D)GetNode("../../Player/Player");
        }
    }

    public void OnBodyExit(Node2D node)
    {
        player = null;
    }
}
