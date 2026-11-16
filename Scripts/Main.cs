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
    /// Se ejecuta antes de que los hijos (jugador, HUD, mapa) entren en escena.
    /// Deja el árbol sin pausa y reinicia el <see cref="GameManager"/> para que la
    /// partida arranque con un estado limpio.
    /// </remarks>
    public override void _EnterTree()
    {
        ProcessMode = ProcessModeEnum.Always;
        GetTree().Paused = false;
        GameManager.Instance?.ResetGame();
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (GameManager.Instance is not null)
            GameManager.Instance.GameOver += OnGameOver;

        GD.Print("Prototipo twin-stick shooter iniciado correctamente (C#).");
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        if (GameManager.Instance is not null)
            GameManager.Instance.GameOver -= OnGameOver;
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
            return;
        }

        if (@event.IsActionPressed("ui_restart") && GameManager.Instance?.IsGameOver == true)
        {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }
    }

    /// <summary>Detiene la partida cuando el jugador se queda sin vidas.</summary>
    private void OnGameOver()
    {
        GetTree().Paused = true;
    }
}
