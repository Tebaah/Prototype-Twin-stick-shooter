using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Genera obstáculos al azar a partir de un catálogo de escenas.
/// </summary>
/// <remarks>
/// Cada obstáculo se coloca en el centro de una celda libre de la arena
/// (12x12 celdas de 64 píxeles), de modo que dos obstáculos nunca comparten
/// celda ni repiten la posición del anterior. Las cuatro celdas centrales
/// quedan reservadas como zona segura de aparición del jugador.
/// </remarks>
public partial class Obstacles : Node
{
	/// <summary>Celdas por lado de la arena (12x12 según el GDD).</summary>
	private const int ArenaCellsPerSide = 12;

	/// <summary>Lado de una celda en píxeles; debe coincidir con el TileSet del mapa.</summary>
	private const int CellSize = 64;

	/// <summary>Coordenada mínima (inclusive) de la zona segura central de 2x2 celdas.</summary>
	private const int SafeZoneMinCell = 5;

	/// <summary>Coordenada máxima (inclusive) de la zona segura central de 2x2 celdas.</summary>
	private const int SafeZoneMaxCell = 6;

	/// <summary>Escenas candidatas entre las que se elige cada obstáculo.</summary>
	/// <remarks>Los nombres de los campos exportados están serializados en las escenas; no renombrar.</remarks>
	[Export] private PackedScene[] obstacles = Array.Empty<PackedScene>();

	/// <summary>Cantidad de obstáculos a instanciar en <see cref="_Ready"/>.</summary>
	[Export] private int numberOfObstacles;

	/// <inheritdoc/>
	public override void _Ready()
	{
		if (obstacles is null || obstacles.Length == 0)
		{
			GD.PushWarning("Obstacles: no hay escenas configuradas; no se generará ningún obstáculo.");
			return;
		}

		// Se sortean celdas libres en lugar de reintentar posiciones al azar:
		// así ningún obstáculo se solapa con otro ni hay bucles infinitos.
		var freeCells = new List<Vector2I>(ArenaCellsPerSide * ArenaCellsPerSide);
		for (int y = 0; y < ArenaCellsPerSide; y++)
		{
			for (int x = 0; x < ArenaCellsPerSide; x++)
			{
				if (!IsSafeZoneCell(x, y))
				{
					freeCells.Add(new Vector2I(x, y));
				}
			}
		}

		int obstacleCount = Math.Min(numberOfObstacles, freeCells.Count);
		if (numberOfObstacles > obstacleCount)
		{
			GD.PushWarning($"Obstacles: la arena solo admite {freeCells.Count} obstáculos fuera de la zona segura; se generarán {obstacleCount} de {numberOfObstacles}.");
		}

		for (int i = 0; i < obstacleCount; i++)
		{
			// RandRange incluye ambos extremos, así que el último índice válido es Length - 1.
			int sceneIndex = GD.RandRange(0, obstacles.Length - 1);
			Node2D obstacle = obstacles[sceneIndex].Instantiate<Node2D>();
			AddChild(obstacle);

			int cellIndex = GD.RandRange(0, freeCells.Count - 1);
			Vector2I cell = freeCells[cellIndex];
			freeCells.RemoveAt(cellIndex);

			// Posición local al mapa, que es el origen de la rejilla de la arena.
			obstacle.Position = CellCenter(cell);
		}
	}

	/// <summary>
	/// Indica si la celda pertenece a la zona segura central donde aparece el jugador.
	/// </summary>
	/// <param name="x">Columna de la celda.</param>
	/// <param name="y">Fila de la celda.</param>
	/// <returns><c>true</c> si la celda está reservada y no admite obstáculos.</returns>
	private static bool IsSafeZoneCell(int x, int y)
	{
		return x >= SafeZoneMinCell && x <= SafeZoneMaxCell
			&& y >= SafeZoneMinCell && y <= SafeZoneMaxCell;
	}

	/// <summary>Calcula el centro en píxeles de una celda de la rejilla.</summary>
	/// <param name="cell">Coordenadas de la celda dentro de la arena.</param>
	/// <returns>Posición en píxeles relativa al origen de la arena.</returns>
	private static Vector2 CellCenter(Vector2I cell)
	{
		return new Vector2(cell.X * CellSize + CellSize / 2f, cell.Y * CellSize + CellSize / 2f);
	}
}