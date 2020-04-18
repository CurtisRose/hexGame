using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    protected HexUnit hexUnit;
    protected static int readyToDeploy = 0;

    public virtual void ExecuteDeploy1() {
        IncrementDeploymentReady();
    }

    public virtual void ExecuteDeploy2() {
        IncrementDeploymentReady();
    }

    public virtual void ExecuteDeploy3() {
        IncrementDeploymentReady();
    }

    public virtual bool ValidateAddCommand(ref List<Command> commands) {
        commands.Add(this);
        return true;
    }
    public virtual HexUnit GetHexUnit() {
        return hexUnit;
    }

    public static void ResetDeploymentReady() {
        readyToDeploy = 0;
    }

    public void IncrementDeploymentReady() {
        readyToDeploy++;
    }

    public static int GetDeploymentReady() {
        return readyToDeploy;
    }
}
