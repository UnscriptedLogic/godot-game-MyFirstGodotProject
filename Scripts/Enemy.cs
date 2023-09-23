using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] private float Speed = 100f;
    [Export] private int OnKillGold;

    protected bool isAlive;

    protected float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    protected AnimatedSprite2D animatedSprite;
    protected Timer chaseTimer;

    protected Vector2 prevPosition;
    protected CharacterBody2D player;

    public static event EventHandler<int> OnEnemyKilled;

    public override void _Ready()
    {
        isAlive = true;

        animatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
        chaseTimer = (Timer)GetNode("ChaseTimer");

        animatedSprite.Play("Idle");

        prevPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isAlive) return;

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

    protected virtual void Animate()
    {
        if ((prevPosition - Position).Length() > 0)
        {
            animatedSprite.Play("Jump");
        }
        else
        {
            animatedSprite.Play("Idle");
        }

        prevPosition = Position;
    }

    public virtual void OnBodyEntered(Node2D node)
    {
        if (node.Name == "Player")
        {
            player = (CharacterBody2D)GetNode("../../Player/Player");
        }
    }

    public virtual void OnBodyExit(Node2D node)
    {
        player = null;
    }

    public virtual void OnKillBoxEntered(Node2D node)
    {
        if (node.Name != "Player") return;
        isAlive = false;

        //Boost player
        CharacterBody2D playerBody = node as CharacterBody2D;
        playerBody.Velocity = new Vector2(0, -400f);

        //Play death animation
        animatedSprite.Play("Death");
        animatedSprite.AnimationFinished += QueueFreeObject;

        //GameManager.Gold += 10;
    }

    protected virtual void QueueFreeObject()
    {
        animatedSprite.AnimationFinished -= QueueFreeObject;

        OnEnemyKilled?.Invoke(this, OnKillGold);

        QueueFree();
    }
}
