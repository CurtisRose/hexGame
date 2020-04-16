using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected HexUnit hexUnit;
    public abstract void Execute();
    public abstract void Undo();
    public abstract bool ValidateAddCommand(ref List<Command> commands);

    public HexUnit GetHexUnit() {
        return hexUnit;
    }
}
