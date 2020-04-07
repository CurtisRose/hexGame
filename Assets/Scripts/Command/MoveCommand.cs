using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    private HexUnit hexUnit;
    List<HexCell> path;

    public MoveCommand(HexUnit hexUnit, List<HexCell> path) {
        this.hexUnit = hexUnit;
        this.path = path;
    }

    public override void Execute() {
        hexUnit.SetPath(path);
        hexUnit.FollowPath();
    }

    public override void Undo() {

    }
}
