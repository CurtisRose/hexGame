using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Team { Blue, Red, Black, Green, Brown, Purple, Tan, White };
public class TeamManager : MonoBehaviour {
    static int numTeams = 8;
    public Material[] unitMaterials = new Material[8];

    public Material GetUnitMaterial(Team team) {
        return unitMaterials[(int)team];
    }

    public Team GetTeamColor(int index) {
        return (Team)index;
    }

    public static int GetNumTeams() {
        return numTeams;
    }

    public static Team GetFirstPlayer() {
        return Team.Blue;
    }
}
