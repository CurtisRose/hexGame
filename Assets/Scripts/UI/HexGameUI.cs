using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	HexUnit selectedUnit;

	bool endTurn = false;
	bool switchPlayer = false;
	[SerializeField]
	bool executingCommands = false;

	List<List<Command>> commandList = new List<List<Command>>();

	[SerializeField]
	Image nextPlayerButtonImage;
	[SerializeField]
	Image endTurnButtonImage;
	[SerializeField]
	Text whoseTurnText;

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

	private void Start() {
		whoseTurnText.text = TurnManager.GetCurrentPlayer().ToString() + "'s Turn";
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
							GiveAttackCommand(targetCell);
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
	}

	public void EndTurn() {
		if (!executingCommands) {
			if (commandList != null && commandList.Count > 0) {
				Debug.Log("Executing Orders");
				executingCommands = true;
				StartCoroutine(ExecuteCommands());
				StopPathFinding();
			}
		}
	}

	public void Switchplayer() {
		if (!executingCommands) {
			StopPathFinding();
			TurnManager.SwitchPlayer();
			grid.ResetVisibility();
			whoseTurnText.text = TurnManager.GetCurrentPlayer().ToString() + "'s Turn";
		}
	}

	void StopPathFinding() {
		selectedUnit = null;
		grid.ClearPath();
	}

	private IEnumerator ExecuteCommands() {
		// Executing Deployment 1 Orders
		foreach (Command command in commandList[0]) {
			command.ExecuteDeploy1();
		}
		while(Command.GetDeploymentReady() != commandList[0].Count) {
			yield return null;
		}
		Command.ResetDeploymentReady();


		// Executing Deployment 2 Orders
		yield return new WaitForSeconds(0.5f);
		foreach (Command command in commandList[0]) {
			command.ExecuteDeploy2();
		}
		while (Command.GetDeploymentReady() != commandList[0].Count) {
			yield return null;
		}
		Command.ResetDeploymentReady();

		// Executing Deployment 3 Orders
		yield return new WaitForSeconds(0.5f);
		foreach (Command command in commandList[0]) {
			command.ExecuteDeploy3();
		}
		while (Command.GetDeploymentReady() != commandList[0].Count) {
			yield return null;
		}
		Command.ResetDeploymentReady();

		// Deleting commands list
		commandList.RemoveAt(0);
		// Letting the GUI know the this commands list has finished executing
		executingCommands = false;
	}

	void DoSelection () {
		if (grid == null)
		{
			if (GameObject.FindGameObjectWithTag("HexGrid") != null)
			{
				grid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();
			}
			else
			{
				return;
			}
		}
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			if (currentCell.Unit && currentCell.Unit.GetPlayer() == TurnManager.GetCurrentPlayer()) {
				selectedUnit = currentCell.Unit;
			} else {
				StopPathFinding();
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

	/// <summary>
	/// TODO: Create a GiveCommand functionality that allows units to define which command is to be created
	/// based on the input arguments. For example if a cell does not have a unit on it, a swordsman would know 
	/// to move to that location. In that same example, though, an archer might want to move OR shoot.
	/// Not quite sure how to consolidate that idea. But, if a cell has a unit on it, then both archers
	/// and swordsman would choose to make an attack order. Unless the tile is not a neighbor, then, swordsman
	/// might make a charge command (this doesn't exist yet, just an idea).
	/// </summary>
	void GiveCommand() {
		CleanOutCommands();
		//selectedUnit.CreateCommand();
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
			StopPathFinding();
		}
	}

	void GiveAttackCommand(HexCell target) {
		CleanOutCommands();
		List<Command> commands;
		if (commandList != null && commandList.Count == 0) {
			commandList.Add(new List<Command>());
			commands = new List<Command>();
		} else {

			commands = commandList[0];
		}
		Command attackCommand = selectedUnit.CreateAttackCommand(target);
		bool goodCommand = attackCommand.ValidateAddCommand(ref commands);
		
		StopPathFinding();
		
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
		if (Camera.main == null)
		{
			return false;
		}
		if (grid == null)
		{
			return false;
		}
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}