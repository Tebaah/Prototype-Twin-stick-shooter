using Godot;

/// <summary>
/// Estado de carga del jugador: permite mantener pulsado el ataque y dispara a
/// cinco disparos por segundo.
/// </summary>
/// <remarks>
/// El cooldown se conserva entre transiciones para que volver desde
/// <c>AttackState</c> no reinicie la cadencia. El primer disparo es inmediato.
/// </remarks>
public partial class ChargeState : PlayerState
{
	/// <summary>Intervalo entre disparos: 0.2 s equivale a 5 disparos/segundo.</summary>
	private const float FireInterval = 0.2f;

	/// <summary>Tiempo restante hasta el siguiente disparo.</summary>
	private float _cooldown;

	/// <inheritdoc/>
	public override void FrameUpdate(double delta)
	{
		base.FrameUpdate(delta);

		_cooldown = Mathf.Max(0f, _cooldown - (float)delta);

		if (Input.IsActionPressed("ui_attack") && _cooldown <= 0f)
		{
			_cooldown = FireInterval;
			Machine.TransitionTo("AttackState");
		}
	}
}
