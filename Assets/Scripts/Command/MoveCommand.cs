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
}
