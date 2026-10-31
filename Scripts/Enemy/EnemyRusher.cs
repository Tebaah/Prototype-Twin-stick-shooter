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
/// <see cref="DestroyOnPlayerContact"/>. La salud la administra su
/// <see cref="HealthComponent"/>; al agotarse, el enemigo muere y otorga puntaje.
/// La presencia del jugador la administra su <see cref="DetectionComponent"/>;
/// cada estado consulta el objetivo para decidir su transición.
/// </remarks>
public partial class EnemyRusher : CharacterBody2D
{
	/// <summary>Velocidad de persecución en píxeles/segundo.</summary>
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
	/// <c>true</c> para el Rusher (explota sin otorgar puntaje); <c>false</c> para
	/// el Shooter, que permanece vivo tras el contacto.
	/// </remarks>
	[Export] public bool DestroyOnPlayerContact { get; set; } = true;

	/// <summary>Dirección unitaria de persecución, recalculada por el estado activo.</summary>
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
