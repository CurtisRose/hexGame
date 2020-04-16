using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    private List<HexCell> path;
    private int turnNumber;

    public MoveCommand(HexUnit hexUnit, List<HexCell> path) {
        this.hexUnit = hexUnit;
        this.path = path;
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {

        // If for some reason the unit doesn't exist, do not add command.
        if (hexUnit == null) {
            Debug.Log("This Unit doesn't exist, do not add command.");
            return false;
        }

        // If the location you are moving to is already occupied, don't add command
        if (path[path.Count - 1].Unit != null) {
            Debug.Log("Target Location Occupied");
            return false;
        }

        // If command has already been given to this unit, replace the command with this command
        Command commandToReplace = null;
        foreach (Command otherMoveCommand in commands) {
            if (otherMoveCommand.GetType() == typeof(MoveCommand)) {
                if (otherMoveCommand.GetHexUnit() == this.hexUnit) {
                    commandToReplace = otherMoveCommand;
                    break;
                }
            }
        }
        if (commandToReplace != null) {
            Debug.Log("Command has already been issued");
            commands.Remove(commandToReplace);
            commandToReplace = null;
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
            Debug.Log("Command interferes with earlier command");
            return false;
        }
        commands.Add(this);
        return true;
    }

    public override void Execute() {
        hexUnit.SetPath(path);
        hexUnit.FollowPath();
    }

    public override void Undo() {

    }

    public HexCell GetTargetCell() {
        return path[path.Count - 1];
    }
}
