using System;
using Godot;
namespace Game.Enemy.States;

public partial class LookingForTargetState : State<EnemyRusher>
{

	private void OnBodyEntered(Node2D target)
	{
		if (target.IsInGroup("Player"))
		{
			Machine.TransitionTo("AttackState");
		}
	}
}
