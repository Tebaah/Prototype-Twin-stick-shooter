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

    /// <summary>Vida inicial del jugador (GDD 2.1: 3 puntos).</summary>
    public const int MaxHealth = GameManager.MaxLives;

    /// <summary>Vida restante del jugador, entre 0 y <see cref="MaxHealth"/>.</summary>
    public int Health { get; private set; } = MaxHealth;

    /// <summary>Se emite cuando cambia la vida, con el valor actual.</summary>
    public event Action<int> HealthChanged;

    /// <summary>Se emite una sola vez cuando la vida llega a 0.</summary>
    public event Action Died;

    #endregion

    #region Godot Methods

    public override void _Ready()
    {
        base._Ready();
        Marker2D = GetNode<Marker2D>("Marker2D");
        GameManager.Instance?.RegisterPlayer(this);
    }

    public override void _ExitTree()
    {
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
    /// Descuenta vida al jugador y notifica a los interesados.
    /// </summary>
    /// <param name="damage">Puntos de vida a descontar; los valores no positivos se ignoran.</param>
    /// <remarks>
    /// La vida nunca baja de 0 y <see cref="Died"/> se emite una sola vez, aunque
    /// el jugador reciba daño en fotogramas posteriores.
    /// </remarks>
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || Health <= 0)
            return;

        Health = Math.Max(0, Health - damage);
        HealthChanged?.Invoke(Health);

        if (Health == 0)
            Died?.Invoke();
    }

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
