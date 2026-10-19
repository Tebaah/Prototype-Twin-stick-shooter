using Godot;
namespace Game.Enemy.States;

/// <summary>
/// Estado de ataque del Rusher: mientras el jugador siga dentro del área de
/// detección, el enemigo comprueba si su cuerpo ha llegado a tocarlo; al impactar
/// le descuenta una vida y muere (el Rusher cae de un solo golpe), sumando su
/// puntaje al marcador global.
/// </summary>
/// <remarks>
/// Este estado no desplaza al enemigo: inspecciona las colisiones registradas por
/// <see cref="CharacterBody2D.MoveAndSlide"/>, que ejecuta <c>MoveState</c> en la
/// máquina de movimiento del mismo cuerpo. Por eso el impacto solo se detecta
/// mientras la persecución está activa, que es justo la condición para atacar.
/// </remarks>
public partial class AttackState : EnemyState
{
	/// <summary>Vidas que descuenta el contacto de un Rusher (GDD 2.2: -1 vida).</summary>
	private const int ContactDamage = 1;

	/// <summary>
	/// Revisa cada fotograma físico los cuerpos con los que el enemigo se topó al
	/// moverse: el primero que sea el jugador pierde <see cref="ContactDamage"/> vidas
	/// y el enemigo se libera.
	/// </summary>
	/// <param name="delta">Tiempo transcurrido desde el fotograma anterior, en segundos.</param>
	public override void PhysicsUpdate(double delta)
	{
		base.PhysicsUpdate(delta);

		for (int i = 0; i < Body.GetSlideCollisionCount(); i++)
		{
			if (Body.GetSlideCollision(i).GetCollider() is not PlayerController player)
				continue;

			player.TakeDamage(ContactDamage);
			Body.Die();
			// El cuerpo queda marcado para liberarse al final del fotograma:
			// no hay nada más que comprobar.
			return;
		}
	}

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
