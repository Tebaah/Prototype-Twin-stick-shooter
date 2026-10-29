using System;
using Godot;
namespace Game.Enemy;

/// <summary>
/// Enemigo de tipo "rusher": persigue al jugador mientras este permanece en su
/// área de detección. La decisión de perseguir o detenerse la gestiona
/// <see cref="EnemyStateMachine"/> a través de sus estados.
/// </summary>
public partial class EnemyRusher : CharacterBody2D
{
	/// <summary>Velocidad de persecución en píxeles/segundo.</summary>
	[Export] public float Speed { get; set; } = 300.0f;

	/// <summary>Puntaje mínimo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MinScore { get; set; } = 10;

	/// <summary>Puntaje máximo que puede otorgar este enemigo al morir.</summary>
	[Export] public int MaxScore { get; set; } = 50;

	/// <summary>Dirección unitaria de persecución, recalculada por el estado activo.</summary>
	public Vector2 Direction { get; set; } = Vector2.Zero;

	/// <summary>Puntaje aleatorio asignado a esta instancia al entrar en escena.</summary>
	public int ScoreValue { get; private set; }

	/// <summary>Evita otorgar el puntaje más de una vez si muere en el mismo fotograma.</summary>
	private bool _isDead;

	/// <inheritdoc/>
	public override void _Ready()
	{
		ScoreValue = GD.RandRange(MinScore, MaxScore);
		GameManager.Instance?.RegisterEnemy(this);
	}

	/// <inheritdoc/>
	public override void _ExitTree()
	{
		GameManager.Instance?.UnregisterEnemy(this);
	}

	/// <summary>
	/// Elimina al enemigo y suma su puntaje al marcador global.
	/// </summary>
	/// <remarks>
	/// Es idempotente: los estados de ataque y las balas pueden invocarlo, pero solo
	/// la primera llamada otorga puntos y libera el nodo.
	/// </remarks>
	public void Die()
	{
		if (_isDead)
			return;

		_isDead = true;
		GameManager.Instance?.AddScore(ScoreValue);
		QueueFree();
	}
}
