using System;
using Godot;

public partial class PlayerController : CharacterBody2D
{
    #region Variables and Properties

    /// <summary>Velocidad base de movimiento en píxeles/segundo (GDD: 200 px/s).</summary>
    [Export] public float MaxSpeed { get; set; } = 200f;

    /// <summary>Dirección de entrada normalizada que aporta el estado activo.</summary>
    public Vector2 DirectionOfMovement { get; set; }

    /// <summary>Pivote que gira alrededor del jugador para apuntar el muzzle.</summary>
    public Node2D MuzzlePivot { get; private set; }

    /// <summary>Punto de salida de los proyectiles, hijo del pivote.</summary>
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

        MuzzlePivot = GetNode<Node2D>("MuzzlePivot");
        Marker2D = GetNode<Marker2D>("MuzzlePivot/Marker2D");

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
        RotateMuzzleToMouse();
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
    /// Orienta solo el pivote del muzzle hacia el cursor; el cuerpo y el sprite del
    /// jugador permanecen en su orientación original.
    /// </summary>
    /// <remarks>
    /// <see cref="Node2D.LookAt"/> alinea el eje +X local del pivote con el cursor,
    /// pero el <see cref="Marker2D"/> está desplazado sobre el eje -Y local y apunta
    /// hacia arriba; de ahí la corrección constante de 90° sobre
    /// <see cref="Node2D.RotationDegrees"/>.
    /// </remarks>
    public void RotateMuzzleToMouse()
    {
        MuzzlePivot.LookAt(GetGlobalMousePosition());
        MuzzlePivot.RotationDegrees += 90f;
    }

    #endregion
}
