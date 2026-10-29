using Game.Enemy;
using Godot;

/// <summary>
/// Proyectil que avanza en línea recta y resuelve su impacto mediante un
/// <see cref="HitboxComponent"/>.
/// </summary>
/// <remarks>
/// La bala se libera al salir de la pantalla, mediante la señal <c>screen_exited</c>
/// de <see cref="VisibleOnScreenNotifier2D"/>, y al impactar a su objetivo. El tipo
/// de proyectil decide si daña a un enemigo o al jugador.
/// </remarks>
public partial class Bullet : Area2D
{
	/// <summary>Vector de avance; las balas del jugador lo fijan apuntando al ratón.</summary>
	public Vector2 Direction { get; set; } = Vector2.Right;

	/// <summary>Velocidad de avance en píxeles/segundo.</summary>
	[Export]
	private float MovementSpeed { get; set; } = 350;

	/// <summary>Tipo de bala; determina el objetivo al que puede dañar.</summary>
	[Export] public HitboxType BulletType { get; set; } = HitboxType.PlayerBullet;

	/// <summary>
	/// Indica si la bala debe apuntar al ratón al entrar en escena.
	/// </summary>
	/// <remarks>
	/// Las balas enemigas lo desactivan y reciben su <see cref="Direction"/> del
	/// disparador antes de añadirse al árbol.
	/// </remarks>
	[Export] public bool AimAtMouse { get; set; } = true;

	/// <summary>Hitbox encargado de detectar el objetivo.</summary>
	private HitboxComponent _hitbox;

	/// <summary>Evita aplicar daño más de una vez si el área entrega varios eventos.</summary>
	private bool _hasHit;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_hitbox = GetNode<HitboxComponent>("HitboxComponent");
		_hitbox.Configure(BulletType);
		_hitbox.Hit += OnHit;

		Direction = AimAtMouse
			? GlobalPosition.DirectionTo(GetGlobalMousePosition())
			: Direction.Normalized();
	}

	/// <summary>Desplaza la bala cada fotograma en su <see cref="Direction"/>.</summary>
	/// <param name="delta">Tiempo transcurrido desde el fotograma anterior, en segundos.</param>
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += Direction * MovementSpeed * (float)delta;
	}

	/// <summary>Resuelve el impacto contra el objetivo correspondiente y libera la bala.</summary>
	/// <param name="other">Hitbox que entró en el área de la bala.</param>
	private void OnHit(HitboxComponent other)
	{
		if (_hasHit)
			return;

		if (_hitbox.Type == HitboxType.PlayerBullet
			&& other.Type == HitboxType.Enemy
			&& other.Entity is EnemyRusher enemy)
		{
			_hasHit = true;
			enemy.TakeDamage(_hitbox.Damage);
			QueueFree();
		}
		else if (_hitbox.Type == HitboxType.EnemyBullet
			&& other.Type == HitboxType.Player
			&& other.Entity is PlayerController player)
		{
			_hasHit = true;
			player.TakeDamage(_hitbox.Damage);
			QueueFree();
		}
	}

	/// <summary>
	/// Libera la bala cuando su notificador sale de la pantalla.
	/// Invocado por la señal <c>screen_exited</c> conectada desde la escena de la bala.
	/// </summary>
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
