using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de reposo: el enemigo permanece inmóvil hasta detectar al jugador.
/// </summary>
/// <remarks>
/// Consulta <see cref="DetectionComponent.HasTarget"/> y pasa a <c>MoveState</c>
/// cuando el jugador entra en el área.
/// </remarks>
public partial class IdleState : EnemyState
{
	/// <inheritdoc/>
	public override void Enter()
	{
		Body.Velocity = Vector2.Zero;
	}

	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		if (Body.Detection.HasTarget)
			Machine.TransitionTo("MoveState");
	}
}
