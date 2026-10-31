using Godot;

/// <summary>
/// Detecta al jugador dentro de un área y expone el objetivo actual.
/// </summary>
/// <remarks>
/// Es un sensor: no persigue ni ataca. Mantiene <see cref="Target"/> y
/// <see cref="HasTarget"/> para que los estados del enemigo consulten la presencia
/// del jugador y decidan sus transiciones.
/// El cuerpo del jugador debe estar en la capa 1 y en <see cref="TargetGroup"/>.
/// La escena del componente debe incluir un <see cref="CollisionShape2D"/> con una
/// <see cref="CircleShape2D"/>; su radio se ajusta por instancia.
/// </remarks>
public partial class DetectionComponent : Area2D
{
	/// <summary>Capa del cuerpo del jugador (capa 1).</summary>
	private const uint PlayerBodyLayer = 1u << 0;

	/// <summary>Radio de detección en píxeles.</summary>
	[Export] public float Radius { get; set; } = 122f;

	/// <summary>Grupo al que debe pertenecer el cuerpo detectado.</summary>
	[Export] public string TargetGroup { get; set; } = "Player";

	/// <summary>Objetivo actual, o <c>null</c> si no hay nadie dentro del área.</summary>
	public Node2D Target { get; private set; }

	/// <summary>Indica si el objetivo actual sigue siendo válido.</summary>
	public bool HasTarget => Target is not null && IsInstanceValid(Target);

	/// <inheritdoc/>
	public override void _Ready()
	{
		ConfigureShape();
		ConfigureCollision();
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	/// <summary>Duplica la forma de la escena y ajusta su radio para esta instancia.</summary>
	private void ConfigureShape()
	{
		CollisionShape2D collision = GetNode<CollisionShape2D>("CollisionShape2D");
		if (collision.Shape is not CircleShape2D circle)
		{
			GD.PushError("DetectionComponent: se requiere un CollisionShape2D con CircleShape2D.");
			return;
		}

		circle = (CircleShape2D)circle.Duplicate();
		circle.Radius = Mathf.Max(1f, Radius);
		collision.Shape = circle;
	}

	/// <summary>Configura el área para detectar solo cuerpos y no ser detectada.</summary>
	private void ConfigureCollision()
	{
		CollisionLayer = 0;
		CollisionMask = PlayerBodyLayer;
		Monitoring = true;
		Monitorable = false;
	}

	/// <summary>Registra al jugador cuando entra en el área.</summary>
	/// <param name="body">Cuerpo que entró en el área.</param>
	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup(TargetGroup) || HasTarget)
			return;

		Target = body;
	}

	/// <summary>Olvida al objetivo cuando sale del área.</summary>
	/// <param name="body">Cuerpo que salió del área.</param>
	private void OnBodyExited(Node2D body)
	{
		if (body != Target)
			return;

		Target = null;
	}
}
