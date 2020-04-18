using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackCommand : AttackCommand
{

    public MeleeAttackCommand(HexUnit attackingUnit, HexUnit defendingUnit):base(attackingUnit, defendingUnit) {
        this.hexUnit = attackingUnit;
        this.defendingUnit = defendingUnit;
    }

    public override bool ValidateAddCommand(ref List<ICommand> commands) {
        // If the units are not neighbors, attack is invalid
        if (!hexUnit.Location.IsNeighbor(defendingUnit.Location)) {
            return false;
        }

        // If units are on the same team, do not attack
        if (hexUnit.GetTeam() == defendingUnit.GetTeam()) {
            Debug.Log("These units are on the same team, do not add command.");
            return false;
        }

        return base.ValidateAddCommand(ref commands);
    }
}
