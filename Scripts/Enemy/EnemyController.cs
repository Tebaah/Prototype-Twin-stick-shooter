using Godot;
namespace Game.Enemy;

/// <summary>
/// Base común de los enemigos: salud, detección, contacto, puntaje y muerte.
/// </summary>
/// <remarks>
/// Las subclases definen el comportamiento propio, como el disparo del Shooter.
/// Los estados pueden notificar la entrada y salida del ataque mediante
/// <see cref="OnAttackEnter"/> y <see cref="OnAttackExit"/>.
/// La salud la administra <see cref="HealthComponent"/> y la presencia del jugador
/// <see cref="DetectionComponent"/>.
/// </remarks>
public abstract partial class EnemyController : CharacterBody2D
{
	/// <summary>Velocidad de desplazamiento en píxeles/segundo.</summary>
	[Export] public float Speed { get; set; } = 300.0f;

	/// <summary>Puntaje mínimo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MinScore { get; set; } = 10;

	/// <summary>Puntaje máximo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MaxScore { get; set; } = 50;

	/// <summary>Vidas que descuenta el contacto con el jugador.</summary>
	[Export] public int ContactDamage { get; set; } = 1;

	/// <summary>
	/// Indica si el enemigo se destruye al tocar al jugador.
	/// </summary>
	/// <remarks>
	/// El Rusher muere al contacto; el Shooter permanece vivo y sigue atacando.
	/// </remarks>
	protected virtual bool DestroyOnPlayerContact => true;

	/// <summary>Dirección unitaria de desplazamiento, recalculada por el estado activo.</summary>
	public Vector2 Direction { get; set; } = Vector2.Zero;

	/// <summary>Componente que administra la salud del enemigo.</summary>
	public HealthComponent HealthComponent { get; private set; }

	/// <summary>Componente que detecta al jugador dentro del área de persecución.</summary>
	public DetectionComponent Detection { get; private set; }

	/// <summary>Vida restante del enemigo.</summary>
	public int Health => HealthComponent.CurrentHealth;

	/// <summary>Vida máxima actual del enemigo.</summary>
	public int MaxHealth => HealthComponent.MaxHealth;

	/// <summary>Puntaje aleatorio asignado a esta instancia al entrar en escena.</summary>
	public int ScoreValue { get; private set; }

	/// <summary>Evita otorgar puntaje o resolver el contacto más de una vez.</summary>
	private bool _isDead;

	/// <summary>Hitbox que detecta el contacto con el jugador.</summary>
	private HitboxComponent _hitbox;

	/// <inheritdoc/>
	public override void _Ready()
	{
		HealthComponent = GetNode<HealthComponent>("HealthComponent");
		HealthComponent.Died += OnHealthDied;

		Detection = GetNode<DetectionComponent>("DetectionComponent");

		ScoreValue = GD.RandRange(MinScore, MaxScore);
		GameManager.Instance?.RegisterEnemy(this);

		_hitbox = GetNode<HitboxComponent>("HitboxComponent");
		_hitbox.Hit += OnHitboxHit;
	}

	/// <inheritdoc/>
	public override void _ExitTree()
	{
		if (HealthComponent is not null)
			HealthComponent.Died -= OnHealthDied;

		GameManager.Instance?.UnregisterEnemy(this);
	}

	/// <summary>Se invoca al entrar en el estado de ataque.</summary>
	/// <remarks>Las subclases pueden iniciar aquí su ataque específico.</remarks>
	public virtual void OnAttackEnter()
	{
	}

	/// <summary>Se invoca al salir del estado de ataque.</summary>
	/// <remarks>Las subclases pueden detener aquí su ataque específico.</remarks>
	public virtual void OnAttackExit()
	{
	}

	/// <summary>Descuenta vida al enemigo a través de su <see cref="HealthComponent"/>.</summary>
	/// <param name="damage">Daño recibido; los valores no positivos se ignoran.</param>
	public void TakeDamage(int damage)
	{
		if (_isDead)
			return;

		HealthComponent.TakeDamage(damage);
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

	/// <summary>Resuelve la muerte cuando el componente de salud llega a cero.</summary>
	private void OnHealthDied() => Die();

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
