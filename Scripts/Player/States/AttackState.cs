using System;
using Godot;

public partial class AttackState : PlayerState
{

    [Export] PackedScene bulletScene;
    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
        // intanciar una bala
        // la posicion debe ser la del markerd
        // debe tener una direccion (hacia el mouse)
        // debe tener una velocidad (la bala)

        Area2D bullet = bulletScene.Instantiate() as Area2D;
        bullet.Position = Body.Marker2D.GlobalPosition;
        GetParent().AddChild(bullet);

        Machine.TransitionTo("ChargeState");

    }
}
