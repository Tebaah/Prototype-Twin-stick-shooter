using Godot;

/// <summary>
/// Base común de los estados del jugador.
/// Centraliza el criterio de velocidad: un estado que desplaza al jugador lo hace
/// a un porcentaje de <see cref="PlayerController.MaxSpeed"/>, nunca a un valor suelto.
/// Un estado que no se desplaza (por ejemplo <see cref="IdleState"/>) no declara velocidad.
/// </summary>
public abstract partial class PlayerState : State<PlayerController>
{
    /// <summary>
    /// Mecánica compartida de desplazamiento: lee el input, fija la velocidad efectiva
    /// del estado y mueve el cuerpo.
    /// </summary>
    /// <param name="multiplier">Porcentaje de la velocidad base (1 = 100%).</param>
    protected void MoveWithInput(float multiplier)
    {
        Body.DirectionOfMovement = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Body.Velocity = Body.DirectionOfMovement * Body.MaxSpeed * multiplier;
        Body.MoveAndSlide();
    }
}

