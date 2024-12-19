using System;
using System.Collections.Generic;
using Mirror;
using Pepperon.Scripts.Entities.States._Base;
using Pepperon.Scripts.Units.States;
using Pepperon.Scripts.Units.States.AttackingState;
using Pepperon.Scripts.Units.States.DyingState;
using Pepperon.Scripts.Units.States.IdleState;
using Pepperon.Scripts.Units.States.RunningState;
using UnityEngine;

namespace Pepperon.Scripts.Net.Serializers {
public static class UnitStateSerializer {
    private static readonly Dictionary<byte, EntityState> StateMap = new() {
        { Idle.Id, IdleHolder.Instance },
        { Running.Id, RunningHolder.Instance },
        { Attacking.Id, AttackingStateHolder.Instance },
        { Dying.Id, DyingHolder.Instance },
    };

    // const byte IDLE = 1;
    // const byte RUNNING = 2;
    // const byte ATTACKING = 3;
    // const byte DYING = 4;

    public static void WriteUnitState(this NetworkWriter writer, EntityState entityState) {
        writer.WriteByte(entityState.GetId);
        // switch (entityState) {
        //     case Idle:
        //         writer.WriteByte(IDLE);
        //         break;
        //     case Running:
        //         writer.WriteByte(RUNNING);
        //         break;
        //     case Attacking:
        //         writer.WriteByte(ATTACKING);
        //         break;
        //     case Dying:
        //         writer.WriteByte(DYING);
        //         break;
        //     default:
        //         writer.WriteByte(IDLE);
        //         break;
        // }
    }

    public static EntityState ReadUnitState(this NetworkReader reader) {
        var type = reader.ReadByte();
        if (StateMap.TryGetValue(type, out var state))
            return state;
        throw new Exception($"Invalid UnitState type {type}");

        // var type = reader.ReadByte();
        // return type switch {
        //     IDLE => Idle.Instance,
        //     RUNNING => Running.Instance,
        //     ATTACKING => Attacking.Instance,
        //     DYING => Dying.Instance,
        //     _ => throw new Exception($"Invalid UnitState type {type}")
        // };
    }
}
}