using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AttackCommand : Command
{
    protected HexCell target;
 
    public AttackCommand(HexUnit attackingUnit, HexCell target) {
        this.hexUnit = attackingUnit;
        this.target = target;
    }

    public override void ExecuteDeploy1() {
        hexUnit.StartAttack(target, this);
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {
        // If for some reason the unit doesn't exist or the defending unit, do not add command.
        if (hexUnit == null || target == null || target.Unit == null) {
            Debug.Log("This Unit doesn't exist (or the defending unit), do not add command.");
            return false;
        }

        // If command has already been given to this unit, replace the command with this command
        for (int i = 0; i < commands.Count; i++) {
            if (commands[i].GetHexUnit() == hexUnit) {
                commands.RemoveAt(i);
            }
        }

        return base.ValidateAddCommand(ref commands);
    }
}
