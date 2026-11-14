using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Construye cada nivel: define la composición de enemigos y obstáculos y los
/// instancia en celdas libres de la arena.
/// </summary>
/// <remarks>
/// La tabla sigue el GDD. Los obstáculos se reparten entre 15 y 29 según el nivel;
/// el máximo de 29 corresponde al nivel 10. Los enemigos nunca aparecen en la zona
/// segura central ni en celdas ocupadas por obstáculos.
/// </remarks>
public partial class LevelManager : Node
{
	/// <summary>Celdas por lado de la arena.</summary>
	private const int ArenaCellsPerSide = 12;

	/// <summary>Lado de una celda en píxeles.</summary>
	private const int CellSize = 64;

	/// <summary>Coordenada mínima de la zona segura central.</summary>
	private const int SafeZoneMinCell = 5;

	/// <summary>Coordenada máxima de la zona segura central.</summary>
	private const int SafeZoneMaxCell = 6;

	/// <summary>Composición de un nivel.</summary>
	private readonly struct LevelDefinition
	{
		public LevelDefinition(int rushers, int snipers, int obstacles)
		{
			Rushers = rushers;
			Snipers = snipers;
			Obstacles = obstacles;
		}

		/// <summary>Cantidad de enemigos Rusher.</summary>
		public int Rushers { get; }

		/// <summary>Cantidad de enemigos Sniper/Shooter.</summary>
		public int Snipers { get; }

		/// <summary>Cantidad de obstáculos.</summary>
		public int Obstacles { get; }
	}

	/// <summary>Composición de los niveles 1 a 10 según el GDD.</summary>
	private static readonly LevelDefinition[] Levels =
	{
		new(3, 2, 15),
		new(3, 2, 15),
		new(4, 2, 15),
		new(4, 3, 18),
		new(5, 3, 18),
		new(5, 3, 18),
		new(6, 4, 22),
		new(6, 4, 22),
		new(7, 4, 22),
		new(7, 5, 29)
	};

	/// <summary>Nodo que contiene los enemigos instanciados.</summary>
	private Node _enemiesContainer;

	/// <summary>Mapa que contiene el generador de obstáculos.</summary>
	private Obstacles _obstacles;

	/// <summary>Escena del enemigo Rusher.</summary>
	[Export] public PackedScene RusherScene { get; set; }

	/// <summary>Escena del enemigo Sniper/Shooter.</summary>
	[Export] public PackedScene ShooterScene { get; set; }

	/// <summary>Nivel cargado actualmente.</summary>
	public int CurrentLevel { get; private set; } = 1;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_enemiesContainer = GetNode<Node>("../Enemies");
		_obstacles = GetNode<Node>("../Map") as Obstacles;
		LoadLevel(1);
	}

	/// <summary>Carga el nivel indicado, regenerando enemigos y obstáculos.</summary>
	/// <param name="level">Nivel solicitado; se limita al rango disponible.</param>
	public void LoadLevel(int level)
	{
		if (_enemiesContainer is null || RusherScene is null || ShooterScene is null)
		{
			GD.PushError("LevelManager: faltan el contenedor de enemigos o las escenas de enemigos.");
			return;
		}

		int levelNumber = Math.Clamp(level, 1, Levels.Length);
		LevelDefinition definition = Levels[levelNumber - 1];

		CurrentLevel = levelNumber;
		GameManager.Instance?.SetLevel(levelNumber);

		if (_obstacles is not null)
			_obstacles.Regenerate(definition.Obstacles);
		else
			GD.PushWarning("LevelManager: el mapa no tiene el componente Obstacles.");

		ClearEnemies();
		SpawnEnemies(definition);
	}

	/// <summary>Avanza al siguiente nivel disponible.</summary>
	public void NextLevel() => LoadLevel(CurrentLevel + 1);

	/// <summary>Elimina los enemigos del nivel anterior de forma inmediata.</summary>
	private void ClearEnemies()
	{
		foreach (Node child in _enemiesContainer.GetChildren())
		{
			_enemiesContainer.RemoveChild(child);
			child.QueueFree();
		}
	}

	/// <summary>Instancia la composición de enemigos en celdas libres.</summary>
	/// <param name="definition">Composición del nivel.</param>
	private void SpawnEnemies(LevelDefinition definition)
	{
		List<Vector2I> freeCells = GetFreeCells();
		int required = definition.Rushers + definition.Snipers;

		if (freeCells.Count < required)
		{
			GD.PushWarning($"LevelManager: solo hay {freeCells.Count} celdas libres para {required} enemigos.");
		}

		for (int i = 0; i < definition.Rushers; i++)
			Spawn(RusherScene, freeCells);

		for (int i = 0; i < definition.Snipers; i++)
			Spawn(ShooterScene, freeCells);
	}

	/// <summary>Instancia un enemigo en una celda libre y la descarta de la lista.</summary>
	/// <param name="scene">Escena del enemigo.</param>
	/// <param name="freeCells">Celdas disponibles; se modifica al elegir una.</param>
	private void Spawn(PackedScene scene, List<Vector2I> freeCells)
	{
		if (freeCells.Count == 0)
			return;

		int index = GD.RandRange(0, freeCells.Count - 1);
		Vector2I cell = freeCells[index];
		freeCells.RemoveAt(index);

		Node2D enemy = scene.Instantiate<Node2D>();
		_enemiesContainer.AddChild(enemy);
		enemy.GlobalPosition = CellCenter(cell);
	}

	/// <summary>Obtiene las celdas que no son zona segura ni están ocupadas por obstáculos.</summary>
	/// <returns>Lista de celdas disponibles.</returns>
	private List<Vector2I> GetFreeCells()
	{
		var occupied = new HashSet<Vector2I>();

		if (_obstacles is not null)
		{
			foreach (Node child in _obstacles.GetChildren())
			{
				if (child is Node2D obstacle)
					occupied.Add(PositionToCell(obstacle.Position));
			}
		}

		var freeCells = new List<Vector2I>(ArenaCellsPerSide * ArenaCellsPerSide);
		for (int y = 0; y < ArenaCellsPerSide; y++)
		{
			for (int x = 0; x < ArenaCellsPerSide; x++)
			{
				var cell = new Vector2I(x, y);
				if (IsSafeZoneCell(cell) || occupied.Contains(cell))
					continue;

				freeCells.Add(cell);
			}
		}

		return freeCells;
	}

	/// <summary>Indica si una celda pertenece a la zona segura central.</summary>
	/// <param name="cell">Celda de la arena.</param>
	/// <returns><c>true</c> si la celda está reservada.</returns>
	private static bool IsSafeZoneCell(Vector2I cell)
	{
		return cell.X >= SafeZoneMinCell && cell.X <= SafeZoneMaxCell
			&& cell.Y >= SafeZoneMinCell && cell.Y <= SafeZoneMaxCell;
	}

	/// <summary>Convierte una posición del mapa en coordenadas de celda.</summary>
	/// <param name="position">Posición local al mapa.</param>
	/// <returns>Celda correspondiente.</returns>
	private static Vector2I PositionToCell(Vector2 position)
	{
		return new Vector2I(
			Mathf.FloorToInt(position.X / CellSize),
			Mathf.FloorToInt(position.Y / CellSize));
	}

	/// <summary>Calcula el centro en píxeles de una celda.</summary>
	/// <param name="cell">Celda de la arena.</param>
	/// <returns>Posición global del centro de la celda.</returns>
	private static Vector2 CellCenter(Vector2I cell)
	{
		return new Vector2(cell.X * CellSize + CellSize / 2f, cell.Y * CellSize + CellSize / 2f);
	}
}
