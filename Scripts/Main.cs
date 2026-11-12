using Godot;

namespace PrototypeTwinstickshooter;

/// <summary>
/// Nodo raíz de la escena principal.
/// Punto de entrada del prototipo (C#).
/// </summary>
public partial class Main : Node
{
    /// <inheritdoc/>
    /// <remarks>
    /// Se ejecuta antes de que los hijos (jugador, HUD, mapa) entren en escena,
    /// para que el <see cref="GameManager"/> arranque con un estado limpio.
    /// </remarks>
    public override void _EnterTree()
    {
        GameManager.Instance?.ResetGame();
    }

    public override void _Ready()
    {
        GD.Print("Prototipo twin-stick shooter iniciado correctamente (C#).");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // ESC / Start cierra el juego durante las pruebas.
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
    }
}
