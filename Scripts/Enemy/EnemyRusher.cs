using System;
using Godot;
namespace Game.Enemy;

/// <summary>
/// Enemigo de tipo "rusher": persigue al jugador mientras este permanece en su
/// área de detección. La decisión de perseguir o detenerse la gestiona
/// <see cref="EnemyStateMachine"/> a través de sus estados.
/// </summary>
/// <remarks>
/// El daño por contacto lo detecta su <see cref="HitboxComponent"/> y lo resuelve
/// este cuerpo: el jugador pierde vida y el enemigo puede destruirse según
/// <see cref="DestroyOnPlayerContact"/>. Los enemigos con más de una vida solo
/// mueren al agotar <see cref="Health"/> por impactos de bala.
/// </remarks>
public partial class EnemyRusher : CharacterBody2D
{
	/// <summary>Velocidad de persecución en píxeles/segundo.</summary>
	[Export] public float Speed { get; set; } = 300.0f;

	/// <summary>Puntaje mínimo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MinScore { get; set; } = 10;

	/// <summary>Puntaje máximo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MaxScore { get; set; } = 50;

	/// <summary>Vidas del enemigo; al llegar a cero se destruye y otorga puntaje.</summary>
	[Export] public int MaxHealth { get; set; } = 1;

	/// <summary>Vidas que descuenta el contacto con el jugador.</summary>
	[Export] public int ContactDamage { get; set; } = 1;

	/// <summary>
	/// Indica si el enemigo se destruye al tocar al jugador.
	/// </summary>
	/// <remarks>
	/// <c>true</c> para el Rusher (explota sin otorgar puntaje); <c>false</c> para
	/// el Shooter, que permanece vivo tras el contacto.
	/// </remarks>
	[Export] public bool DestroyOnPlayerContact { get; set; } = true;

	/// <summary>Dirección unitaria de persecución, recalculada por el estado activo.</summary>
	public Vector2 Direction { get; set; } = Vector2.Zero;

	/// <summary>Vida restante del enemigo.</summary>
	public int Health { get; private set; }

	/// <summary>Puntaje aleatorio asignado a esta instancia al entrar en escena.</summary>
	public int ScoreValue { get; private set; }

	/// <summary>Evita otorgar puntaje o resolver el contacto más de una vez.</summary>
	private bool _isDead;

	/// <summary>Hitbox que detecta el contacto con el jugador.</summary>
	private HitboxComponent _hitbox;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Health = Math.Max(1, MaxHealth);
		ScoreValue = GD.RandRange(MinScore, MaxScore);
		GameManager.Instance?.RegisterEnemy(this);

		_hitbox = GetNode<HitboxComponent>("HitboxComponent");
		_hitbox.Hit += OnHitboxHit;
	}

	/// <inheritdoc/>
	public override void _ExitTree()
	{
		GameManager.Instance?.UnregisterEnemy(this);
	}

	/// <summary>Descuenta vida al enemigo y lo destruye si llega a cero.</summary>
	/// <param name="damage">Daño recibido; los valores no positivos se ignoran.</param>
	public void TakeDamage(int damage)
	{
		if (_isDead || damage <= 0)
			return;

		Health = Math.Max(0, Health - damage);
		if (Health == 0)
			Die();
	}

	/// <summary>
	/// Elimina al enemigo y, opcionalmente, suma su puntaje al marcador global.
	/// </summary>
	/// <param name="awardScore">
	/// <c>true</c> si la muerte la provocó el jugador; <c>false</c> si fue por contacto.
	/// </param>
	/// <remarks>
	/// Es idempotente: aunque varias balas o estados lo invoquen en el mismo
	/// fotograma, solo la primera llamada libera el nodo y otorga puntos.
	/// </remarks>
	public void Die(bool awardScore = true)
	{
		if (_isDead)
			return;

		_isDead = true;
		if (awardScore)
			GameManager.Instance?.AddScore(ScoreValue);

		QueueFree();
	}

	/// <summary>Resuelve el contacto con el jugador cuando el hitbox lo detecta.</summary>
	/// <param name="other">Hitbox que entró en el área del enemigo.</param>
	private void OnHitboxHit(HitboxComponent other)
	{
		if (_isDead || other.Type != HitboxType.Player)
			return;

		if (other.Entity is not PlayerController player)
			return;

		player.TakeDamage(ContactDamage);

		if (DestroyOnPlayerContact)
			Die(awardScore: false);
	}
}
