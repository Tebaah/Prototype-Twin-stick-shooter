using Godot;

/// <summary>
/// HUD de la partida: vidas, puntaje y aviso de fin de partida.
/// </summary>
/// <remarks>
/// Se suscribe a <see cref="GameManager"/> en <see cref="_Ready"/> y no consulta al
/// jugador directamente, de modo que el gestor sigue siendo la única fuente de verdad.
/// </remarks>
public partial class Hud : CanvasLayer
{
	/// <summary>Etiqueta de las vidas restantes.</summary>
	private Label _livesLabel;

	/// <summary>Etiqueta del puntaje acumulado.</summary>
	private Label _scoreLabel;

	/// <summary>Aviso centrado de fin de partida, oculto durante el juego.</summary>
	private Label _gameOverLabel;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_livesLabel = GetNode<Label>("LivesLabel");
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_gameOverLabel = GetNode<Label>("GameOverLabel");

		GameManager gameManager = GameManager.Instance;
		if (gameManager is null)
		{
			GD.PushWarning("Hud: no hay GameManager cargado; el HUD no se actualizará.");
			return;
		}

		gameManager.LivesChanged += UpdateLives;
		gameManager.ScoreChanged += UpdateScore;
		gameManager.GameOver += ShowGameOver;

		UpdateLives(gameManager.Lives);
		UpdateScore(gameManager.Score);
		_gameOverLabel.Visible = false;
	}

	/// <inheritdoc/>
	public override void _ExitTree()
	{
		GameManager gameManager = GameManager.Instance;
		if (gameManager is null)
			return;

		gameManager.LivesChanged -= UpdateLives;
		gameManager.ScoreChanged -= UpdateScore;
		gameManager.GameOver -= ShowGameOver;
	}

	/// <summary>Refleja en la etiqueta la vida reportada por el gestor.</summary>
	/// <param name="lives">Vidas restantes.</param>
	private void UpdateLives(int lives) => _livesLabel.Text = $"VIDAS: {lives}";

	/// <summary>Refleja en la etiqueta el puntaje reportado por el gestor.</summary>
	/// <param name="score">Puntaje acumulado.</param>
	private void UpdateScore(int score) => _scoreLabel.Text = $"PUNTOS: {score}";

	/// <summary>Muestra el aviso de fin de partida.</summary>
	private void ShowGameOver() => _gameOverLabel.Visible = true;
}
