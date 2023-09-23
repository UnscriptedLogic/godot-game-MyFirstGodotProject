using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private float Speed = 200.0f;
	[Export] private float JumpVelocity = -350.0f;

	//Components
	private CollisionShape2D collider;
	private CollisionShape2D hitBoxCollider;

	private AnimationPlayer animator;
	private AnimatedSprite2D animatedSprite;
	
	private Vector2 direction;

	private Timer coyoteTimer;
	private bool wasOnFloor;

    private Timer jumpForgiveTimer;
    private bool isWaitingForJump;

	private bool canMove;
	private bool canAnimate;

	public Signal onPlayerHitSignal;

    Vector2 velocity;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
		animator = (AnimationPlayer)GetNode("AnimationPlayer");
		animatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		hitBoxCollider = GetNode<Area2D>("HitBox").GetNode<CollisionShape2D>("CollisionShape2D");
		
		coyoteTimer = (Timer)GetNode("CoyoteTimer");
        jumpForgiveTimer = (Timer)GetNode("JumpForgiveTimer");

		animator.Play("Idle");
		canMove = true;
		canAnimate = true;

        Enemy.OnEnemyKilled += Enemy_OnEnemyKilled;
    }

    private void Enemy_OnEnemyKilled(object sender, int onKillGold)
    {
		GameManager.Gold += onKillGold;
    }

	public void OnHitBoxEntered(Node2D node)
	{
		Enemy enemy = node as Enemy;

		if (enemy != null)
		{
            GameManager.PlayerHP--;
			
            if (GameManager.PlayerHP <= 0)
            {
				collider.SetDeferred("disabled", true);
				hitBoxCollider.SetDeferred("disabled", true);

				canMove = false;
				canAnimate = false;

				animator.Play("Death");

				Velocity = new Vector2(0f, JumpVelocity);
				Speed = 0f;
            }
			else
			{
				canMove = false;
                hitBoxCollider.SetDeferred("disabled", true);

                Velocity = Position.DirectionTo(enemy.Position) * -300f;

                GetTree().CreateTimer(.5f).Timeout += () =>
                {
                    canMove = true;
                    hitBoxCollider.SetDeferred("disabled", false);
                };
			}
        }
	}

    //FixedUpdate
    public override void _PhysicsProcess(double delta)
	{
		velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept"))
		{
            if (!canMove) return;

            if (IsOnFloor() || !coyoteTimer.IsStopped())
			{
                velocity.Y = JumpVelocity;
            }
			else
			{
				isWaitingForJump = true;
				jumpForgiveTimer.Start();
			}
        }

		if (isWaitingForJump || !jumpForgiveTimer.IsStopped())
		{
			if (jumpForgiveTimer.IsStopped())
			{
				isWaitingForJump = false;
			}
			else
			{
                if (IsOnFloor())
                {
                    isWaitingForJump = false;
                    velocity.Y = JumpVelocity;
                }
            }
        }

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		if (canMove)
		{
            direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }
        }

		wasOnFloor = IsOnFloor();

        Velocity = velocity;
		MoveAndSlide();

		AnimateCharacter();

		if (wasOnFloor && !IsOnFloor())
		{
			coyoteTimer.Start();
		}
	}

	private void AnimateCharacter()
	{
		if (!canAnimate) return;

		if (direction.X >= 1)
		{
            animatedSprite.FlipH = false;
        }
        else if (direction.X <= -1)
		{
			animatedSprite.FlipH = true;
		}

		if (Velocity.Y > 0)
		{
			animator.Play("Fall");
			return;
		}
		else if (Velocity.Y < 0)
		{
			animator.Play("Jump");
			return;
		}

		if (direction != Vector2.Zero)
		{
			animator.Play("Run");
		}
		else
		{
			animator.Play("Idle");
		}
	}

    public override void _ExitTree()
    {
		Enemy.OnEnemyKilled -= Enemy_OnEnemyKilled;
    }
}
