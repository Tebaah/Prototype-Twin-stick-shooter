using Godot;
using System;

public abstract partial class State<T> : Node where T : Node
{
	public T Body {get; private set;}
	public StateMachine<T> Machine {get; private set; }


	public void Init(T body, StateMachine<T> machine)
	{
		Body = body;
		Machine = machine;
	}

	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void FrameUpdate(double delta){}
	public virtual void PhysicsUpdate(double delta) {}
}
