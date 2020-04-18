using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    HexUnit defendingUnit;

    public AttackCommand(HexUnit attackingUnit, HexUnit defendingUnit) {

        this.hexUnit = attackingUnit;
        this.defendingUnit = defendingUnit;
    }

    public override void Execute() {
        hexUnit.StartAttack(defendingUnit);
    }

    public override void Undo() {
        hexUnit.Heal(defendingUnit);
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {

        // If for some reason the unit doesn't exist or the defending unit, do not add command.
        if (hexUnit == null || defendingUnit == null) {
            Debug.Log("This Unit doesn't exist (or the defending unit), do not add command.");
            return false;
        }

        bool areNeighbors = false;
        foreach(HexCell hexCell in hexUnit.Location.GetNeighbors()) {
            if (hexCell == defendingUnit.Location) {
                areNeighbors = true;
                break;
            }
        }
        if (!areNeighbors) {
            Debug.Log("These units are not next to eachother, do not add command.");
            return false;
        }

        // If units are on the same team, do not attack
        if (hexUnit.GetPlayer() == defendingUnit.GetPlayer()) {
            Debug.Log("These units are on the same team, do not add command.");
            return false;
        }

        // If command has already been given to this unit, replace the command with this command
        for (int i = 0; i < commands.Count; i++) {
            if (commands[i].GetHexUnit() == hexUnit) {
                commands.RemoveAt(i);
            }
        }

        commands.Add(this);
        return true;
    }
}
