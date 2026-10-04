using System;
using Godot;

/// <summary>
/// Proyectil disparado por el jugador.
/// Es un <see cref="Area2D"/> (detecta colisiones sin resolverlas físicamente):
/// al entrar en escena orienta su <see cref="Direction"/> hacia el cursor y
/// avanza en línea recta a <see cref="MovementSpeed"/> píxeles por segundo.
/// </summary>
/// <remarks>
/// Ciclo de vida parcial (v1.0): la bala se libera solo al salir de la pantalla,
/// mediante la señal <c>screen_exited</c> de <see cref="VisibleOnScreenNotifier2D"/>.
/// Todavía no se destruye al impactar obstáculos ni enemigos.
/// </remarks>
public partial class Bullet : Area2D
{
	/// <summary>Vector unitario de avance. Lo fija <see cref="_Ready"/> apuntando al ratón.</summary>
	private Vector2 Direction { get; set; }

	/// <summary>Velocidad de avance en píxeles/segundo.</summary>
	[Export]
	private float MovementSpeed { get; set; } = 350;

	/// <summary>
	/// Al entrar en escena apunta la bala hacia la posición actual del cursor,
	/// de modo que se dispara hacia donde apunta el jugador en ese instante.
	/// </summary>
	public override void _Ready()
	{
		Direction = GlobalPosition.DirectionTo(GetGlobalMousePosition());
	}

	/// <summary>Desplaza la bala cada fotograma en su <see cref="Direction"/> a <see cref="MovementSpeed"/>.</summary>
	/// <param name="delta">Tiempo transcurrido desde el fotograma anterior, en segundos.</param>
	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * MovementSpeed * (float)delta;
	}

	/// <summary>
	/// Libera la bala cuando su notificador sale de la pantalla.
	/// Invocado por la señal <c>screen_exited</c> conectada desde <c>bullet.tscn</c>.
	/// </summary>
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
