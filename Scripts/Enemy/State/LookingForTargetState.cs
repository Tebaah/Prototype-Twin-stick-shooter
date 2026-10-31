namespace Game.Enemy.States;

/// <summary>
/// Estado de espera entre perder el objetivo y volver a detectarlo.
/// </summary>
/// <remarks>
/// Si <see cref="DetectionComponent"/> vuelve a detectar al jugador, pasa a
/// <c>AttackState</c>.
/// </remarks>
public partial class LookingForTargetState : EnemyState
{
	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		if (Body.Detection.HasTarget)
			Machine.TransitionTo("AttackState");
	}
}
