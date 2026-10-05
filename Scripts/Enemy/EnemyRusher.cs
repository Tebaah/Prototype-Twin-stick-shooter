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

	/// <summary>Dirección unitaria de persecución, recalculada por el estado activo.</summary>
	public Vector2 Direction { get; set; } = Vector2.Zero;
}
