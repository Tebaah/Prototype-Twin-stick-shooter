using System;
using Godot;

public partial class ChargeState : PlayerState
{
    public override void FrameUpdate(double delta)
    {
        base.FrameUpdate(delta);
        // si presiono el boton de ataque cambia al estado de ataque

        if (Input.IsActionJustPressed("ui_attack"))
        {
            Machine.TransitionTo("AttackState");
        }
    }
}
