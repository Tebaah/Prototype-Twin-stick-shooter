using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

public partial class StateMachine<T> : Node where T : Node
{
    [Export] private Node _initialStateNode;
    [Export] private Node _bodyNode;

    public State<T> CurrentState {get; private set;}
    private readonly Dictionary<string, State<T>> _states = new();

    public override void _Ready()
    {
        base._Ready();
        // Registrar todos los estados hijos
        foreach (Node child in GetChildren())
            if (child is State<T> state)
                _states[state.Name] = state;

        //  Conectar cada esado con su cuerpo y su máquina
        var body = _bodyNode as T;
        foreach (var state in _states.Values)
            state.Init(body, this);

        // Arrancar el estado inicial
        CurrentState = _initialStateNode as State<T>;
        CurrentState?.Enter();
    }

    public override void _Process(double delta)
        => CurrentState?.FrameUpdate(delta);

    public override void _PhysicsProcess(double delta)
        => CurrentState?.PhysicsUpdate(delta);

    public void TransitionTo(string stateName)
    {
        if (!_states.TryGetValue(stateName, out var next))
        {
            GD.PushWarning($"No existe el estado '{stateName}'.");
            return; 
        }
        if (next == CurrentState) return;

        CurrentState.Exit();
        CurrentState = next;
        next.Enter();
    }
}
