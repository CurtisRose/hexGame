using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public static TurnManager instance;
    private static Team currentPlayer = TeamManager.GetFirstPlayer();
    
    public static TurnManager GetInstance() {
        if (instance == null) {
            instance = new TurnManager();
        }
        return instance;
    }

    public static void SwitchPlayer() {
        currentPlayer++;
        if ((int)currentPlayer >= TeamManager.GetNumTeams()) {
            currentPlayer = TeamManager.GetFirstPlayer();
        }
        Debug.Log("Switching to team " + currentPlayer.ToString());
    }

    public static Team GetCurrentPlayer() {
        return currentPlayer;
    }
}
