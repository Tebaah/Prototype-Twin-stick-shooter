using System;
using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de reposo: el enemigo permanece inmóvil hasta detectar al jugador.
/// </summary>
public partial class IdleState : EnemyState
{
	/// <inheritdoc/>
	public override void Enter()
	{
		Body.Velocity = Vector2.Zero;
	}

	/// <summary>Al detectar al jugador dentro del área, pasa a perseguirlo.</summary>
	/// <param name="target">Cuerpo que entró en el área de detección.</param>
	public void OnBodyEntered(Node2D target)
	{
		if (target.IsInGroup("Player"))
		{
			Machine.TransitionTo("MoveState");
		}
	}
}
