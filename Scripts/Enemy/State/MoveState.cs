using System;
using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de persecución: mientras el jugador permanezca dentro del área de
/// detección, el enemigo avanza hacia él recalculando la dirección en cada
/// fotograma físico, de modo que no pierde al objetivo si este se mueve.
/// </summary>
public partial class MoveState : EnemyState
{
	/// <summary>Jugador perseguido, capturado al entrar en el área de detección.</summary>
	private Node2D _target;

	/// <summary>Registra al jugador como objetivo en cuanto entra al área de detección.</summary>
	/// <param name="target">Cuerpo que entró en el área.</param>
	public void OnBodyEntered(Node2D target)
	{
		if (target.IsInGroup("Player"))
			_target = target;
	}

	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		// Si el objetivo ya no es válido (null o liberado), no hay nada que perseguir.
		if (!IsInstanceValid(_target)) return;

		Body.Direction = Body.GlobalPosition.DirectionTo(_target.GlobalPosition);
		Body.Velocity = Body.Direction * Body.Speed;
		Body.MoveAndSlide();
	}

	/// <summary>Al salir el jugador del área, olvida el objetivo y vuelve a reposo.</summary>
	/// <param name="target">Cuerpo que salió del área.</param>
	public void OnBodyExited(Node2D target)
	{
		if (target == _target)
			_target = null;

		Machine.TransitionTo("IdleState");
	}
}
