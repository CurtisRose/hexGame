using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamColor { Blue, Red, Black, Green, Brown, Purple, Tan, White };
public class TurnManager
{
    public static TurnManager instance;
    private static TeamColor currentPlayer = TurnManager.GetFirstPlayer();
    static int numTeams = 8;
    static Material[] unitMaterials = new Material[8];

    public static TurnManager GetInstance() {
        if (instance == null) {
            instance = new TurnManager();
        }
        return instance;
    }

    public static void SwitchPlayer() {
        currentPlayer++;
        if ((int)currentPlayer >= TurnManager.GetNumTeams()) {
            currentPlayer = TurnManager.GetFirstPlayer();
        }
    }

    public static TeamColor GetCurrentPlayer() {
        return currentPlayer;
    }

    public Material GetUnitMaterial(TeamColor team) {
        return unitMaterials[(int)team];
    }

    public TeamColor GetTeamColor(int index) {
        return (TeamColor)index;
    }

    public static int GetNumTeams() {
        return numTeams;
    }

    public static TeamColor GetFirstPlayer() {
        return TeamColor.Blue;
    }

    public static void LoadMaterials() {
        for (int i = 0; i < numTeams; i++) {
            TeamColor team = (TeamColor)i;
            unitMaterials[i] = Resources.Load("TeamColors/WK_Standard_Units_"+team.ToString().ToLower(), typeof(Material)) as Material;
        }
    }
}
