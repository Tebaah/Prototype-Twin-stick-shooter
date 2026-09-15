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

    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
