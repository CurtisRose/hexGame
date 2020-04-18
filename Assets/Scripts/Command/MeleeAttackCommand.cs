using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackCommand : AttackCommand
{
    public MeleeAttackCommand(HexUnit attackingUnit, HexCell target):base(attackingUnit, target) {
        this.hexUnit = attackingUnit;
        this.target = target;
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {
        // If the units are not neighbors, attack is invalid
        if (!hexUnit.Location.IsNeighbor(target)) {
            return false;
        }

        // If units are on the same team, do not attack
        if (hexUnit.GetPlayer() == target.Unit.GetPlayer()) {
            Debug.Log("These units are on the same team, do not add command.");
            return false;
        }

        return base.ValidateAddCommand(ref commands);
    }
}
