using Godot;
using System;

public partial class Frog : CharacterBody2D
{
    private const float JumpVelocity = -400.0f;
    private const float Speed = 100f;

    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D animatedSprite;
    private Timer chaseTimer;

    private Vector2 prevPosition;
    private Node2D player;

    public override void _Ready()
    {
        animatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        chaseTimer = (Timer)GetNode("ChaseTimer");

        animatedSprite.Play("Idle");
        
        prevPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        velocity.Y += gravity * (float)delta;

        if (player != null)
        {
            Vector2 direction = (player.Position - Position).Normalized();
            if (direction.X > 0)
            {
                //Right
                animatedSprite.FlipH = true;
                velocity.X = 1 * Speed;
            }
            else
            {
                //Left
                animatedSprite.FlipH = false;
                velocity.X = -1 * Speed;
            }

            chaseTimer.Start();
        }
        else
        {
            if (chaseTimer.IsStopped())
            {
                velocity.X = 0;
                chaseTimer.Stop();
            }
        }

        Velocity = velocity;
        MoveAndSlide();

        Animate();
    }

    private void Animate()
    {
        if ((prevPosition - Position).Length() > 0)
        {
            GD.Print("Moving");
            animatedSprite.Play("Jump");
        }
        else
        {
            animatedSprite.Play("Idle");
        }

        prevPosition = Position;
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
