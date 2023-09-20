using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private AnimationPlayer animator;
	private AnimatedSprite2D animatedSprite;
	
	private Vector2 direction;

	private Timer coyoteTimer;
	private bool wasOnFloor;

    private Timer jumpForgiveTimer;
    private bool isWaitingForJump;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
		animator = (AnimationPlayer)GetNode("AnimationPlayer");
		animatedSprite = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
		
		coyoteTimer = (Timer)GetNode("CoyoteTimer");
        jumpForgiveTimer = (Timer)GetNode("JumpForgiveTimer");

		animator.Play("Idle");
    }

    //FixedUpdate
    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept"))
		{
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
        direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
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
}
