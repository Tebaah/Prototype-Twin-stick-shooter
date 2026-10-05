using System;
using Godot;
namespace Game.Enemy;

/// <summary>
/// Base de los estados del enemigo. El cuerpo sobre el que actúan es un
/// <see cref="EnemyRusher"/>; <see cref="EnemyStateMachine"/> orquesta las
/// transiciones entre estados.
/// </summary>
public partial class EnemyState : State<EnemyRusher>
{
}
