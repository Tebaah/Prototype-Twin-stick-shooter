using Godot;

/// <summary>
/// HUD de la partida: corazones de vida, nivel, enemigos restantes y puntaje.
/// </summary>
/// <remarks>
/// Se suscribe a <see cref="GameManager"/> en <see cref="_Ready"/> y no consulta al
/// jugador directamente, de modo que el gestor sigue siendo la única fuente de verdad.
/// </remarks>
public partial class Hud : CanvasLayer
{
	/// <summary>Corazones que representan las vidas restantes, en orden de izquierda a derecha.</summary>
	private TextureRect[] _hearts;

	/// <summary>Etiqueta del nivel actual.</summary>
	private Label _levelLabel;

	/// <summary>Etiqueta de enemigos restantes y total del nivel.</summary>
	private Label _enemiesLabel;

	/// <summary>Etiqueta del puntaje acumulado.</summary>
	private Label _scoreLabel;

	/// <summary>Aviso centrado de fin de partida, oculto durante el juego.</summary>
	private Label _gameOverLabel;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_hearts = new[]
		{
			GetNode<TextureRect>("HudBar/Hearts/Heart1"),
			GetNode<TextureRect>("HudBar/Hearts/Heart2"),
			GetNode<TextureRect>("HudBar/Hearts/Heart3")
		};
		_levelLabel = GetNode<Label>("HudBar/LevelLabel");
		_enemiesLabel = GetNode<Label>("HudBar/EnemiesLabel");
		_scoreLabel = GetNode<Label>("HudBar/ScoreLabel");
		_gameOverLabel = GetNode<Label>("GameOverLabel");

		GameManager gameManager = GameManager.Instance;
		if (gameManager is null)
		{
			GD.PushWarning("Hud: no hay GameManager cargado; el HUD no se actualizará.");
			return;
		}

		gameManager.LivesChanged += UpdateLives;
		gameManager.LevelChanged += UpdateLevel;
		gameManager.EnemiesChanged += UpdateEnemies;
		gameManager.ScoreChanged += UpdateScore;
		gameManager.GameOver += ShowGameOver;

		UpdateLives(gameManager.Lives);
		UpdateLevel(gameManager.Level);
		UpdateEnemies(gameManager.EnemiesRemaining, gameManager.TotalEnemies);
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
		gameManager.LevelChanged -= UpdateLevel;
		gameManager.EnemiesChanged -= UpdateEnemies;
		gameManager.ScoreChanged -= UpdateScore;
		gameManager.GameOver -= ShowGameOver;
	}

	/// <summary>
	/// Muestra tantos corazones como vidas queden y oculta los sobrantes.
	/// </summary>
	/// <param name="lives">Vidas restantes.</param>
	/// <remarks>
	/// Se cambia el alfa en lugar de <see cref="CanvasItem.Visible"/> para que el
	/// contenedor reserve siempre el espacio de los tres corazones y el resto del
	/// HUD no se desplace al perder una vida.
	/// </remarks>
	private void UpdateLives(int lives)
	{
		for (int i = 0; i < _hearts.Length; i++)
			_hearts[i].Modulate = new Color(1f, 1f, 1f, i < lives ? 1f : 0f);
	}

	/// <summary>Refleja en la etiqueta el nivel reportado por el gestor.</summary>
	/// <param name="level">Nivel actual.</param>
	private void UpdateLevel(int level) => _levelLabel.Text = $"NIVEL: {level}";

	/// <summary>Refleja en la etiqueta los enemigos restantes y el total del nivel.</summary>
	/// <param name="remaining">Enemigos vivos.</param>
	/// <param name="total">Enemigos generados en el nivel.</param>
	private void UpdateEnemies(int remaining, int total) => _enemiesLabel.Text = $"ENEMIGOS: {remaining}/{total}";

	/// <summary>Refleja en la etiqueta el puntaje reportado por el gestor.</summary>
	/// <param name="score">Puntaje acumulado.</param>
	private void UpdateScore(int score) => _scoreLabel.Text = $"PUNTOS: {score}";

	/// <summary>Muestra el aviso de fin de partida.</summary>
	private void ShowGameOver() => _gameOverLabel.Visible = true;
}
