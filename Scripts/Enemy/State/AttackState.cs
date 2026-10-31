namespace Game.Enemy.States;

/// <summary>
/// Estado de ataque mientras el jugador permanece en el área de detección.
/// </summary>
/// <remarks>
/// El daño por contacto lo resuelve <see cref="HitboxComponent"/> y
/// <see cref="EnemyRusher"/>. Si el jugador sale del área, vuelve a
/// <c>LookingForTargetState</c>.
/// </remarks>
public partial class AttackState : EnemyState
{
	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		if (!Body.Detection.HasTarget)
			Machine.TransitionTo("LookingForTargetState");
	}
}
