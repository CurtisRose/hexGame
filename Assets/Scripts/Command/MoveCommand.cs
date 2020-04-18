using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand {
    private List<HexCell> path;

    public MoveCommand(HexUnit hexUnit, List<HexCell> path) {
        this.hexUnit = hexUnit;
        this.path = path;
    }

    public override void Execute() {
        hexUnit.SetPath(path);
        hexUnit.FollowPath();
    }

    public override bool ValidateAddCommand(ref List<ICommand> commands) {

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
        foreach (MoveCommand otherMoveCommand in commands) {
            if (GetTargetCell() == otherMoveCommand.GetTargetCell()) {
                conflictingCommand = true;
                break;
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
