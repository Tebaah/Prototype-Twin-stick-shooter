using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de persecución: mientras el jugador permanezca dentro del área de
/// detección, el enemigo avanza hacia él recalculando la dirección en cada
/// fotograma físico, de modo que no pierde al objetivo si este se mueve.
/// </summary>
/// <remarks>
/// Usa <see cref="DetectionComponent.Target"/> para perseguir y vuelve a
/// <c>IdleState</c> si el objetivo sale del área.
/// </remarks>
public partial class MoveState : EnemyState
{
	/// <inheritdoc/>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		if (!Body.Detection.HasTarget)
		{
			Machine.TransitionTo("IdleState");
			return;
		}

		Node2D target = Body.Detection.Target;
		Body.Direction = Body.GlobalPosition.DirectionTo(target.GlobalPosition);
		Body.Velocity = Body.Direction * Body.Speed;
		Body.MoveAndSlide();
	}
}