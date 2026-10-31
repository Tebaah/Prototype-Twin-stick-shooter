using Godot;
using System;

/// <summary>
/// Administra la salud de una entidad: salud máxima, salud actual, daño y curación.
/// </summary>
/// <remarks>
/// Es un componente de datos: no dibuja ni procesa cada fotograma. La entidad dueña
/// decide qué hacer cuando la salud llega a cero mediante <see cref="Died"/>.
/// La salud máxima puede aumentarse en tiempo de ejecución sin recrear la entidad.
/// </remarks>
public partial class HealthComponent : Node2D
{
	/// <summary>Salud máxima con la que inicia la entidad.</summary>
	[Export] public int InitialMaxHealth { get; set; } = 3;

	/// <summary>Salud máxima actual; puede aumentar mediante <see cref="IncreaseMaxHealth"/>.</summary>
	public int MaxHealth { get; private set; }

	/// <summary>Salud actual, siempre entre 0 y <see cref="MaxHealth"/>.</summary>
	public int CurrentHealth { get; private set; }

	/// <summary>Indica si la entidad se quedó sin salud.</summary>
	public bool IsDead => CurrentHealth <= 0;

	/// <summary>Se emite cuando cambia la salud, con (salud actual, salud máxima).</summary>
	public event Action<int, int> HealthChanged;

	/// <summary>Se emite una sola vez cuando la salud llega a 0.</summary>
	public event Action Died;

	/// <inheritdoc/>
	public override void _Ready()
	{
		MaxHealth = Math.Max(1, InitialMaxHealth);
		CurrentHealth = MaxHealth;
	}

	/// <summary>Descuenta salud y emite <see cref="Died"/> si llega a cero.</summary>
	/// <param name="amount">Daño recibido; los valores no positivos se ignoran.</param>
	public void TakeDamage(int amount)
	{
		if (amount <= 0 || IsDead)
			return;

		CurrentHealth = Math.Max(0, CurrentHealth - amount);
		NotifyHealthChanged();

		if (CurrentHealth == 0)
			Died?.Invoke();
	}

	/// <summary>Recupera salud sin superar el máximo ni revivir a una entidad muerta.</summary>
	/// <param name="amount">Salud a recuperar; los valores no positivos se ignoran.</param>
	public void Heal(int amount)
	{
		if (amount <= 0 || IsDead || CurrentHealth >= MaxHealth)
			return;

		CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
		NotifyHealthChanged();
	}

	/// <summary>Aumenta la salud máxima de la entidad.</summary>
	/// <param name="amount">Incremento de salud máxima; los valores no positivos se ignoran.</param>
	/// <param name="healBySameAmount">
	/// Si es <c>true</c>, también recupera <paramref name="amount"/> de salud actual,
	/// sin superar el nuevo máximo. Si la entidad está muerta no se revive.
	/// </param>
	public void IncreaseMaxHealth(int amount, bool healBySameAmount = false)
	{
		if (amount <= 0)
			return;

		MaxHealth += amount;

		if (healBySameAmount && !IsDead)
			CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);

		NotifyHealthChanged();
	}

	/// <summary>Notifica el estado actual de salud a los interesados.</summary>
	private void NotifyHealthChanged() => HealthChanged?.Invoke(CurrentHealth, MaxHealth);
}
