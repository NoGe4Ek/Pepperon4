using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Pepperon.Scripts.Entities.Components;
using Pepperon.Scripts.Entities.Components.AttackingComponents;
using Pepperon.Scripts.Entities.States._Base;
using Pepperon.Scripts.Entities.States._Base.Strategies;
using Pepperon.Scripts.Managers;
using Pepperon.Scripts.Net.Serializers;
using Pepperon.Scripts.Units.Components.AttackingComponents;
using Pepperon.Scripts.Units.States.DyingState;
using Pepperon.Scripts.Units.States.RunningState;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Pepperon.Scripts.Units.Components.StateMachines {
[Serializable]
public class StateStrategyItem {
    [SerializeField] public EntityState state;
    [SerializeField] public CompositeStrategy strategy;
}

[Serializable]
public class StateStrategyMap {
    [SerializeField] private StateStrategyItem[] map;
    private Dictionary<EntityState, CompositeStrategy> cachedDictionary;

    private Dictionary<EntityState, CompositeStrategy> Dictionary {
        get { return cachedDictionary ??= ToDictionary(); }
    }

    private Dictionary<EntityState, CompositeStrategy> ToDictionary() {
        return map.ToDictionary(item => item.state, item => item.strategy);
    }

    public CompositeStrategy GetStrategy(EntityState state) {
        return Dictionary.GetValueOrDefault(state);
    }
}

public class StateMachineComponent : NetworkBehaviour {
    [SerializeField] private StateStrategyMap stateStrategy;

    [SyncVar(hook = nameof(OnUnitStateChange))]
    public EntityState currentState;

    public CompositeStrategy CurrentStrategy() => stateStrategy.GetStrategy(currentState);

    public MovementComponent movementComponent;
    public HealthComponent healthComponent;
    public BaseAttackingComponent attackingComponent;
    public AnimationComponent animationComponent;


    public void OnUnitStateChange(EntityState oldEntityState, EntityState newEntityState) {
        if (isServer) return;
        stateStrategy.GetStrategy(oldEntityState).OnExitState(this);
        stateStrategy.GetStrategy(newEntityState).OnEnterState(this);
    }

    public bool IsDying() => currentState is Dying;
    public bool IsRunning() => currentState is Running;

    private void Awake() {
        movementComponent = GetComponent<MovementComponent>();
        healthComponent = GetComponent<HealthComponent>();
        attackingComponent = GetComponent<BaseAttackingComponent>();
        animationComponent = GetComponent<AnimationComponent>();
    }

    private void Start() {
        CurrentStrategy().OnEnterState(this);

        if (!isServer) return;
        BattleManager.OnKill += OnDie;
    }

    private void Update() {
        CurrentStrategy().OnActiveState(this);
    }

    public void SwitchState(EntityState newEntityState) {
        CurrentStrategy().OnExitState(this);
        currentState = newEntityState;
        CurrentStrategy().OnEnterState(this);
    }

    private void OnDie(GameObject killerObject, GameObject diedObject) {
        if (diedObject == gameObject && !IsDying()) {
            SwitchState(DyingHolder.Instance);
        }
    }

    private void OnDestroy() {
        BattleManager.OnKill -= OnDie;
    }
}
}