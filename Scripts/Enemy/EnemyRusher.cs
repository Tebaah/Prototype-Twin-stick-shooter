namespace Game.Enemy;

/// <summary>
/// Enemigo Rusher: persigue al jugador y explota al contacto sin otorgar puntaje.
/// </summary>
/// <remarks>
/// Toda la salud, detección, contacto y puntaje se hereda de <see cref="EnemyController"/>.
/// Su diferencia es que <see cref="EnemyController.DestroyOnPlayerContact"/> vale
/// <c>true</c> por defecto.
/// </remarks>
public partial class EnemyRusher : EnemyController
{
}
