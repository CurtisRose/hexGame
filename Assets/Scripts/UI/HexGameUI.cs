using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	HexUnit selectedUnit;

	bool endTurn = false;
	bool switchPlayer = false;

	List<List<Command>> commandList = new List<List<Command>>();

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1)) {
					HexCell targetCell = GetCellUnderCursor();
					if (targetCell != null) {
						HexUnit targetUnit = targetCell.Unit;
						if (targetUnit != null ) {
							GiveAttackCommand(targetUnit);
						} else {
							GiveMoveCommand();
						}
					}
				}
				else {
					DoPathfinding();
				}
			}
		}
		if (Input.GetAxis("End Turn") > 0 && endTurn == false) {
			Debug.Log("Ending Turn");
			endTurn = true;
			if (commandList != null && commandList.Count > 0) {
				foreach (Command command in commandList[0]) {
					command.Execute();
				}
				commandList.RemoveAt(0);
			}
		} else if (Input.GetAxis("End Turn") == 0) {
			endTurn = false;
		}
		if (Input.GetAxis("Switch Player") > 0 && switchPlayer == false) {
			switchPlayer = true;
			selectedUnit = null;
			grid.ClearPath();
			TurnManager.SwitchPlayer();
			grid.ResetVisibility();
		} else if (Input.GetAxis("Switch Player") == 0) {
			switchPlayer = false;
		}
	}

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			if (currentCell.Unit && currentCell.Unit.GetPlayer() == TurnManager.GetCurrentPlayer()) {
				selectedUnit = currentCell.Unit;
			}
		}
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
			}
			else {
				grid.ClearPath();
			}
		}
	}

	void CleanOutCommands() {
		// Shitty Code, but, if the unit attributed to a command has died, remove all of it's commands.
		List<Command> commandsToReplace = new List<Command>();
		foreach (List<Command> commands in commandList) {
			for (int i = 0; i < commands.Count; i++) {
				if (commands[i].GetHexUnit() == null) {
					commands.RemoveAt(i);
				}
			}
		}
	}

	void GiveMoveCommand() {
		CleanOutCommands();
		if (grid.HasPath) {
			//selectedUnit.SetPath(grid.GetPath());
			List<HexCell> fullPath = grid.GetPath();
			int turnNumber = 0;

			while (fullPath.Count > 1) {
				List<HexCell> partialPath = selectedUnit.PartitionPath(ref fullPath);
				MoveCommand moveCommand = new MoveCommand(selectedUnit, partialPath);
				List<Command> commands;
				if (commandList != null && commandList.Count < (turnNumber+1)) {
					commandList.Add(new List<Command>());
					commands = new List<Command>();
				} else {
					commands = commandList[turnNumber];
				}
				bool goodCommand = moveCommand.ValidateAddCommand(ref commands);

				if (goodCommand) {
					commandList[turnNumber] = commands;
				} else {
					// Invalid command, don't continue.
					return;
				}
				turnNumber++;
			}
		}
	}

	void GiveAttackCommand(HexUnit targetUnit) {
		Debug.Log("Attempting to Give Attack Command");
		CleanOutCommands();
		List<Command> commands;
		if (commandList != null && commandList.Count == 0) {
			commandList.Add(new List<Command>());
			commands = new List<Command>();
		} else {

			commands = commandList[0];
		}
		AttackCommand attackCommand = new AttackCommand(selectedUnit, targetUnit);
		bool goodCommand = attackCommand.ValidateAddCommand(ref commands);

		if (goodCommand) {
			commandList[0] = commands;
		} else {
			return;
		}
	}

	HexCell GetCellUnderCursor() {
		return grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}