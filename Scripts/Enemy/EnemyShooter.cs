using Godot;
namespace Game.Enemy;

/// <summary>
/// Enemigo Shooter: permanece vivo al contacto y dispara al jugador mientras lo detecta.
/// </summary>
/// <remarks>
/// Dispara un proyectil cada <see cref="FireInterval"/> segundos mientras el estado
/// de ataque esté activo. El primer disparo ocurre al cumplirse el intervalo.
/// </remarks>
public partial class EnemyShooter : EnemyController
{
	/// <summary>Escena del proyectil enemigo que se instancia al disparar.</summary>
	[Export] public PackedScene BulletScene { get; set; }

	/// <summary>Segundos entre disparos; por defecto 1.5 según el GDD.</summary>
	[Export] public float FireInterval { get; set; } = 1.5f;

	/// <summary>El Shooter no se destruye al tocar al jugador.</summary>
	protected override bool DestroyOnPlayerContact => false;

	/// <summary>Distancia que intenta mantener con el jugador, en píxeles.</summary>
	[Export] public float PreferredDistance { get; set; } = 150f;

	/// <summary>Margen alrededor de la distancia preferida donde el Shooter se detiene.</summary>
	[Export] public float DistanceTolerance { get; set; } = 20f;

	/// <summary>Calcula el movimiento de retirada o acercamiento según la distancia.</summary>
	/// <param name="target">Jugador detectado.</param>
	/// <returns>Dirección de alejamiento, acercamiento o cero si está en rango.</returns>
	public override Vector2 GetMovementDirection(Node2D target)
	{
		float distance = GlobalPosition.DistanceTo(target.GlobalPosition);

		if (distance < PreferredDistance - DistanceTolerance)
			return target.GlobalPosition.DirectionTo(GlobalPosition);

		if (distance > PreferredDistance + DistanceTolerance)
			return GlobalPosition.DirectionTo(target.GlobalPosition);

		return Vector2.Zero;
	}

	/// <summary>Temporizador que marca la cadencia de disparo.</summary>
	private Timer _attackTimer;

	/// <summary>Punto de salida del proyectil.</summary>
	private Marker2D _muzzle;

	/// <inheritdoc/>
	public override void _Ready()
	{
		base._Ready();

		_muzzle = GetNode<Marker2D>("Marker2D");

		_attackTimer = new Timer
		{
			WaitTime = Mathf.Max(0.1f, FireInterval),
			OneShot = false,
			Autostart = false
		};
		AddChild(_attackTimer);
		_attackTimer.Timeout += OnAttackTimerTimeout;
	}

	/// <summary>Inicia la cadencia de disparo al entrar en el estado de ataque.</summary>
	public override void OnAttackEnter()
	{
		_attackTimer.Start();
	}

	/// <summary>Detiene la cadencia cuando el jugador sale del área.</summary>
	public override void OnAttackExit()
	{
		_attackTimer.Stop();
	}

	/// <summary>Dispara al objetivo actual si sigue dentro del área.</summary>
	private void OnAttackTimerTimeout()
	{
		if (HealthComponent.IsDead || !Detection.HasTarget)
		{
			_attackTimer.Stop();
			return;
		}

		FireAt(Detection.Target);
	}

	/// <summary>Instancia una bala enemiga dirigida al objetivo.</summary>
	/// <param name="target">Jugador detectado.</param>
	private void FireAt(Node2D target)
	{
		if (BulletScene is null)
		{
			GD.PushWarning("EnemyShooter: no hay BulletScene configurada.");
			return;
		}

		Vector2 direction = _muzzle.GlobalPosition.DirectionTo(target.GlobalPosition);
		if (direction == Vector2.Zero)
			return;

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		GetParent().AddChild(bullet);
		bullet.GlobalPosition = _muzzle.GlobalPosition;
		bullet.Direction = direction.Normalized();
	}
}
