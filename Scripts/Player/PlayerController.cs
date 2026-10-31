using System;
using Godot;

public partial class PlayerController : CharacterBody2D
{
    #region Variables and Properties

    /// <summary>Velocidad base de movimiento en píxeles/segundo (GDD: 200 px/s).</summary>
    [Export] public float MaxSpeed { get; set; } = 200f;

    /// <summary>Dirección de entrada normalizada que aporta el estado activo.</summary>
    public Vector2 DirectionOfMovement { get; set; }
    public Marker2D Marker2D { get; set; }

    /// <summary>Componente que administra la salud del jugador.</summary>
    public HealthComponent HealthComponent { get; private set; }

    /// <summary>Vida restante del jugador, entre 0 y <see cref="MaxHealth"/>.</summary>
    public int Health => HealthComponent.CurrentHealth;

    /// <summary>Vida máxima actual del jugador.</summary>
    public int MaxHealth => HealthComponent.MaxHealth;

    /// <summary>Se emite cuando cambia la vida, con (vida actual, vida máxima).</summary>
    public event Action<int, int> HealthChanged;

    /// <summary>Se emite una sola vez cuando la vida llega a 0.</summary>
    public event Action Died;

    #endregion

    #region Godot Methods

    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Player");
        Marker2D = GetNode<Marker2D>("Marker2D");

        HealthComponent = GetNode<HealthComponent>("HealthComponent");
        HealthComponent.HealthChanged += OnHealthChanged;
        HealthComponent.Died += OnDied;

        GameManager.Instance?.RegisterPlayer(this);
    }

    public override void _ExitTree()
    {
        if (HealthComponent is not null)
        {
            HealthComponent.HealthChanged -= OnHealthChanged;
            HealthComponent.Died -= OnDied;
        }

        GameManager.Instance?.UnregisterPlayer(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        RotateToMouse();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Descuenta vida al jugador a través de su <see cref="HealthComponent"/>.
    /// </summary>
    /// <param name="damage">Puntos de vida a descontar; los valores no positivos se ignoran.</param>
    public void TakeDamage(int damage) => HealthComponent.TakeDamage(damage);

    /// <summary>
    /// Reenvía el cambio de salud con la firma que consume el <see cref="GameManager"/>.
    /// </summary>
    /// <param name="current">Vida actual tras el cambio.</param>
    /// <param name="max">Vida máxima actual.</param>
    private void OnHealthChanged(int current, int max) => HealthChanged?.Invoke(current, max);

    /// <summary>Reenvía la muerte del componente de salud.</summary>
    private void OnDied() => Died?.Invoke();

    /// <summary>
    /// Orienta al jugador hacia el cursor del ratón.
    /// <see cref="Node2D.LookAt"/> alinea el eje +X local con el cursor, pero el frente del
    /// personaje y el marcador de puntería (Marker2D, en (0, -17) local) apuntan hacia arriba
    /// (eje -Y local); de ahí la corrección constante de 90° sobre <see cref="Node2D.RotationDegrees"/>.
    /// </summary>
    public void RotateToMouse()
    {
        LookAt(GetGlobalMousePosition());
        RotationDegrees += 90f;
    }

    #endregion
}
