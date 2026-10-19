using Godot;
using System;

/// <summary>
/// Estado global de la partida: vidas del jugador, puntaje acumulado y fin de partida.
/// </summary>
/// <remarks>
/// Está registrado como autoload en <c>project.godot</c>, así que existe una única
/// instancia accesible mediante <see cref="Instance"/> durante toda la sesión.
/// El jugador se registra al entrar al árbol (<see cref="RegisterPlayer"/>) y el
/// gestor replica sus vidas; los enemigos notifican su puntaje al morir
/// (<see cref="AddScore"/>). La interfaz se suscribe a <see cref="LivesChanged"/>,
/// <see cref="ScoreChanged"/> y <see cref="GameOver"/> para reflejar esos cambios.
/// </remarks>
public partial class GameManager : Node
{
	/// <summary>Número de vidas del jugador al iniciar la partida.</summary>
	public const int MaxLives = 3;

	/// <summary>Instancia activa del gestor; <c>null</c> si el autoload no está cargado.</summary>
	public static GameManager Instance { get; private set; }

	/// <summary>Vidas restantes del jugador; siempre entre 0 y <see cref="MaxLives"/>.</summary>
	public int Lives { get; private set; } = MaxLives;

	/// <summary>Puntaje acumulado por los enemigos eliminados.</summary>
	public int Score { get; private set; }

	/// <summary>Se emite cuando cambian las vidas, con el valor actual.</summary>
	public event Action<int> LivesChanged;

	/// <summary>Se emite cuando cambia el puntaje, con el valor actual.</summary>
	public event Action<int> ScoreChanged;

	/// <summary>Se emite una sola vez cuando el jugador se queda sin vidas.</summary>
	public event Action GameOver;

	/// <summary>Jugador vinculado actualmente, si lo hay.</summary>
	private PlayerController _player;

	/// <inheritdoc/>
	public override void _Ready()
	{
		Instance = this;
	}

	/// <inheritdoc/>
	public override void _ExitTree()
	{
		if (Instance == this)
			Instance = null;
	}

	/// <summary>
	/// Vincula al jugador con el gestor: replica sus vidas y escucha sus cambios.
	/// </summary>
	/// <param name="player">Jugador activo, ya dentro del árbol de escenas.</param>
	public void RegisterPlayer(PlayerController player)
	{
		if (player is null || player == _player)
			return;

		UnregisterPlayer(_player);
		_player = player;
		_player.HealthChanged += OnPlayerHealthChanged;
		_player.Died += OnPlayerDied;

		SetLives(_player.Health);
	}

	/// <summary>
	/// Desvincula al jugador cuando sale del árbol, para no invocar eventos sobre
	/// un nodo ya liberado.
	/// </summary>
	/// <param name="player">Jugador que se retira.</param>
	public void UnregisterPlayer(PlayerController player)
	{
		if (player is null || player != _player)
			return;

		_player.HealthChanged -= OnPlayerHealthChanged;
		_player.Died -= OnPlayerDied;
		_player = null;
	}

	/// <summary>Suma al marcador el puntaje de un enemigo eliminado.</summary>
	/// <param name="amount">Puntos a sumar; los valores no positivos se ignoran.</param>
	public void AddScore(int amount)
	{
		if (amount <= 0)
			return;

		Score += amount;
		ScoreChanged?.Invoke(Score);
	}

	/// <summary>Replica en el gestor la vida que reporta el jugador.</summary>
	/// <param name="health">Vida restante reportada por el jugador.</param>
	private void OnPlayerHealthChanged(int health) => SetLives(health);

	/// <summary>Propaga el fin de partida cuando el jugador se queda sin vidas.</summary>
	private void OnPlayerDied() => GameOver?.Invoke();

	/// <summary>Fija las vidas dentro del rango permitido y avisa a la interfaz.</summary>
	/// <param name="lives">Vidas reportadas.</param>
	private void SetLives(int lives)
	{
		Lives = Math.Clamp(lives, 0, MaxLives);
		LivesChanged?.Invoke(Lives);
	}
}
