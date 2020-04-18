using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICommand
{
    protected HexUnit hexUnit;
    public abstract void Execute();
    public virtual bool ValidateAddCommand(ref List<ICommand> commands) {
        commands.Add(this);
        return true;
    }
    public virtual HexUnit GetHexUnit() {
        return hexUnit;
    }
}
