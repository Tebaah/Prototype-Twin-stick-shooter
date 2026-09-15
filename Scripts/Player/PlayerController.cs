using System;
using Godot;

public partial class PlayerController : CharacterBody2D
{
    #region Variables and Properties

    /// <summary>Velocidad base de movimiento en píxeles/segundo (GDD: 200 px/s).</summary>
    [Export] public float MaxSpeed { get; set; } = 200f;

    /// <summary>Dirección de entrada normalizada que aporta el estado activo.</summary>
    public Vector2 DirectionOfMovement { get; set; }

    #endregion

    #region Godot Methods

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        RotateToMouse();
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Orienta al jugador hacia el cursor del ratón.
    /// <see cref="Node2D.LookAt"/> alinea el eje +X local con el cursor, pero el frente del
    /// personaje y el marcador de puntería (Marker2D, en (0, -22) local) apuntan hacia arriba
    /// (eje -Y local); de ahí la corrección constante de 90° sobre <see cref="Node2D.RotationDegrees"/>.
    /// </summary>
    public void RotateToMouse()
    {
        LookAt(GetGlobalMousePosition());
        RotationDegrees += 90f;
    }

    #endregion
}
