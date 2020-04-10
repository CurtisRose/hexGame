using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {
    static int numTeams = 8;
    public enum TeamColor { Blue, Red, Black, Green, Brown, Purple, Tan, White };
    public Material[] unitMaterials = new Material[8];

    public Material GetUnitMaterial(TeamColor teamColor) {
        return unitMaterials[(int)teamColor];
    }

    public TeamColor GetTeamColor(int index) {
        return (TeamColor)index;
    }

    public static int GetNumTeams() {
        return numTeams;
    }
}
