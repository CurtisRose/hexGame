using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	HexUnit selectedUnit;

	bool endTurn = false;

	List<Command> commands = new List<Command>();

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
					SetPath();
				}
				else {
					DoPathfinding();
				}
			}
		}
		if (Input.GetAxis("End Turn") > 0 && endTurn == false) {
			endTurn = true;
			foreach(Command command in commands) {
				command.Execute();
			}
			commands.Clear();
			/*foreach(HexUnit hexUnit in grid.GetUnits()) {
				hexUnit.FollowPath();
			}*/
		} else if (Input.GetAxis("End Turn") == 0) {
			endTurn = false;
		}
	}

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
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

	void SetPath() {
		if (grid.HasPath) {
			//selectedUnit.SetPath(grid.GetPath());
			commands.Add(new MoveCommand(selectedUnit, grid.GetPath()));
			//grid.ClearPath();
		}
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