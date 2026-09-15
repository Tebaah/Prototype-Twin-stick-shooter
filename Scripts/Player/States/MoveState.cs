using System;
using Godot;

public partial class MoveState : PlayerState
{
    /// <summary>Porcentaje de <see cref="PlayerController.MaxSpeed"/> mientras el jugador se mueve.</summary>
    [Export] private float _speedMultiplier = 1f;

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        MoveWithInput(_speedMultiplier);
    }

    public override void FrameUpdate(double delta)
    {
        base.FrameUpdate(delta);

        bool isMoving = Input.IsActionPressed("ui_left")
                     || Input.IsActionPressed("ui_right")
                     || Input.IsActionPressed("ui_up")
                     || Input.IsActionPressed("ui_down");

        if (!isMoving)
        {
            Machine.TransitionTo("IdleState");
        }
    }
}
