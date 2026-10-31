using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Estado global de la partida: vidas, nivel, enemigos, puntaje y fin de partida.
/// </summary>
/// <remarks>
/// Está registrado como autoload en <c>project.godot</c>, así que existe una única
/// instancia accesible mediante <see cref="Instance"/> durante toda la sesión.
/// El jugador se registra al entrar al árbol (<see cref="RegisterPlayer"/>) y el
/// gestor replica sus vidas; los enemigos se registran al aparecer y notifican su
/// puntaje al morir (<see cref="AddScore"/>). La interfaz se suscribe a
/// <see cref="LivesChanged"/>, <see cref="LevelChanged"/>, <see cref="EnemiesChanged"/>,
/// <see cref="ScoreChanged"/> y <see cref="GameOver"/> para reflejar esos cambios.
/// </remarks>
public partial class GameManager : Node
{
	/// <summary>Número de vidas con el que inicia la partida.</summary>
	public const int DefaultMaxLives = 3;

	/// <summary>Instancia activa del gestor; <c>null</c> si el autoload no está cargado.</summary>
	public static GameManager Instance { get; private set; }

	/// <summary>Vidas máximas actuales del jugador; puede aumentar con su salud máxima.</summary>
	public int MaxLives { get; private set; } = DefaultMaxLives;

	/// <summary>Vidas restantes del jugador; siempre entre 0 y <see cref="MaxLives"/>.</summary>
	public int Lives { get; private set; } = DefaultMaxLives;

	/// <summary>Nivel actual de la partida; empieza en 1.</summary>
	public int Level { get; private set; } = 1;

	/// <summary>Total de enemigos generados en el nivel actual.</summary>
	public int TotalEnemies { get; private set; }

	/// <summary>Cantidad de enemigos vivos en el nivel actual.</summary>
	public int EnemiesRemaining => _aliveEnemies.Count;

	/// <summary>Puntaje acumulado por los enemigos eliminados.</summary>
	public int Score { get; private set; }

	/// <summary>Se emite cuando cambian las vidas, con (vidas actuales, vidas máximas).</summary>
	public event Action<int, int> LivesChanged;

	/// <summary>Se emite cuando cambia el nivel, con el valor actual.</summary>
	public event Action<int> LevelChanged;

	/// <summary>Se emite cuando cambia el conteo de enemigos, con (restantes, total).</summary>
	public event Action<int, int> EnemiesChanged;

	/// <summary>Se emite cuando cambia el puntaje, con el valor actual.</summary>
	public event Action<int> ScoreChanged;

	/// <summary>Se emite una sola vez cuando el jugador se queda sin vidas.</summary>
	public event Action GameOver;

	/// <summary>Enemigos vivos registrados, usados para calcular los restantes.</summary>
	private readonly HashSet<Node2D> _aliveEnemies = new();

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

		SetLives(_player.Health, _player.MaxHealth);
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

	/// <summary>Registra un enemigo vivo y actualiza el contador restantes/total.</summary>
	/// <param name="enemy">Enemigo que acaba de entrar en escena.</param>
	/// <remarks>
	/// Si no quedaba ningún enemigo vivo se reinicia el total, de modo que una nueva
	/// oleada o una recarga de escena no acumule los enemigos de la anterior.
	/// </remarks>
	public void RegisterEnemy(Node2D enemy)
	{
		if (enemy is null || !_aliveEnemies.Add(enemy))
			return;

		if (_aliveEnemies.Count == 1)
			TotalEnemies = 0;

		TotalEnemies++;
		EnemiesChanged?.Invoke(EnemiesRemaining, TotalEnemies);
	}

	/// <summary>Quita un enemigo del conteo cuando muere o sale de escena.</summary>
	/// <param name="enemy">Enemigo que deja de estar vivo.</param>
	public void UnregisterEnemy(Node2D enemy)
	{
		if (enemy is null || !_aliveEnemies.Remove(enemy))
			return;

		EnemiesChanged?.Invoke(EnemiesRemaining, TotalEnemies);
	}

	/// <summary>Fija el nivel actual y avisa a la interfaz.</summary>
	/// <param name="level">Nivel a mostrar; los valores menores que 1 se elevan a 1.</param>
	public void SetLevel(int level)
	{
		int nextLevel = Math.Max(1, level);
		if (nextLevel == Level)
			return;

		Level = nextLevel;
		LevelChanged?.Invoke(Level);
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

	/// <summary>Replica en el gestor la vida y el máximo que reporta el jugador.</summary>
	/// <param name="health">Vida restante reportada por el jugador.</param>
	/// <param name="maxHealth">Vida máxima actual del jugador.</param>
	private void OnPlayerHealthChanged(int health, int maxHealth) => SetLives(health, maxHealth);

	/// <summary>Propaga el fin de partida cuando el jugador se queda sin vidas.</summary>
	private void OnPlayerDied() => GameOver?.Invoke();

	/// <summary>Fija las vidas dentro del rango permitido y avisa a la interfaz.</summary>
	/// <param name="lives">Vidas reportadas.</param>
	/// <param name="maxLives">Vidas máximas actuales; debe ser al menos 1.</param>
	private void SetLives(int lives, int maxLives)
	{
		MaxLives = Math.Max(1, maxLives);
		Lives = Math.Clamp(lives, 0, MaxLives);
		LivesChanged?.Invoke(Lives, MaxLives);
	}
}
