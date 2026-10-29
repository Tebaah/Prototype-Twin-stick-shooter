using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de ataque del enemigo mientras el jugador permanece en su área de detección.
/// </summary>
/// <remarks>
/// El daño por contacto no se resuelve aquí: lo detecta <see cref="HitboxComponent"/>
/// y lo aplica <see cref="EnemyRusher"/>. Este estado solo mantiene el ciclo de ataque
/// y vuelve a buscar objetivo cuando el jugador sale del área.
/// </remarks>
public partial class AttackState : EnemyState
{
	/// <summary>
	/// Termina el ataque cuando el jugador abandona el área de detección, de modo
	/// que el enemigo deja de perseguirlo hasta volver a detectarlo.
	/// </summary>
	/// <param name="target">Cuerpo que salió del área de detección.</param>
	public void OnBodyExited(Node2D target)
	{
		if (target.IsInGroup("Player"))
			Machine.TransitionTo("LookingForTargetState");
	}
}
