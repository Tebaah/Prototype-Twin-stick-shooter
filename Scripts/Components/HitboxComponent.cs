using Godot;
using System;

/// <summary>
/// Clasifica la entidad que posee un <see cref="HitboxComponent"/>.
/// </summary>
public enum HitboxType
{
	/// <summary>Jugador; actúa como objetivo pasivo.</summary>
	Player,

	/// <summary>Enemigo; detecta el contacto con el jugador.</summary>
	Enemy,

	/// <summary>Proyectil disparado por el jugador; daña enemigos.</summary>
	PlayerBullet,

	/// <summary>Proyectil disparado por un enemigo; daña al jugador.</summary>
	EnemyBullet
}

/// <summary>
/// Área de colisión reutilizable que identifica a su entidad y notifica impactos.
/// </summary>
/// <remarks>
/// Las capas se configuran de forma unilateral para que cada pareja se detecte una
/// sola vez: el jugador es solo detectable, los enemigos detectan al jugador y las
/// balas detectan a su objetivo. El propietario decide cómo reaccionar a
/// <see cref="Hit"/>; el componente no aplica daño por sí mismo.
/// La escena del componente debe incluir un <see cref="CollisionShape2D"/> con una
/// <see cref="CircleShape2D"/>; su radio se ajusta por instancia.
/// </remarks>
public partial class HitboxComponent : Area2D
{
	/// <summary>Capa del hitbox del jugador (capa 3).</summary>
	private const uint PlayerLayer = 1u << 2;

	/// <summary>Capa del hitbox de los enemigos (capa 4).</summary>
	private const uint EnemyLayer = 1u << 3;

	/// <summary>Capa de los hitboxes de balas del jugador (capa 5).</summary>
	private const uint PlayerBulletLayer = 1u << 4;

	/// <summary>Capa de los hitboxes de balas enemigas (capa 6).</summary>
	private const uint EnemyBulletLayer = 1u << 5;

	/// <summary>Tipo de entidad que posee este hitbox.</summary>
	[Export] public HitboxType Type { get; set; } = HitboxType.Player;

	/// <summary>Radio del hitbox en píxeles; debe ser mayor que cero.</summary>
	[Export] public float Radius { get; set; } = 12f;

	/// <summary>Daño que aplica el propietario cuando este hitbox impacta.</summary>
	[Export] public int Damage { get; set; } = 1;

	/// <summary>Entidad dueña del hitbox, normalmente el nodo padre.</summary>
	public Node2D Entity { get; private set; }

	/// <summary>Se emite cuando otro hitbox entra en este, con el hitbox detectado.</summary>
	public event Action<HitboxComponent> Hit;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Entity = GetParent() as Node2D;
		if (Entity is null)
			GD.PushWarning($"HitboxComponent ({Name}): el padre no es un Node2D.");

		ConfigureShape();
		ApplyCollisionConfiguration();
		AreaEntered += OnAreaEntered;
	}

	/// <summary>
	/// Cambia el tipo de hitbox en tiempo de ejecución y reconfigura sus capas.
	/// </summary>
	/// <param name="type">Nuevo tipo de entidad.</param>
	/// <remarks>
	/// Se usa en proyectiles reutilizables que comparten escena para jugador y enemigos.
	/// </remarks>
	public void Configure(HitboxType type)
	{
		Type = type;

		if (IsInsideTree())
			ApplyCollisionConfiguration();
	}

	/// <summary>Duplica la forma de la escena y ajusta su radio para esta instancia.</summary>
	/// <remarks>
	/// El <see cref="CollisionShape2D"/> es obligatorio en la escena del componente.
	/// Se duplica su <see cref="CircleShape2D"/> para que el radio de un hitbox no
	/// modifique el de las demás instancias que comparten la misma escena.
	/// </remarks>
	private void ConfigureShape()
	{
		CollisionShape2D collision = GetNode<CollisionShape2D>("CollisionShape2D");
		if (collision.Shape is not CircleShape2D circle)
		{
			GD.PushError("HitboxComponent: se requiere un CollisionShape2D con CircleShape2D.");
			return;
		}

		circle = (CircleShape2D)circle.Duplicate();
		circle.Radius = Mathf.Max(1f, Radius);
		collision.Shape = circle;
	}

	/// <summary>Asigna capas y máscaras según el tipo para que cada impacto tenga un único detector.</summary>
	private void ApplyCollisionConfiguration()
	{
		CollisionLayer = 0;
		CollisionMask = 0;
		Monitoring = false;
		Monitorable = false;

		switch (Type)
		{
			case HitboxType.Player:
				CollisionLayer = PlayerLayer;
				Monitorable = true;
				break;

			case HitboxType.Enemy:
				CollisionLayer = EnemyLayer;
				CollisionMask = PlayerLayer;
				Monitoring = true;
				Monitorable = true;
				break;

			case HitboxType.PlayerBullet:
				CollisionLayer = PlayerBulletLayer;
				CollisionMask = EnemyLayer;
				Monitoring = true;
				break;

			case HitboxType.EnemyBullet:
				CollisionLayer = EnemyBulletLayer;
				CollisionMask = PlayerLayer;
				Monitoring = true;
				break;
		}
	}

	/// <summary>Notifica al propietario cuando otro hitbox entra en el área.</summary>
	/// <param name="area">Área que entró; se ignora si no es un hitbox.</param>
	private void OnAreaEntered(Area2D area)
	{
		if (area is not HitboxComponent other || other == this)
			return;

		Hit?.Invoke(other);
	}
}
