using System;
using Godot;

public partial class IdleState : PlayerState
{
    public override void Enter()
    {
        base.Enter();
        // El jugador detenido no se desplaza: descarta la velocidad heredada del estado anterior.
        Body.Velocity = Vector2.Zero;
    }
    public override void FrameUpdate(double delta)
    {
        bool isMoving = Input.IsActionJustPressed("ui_left")
                     || Input.IsActionJustPressed("ui_right")
                     || Input.IsActionJustPressed("ui_up")
                     || Input.IsActionJustPressed("ui_down");

        if (isMoving)
        {
            Machine.TransitionTo("MoveState");
        }
    }
}
