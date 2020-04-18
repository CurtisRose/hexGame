using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command {
    private List<HexCell> path;

    public MoveCommand(HexUnit hexUnit, List<HexCell> path) {
        this.hexUnit = hexUnit;
        this.path = path;
    }

    public override void ExecuteDeploy2() {
        hexUnit.SetPath(path);
        hexUnit.FollowPath(this);
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {

        // If for some reason the unit doesn't exist, do not add command.
        if (hexUnit == null) {
            return false;
        }

        // If the location you are moving to is already occupied, don't add command
        if (path[path.Count - 1].Unit != null) {
            return false;
        }

        // If the location you are moving to will be occupied next turn, remove old command, add this one
        bool conflictingCommand = false;
        foreach (Command otherCommand in commands) {
            if (otherCommand is MoveCommand) {
                if (GetTargetCell() == ((MoveCommand)otherCommand).GetTargetCell()) {
                    conflictingCommand = true;
                    break;
                }
            }
        }
        if (conflictingCommand) {
            return false;
        }
        return base.ValidateAddCommand(ref commands);
    }

    public HexCell GetTargetCell() {
        return path[path.Count - 1];
    }
}
