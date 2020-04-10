using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    private HexUnit hexUnit;
    private List<HexCell> path;
    private int turnNumber;

    public MoveCommand(HexUnit hexUnit, List<HexCell> path) {

        this.hexUnit = hexUnit;
        this.path = path;
    }

    public override bool ValidateAddCommand(ref List<Command> commands) {
        Debug.Log("Starting Command Validation");

        // If the location you are moving to is already occupied, don't add command
        if (path[path.Count - 1].Unit != null) {
            Debug.Log("Target Location Occupied");
            return false;
        }

        // If command has already been given to this unit, replace the command with this command
        Command commandToReplace = null;
        foreach (MoveCommand otherMoveCommand in commands) {
            if (otherMoveCommand.GetHexUnit() == this.hexUnit) {
                commandToReplace = otherMoveCommand;
                break;
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

    public HexUnit GetHexUnit() {
        return hexUnit;
    }

    public HexCell GetTargetCell() {
        return path[path.Count - 1];
    }
}
