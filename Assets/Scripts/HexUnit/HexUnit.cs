﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour {
	protected const float rotationSpeed = 180f;
	protected const float travelSpeed = 1f;

	// Just setting the default HexUnit to Swordsman type.
	protected UnitType unitType = UnitType.Swordsman;
	protected HealthBar healthBar;

	[SerializeField]
	protected GameObject unitBody;

	public HexGrid Grid { get; set; }

	protected Animator animator;

	public uint Damage { 
		get {
			return damage;
		}
		set {}
	}
	protected uint damage = 50;

	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				Grid.DecreaseVisibility(location, VisionRange, player);
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			Grid.IncreaseVisibility(location, VisionRange, player);
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

	protected HexCell location, currentTravelLocation;

	public float Orientation {
		get {
			return orientation;
		}
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}
	protected float orientation;

	public int Speed {
		get {
			return 24;
		}
	}

	public int VisionRange {
		get {
			return 3;
		}
	}

	public uint Health {
		get {
			return healthBar.Health;
		}
	}

	protected Player player;

	List<HexCell> pathToTravel;

	private void Awake() {
		animator = GetComponentInChildren<Animator>();
		healthBar = gameObject.AddComponent<HealthBar>();
		pathToTravel = new List<HexCell>();
	}

	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	public virtual bool IsValidDestination (HexCell cell) {
		return cell.IsExplored(player) && !cell.IsUnderwater /*&& !cell.Unit*/;
	}

	public void SetPath(List<HexCell> path) {
		pathToTravel = path;
	}

	public void FollowPath(Command command) {
		if (!Dead && pathToTravel.Count > 0) {
			location.Unit = null;
			location = pathToTravel[pathToTravel.Count - 1];
			location.Unit = this;
			StopAllCoroutines();
			StartCoroutine(TravelPath(command));
		} else {
			command.IncrementDeploymentReady();
		}
	}


	// Returns the one turn path and alters the input path to be the remainder of the path
	public List<HexCell> PartitionPath(ref List<HexCell> fullPathToTravel) {
		// If the path to follow only includes the current location, path is invalid, do not follow
		if (fullPathToTravel.Count <= 1) {
			return null;
		}

		List<HexCell> immediatePathToTravel = new List<HexCell>();

		foreach (HexCell hexCell in fullPathToTravel) {
			if (hexCell.Distance < Speed) {
				immediatePathToTravel.Add(hexCell);
			}
		}

		fullPathToTravel.RemoveRange(0, immediatePathToTravel.Count - 1);

		// Seems super messy, but partitioning works by using the distances on each hex.
		// After the first partition the remaining hexes have the old distances
		// So, we have to shift the distances, acting as if it is a new turn
		foreach (HexCell hexCell in fullPathToTravel) {
			hexCell.Distance -= Speed;
		}

		return immediatePathToTravel;
	}

	protected IEnumerator TravelPath (Command command) {
		Vector3 a, b, c = pathToTravel[0].Position;
		animator.SetBool("isIdle", false);
		animator.SetBool("isWalking", true);
		yield return LookAt(pathToTravel[1].Position);
		animator.SetBool("isRunning", true);
		animator.SetBool("isWalking", false);
		if (!currentTravelLocation) {
			currentTravelLocation = pathToTravel[0];
		}
		Grid.DecreaseVisibility(currentTravelLocation, VisionRange, player);
		int currentColumn = currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			if (currentTravelLocation.IsVisible()) {
				MakeVisible();
			} else {
				MakeInvisible();
			}
			a = c;
			b = pathToTravel[i - 1].Position;

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn) {
				if (nextColumn < currentColumn - 1) {
					a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				else if (nextColumn > currentColumn + 1) {
					a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				Grid.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + currentTravelLocation.Position) * 0.5f;
			Grid.IncreaseVisibility(pathToTravel[i], VisionRange, player);

			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
			Grid.DecreaseVisibility(pathToTravel[i], VisionRange, player);
			t -= 1f;
		}
		currentTravelLocation = null;

		a = c;
		b = location.Position;
		c = b;
		Grid.IncreaseVisibility(location, VisionRange, player);
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}

		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;

		animator.SetBool("isIdle", true);
		animator.SetBool("isRunning", false);
		command.IncrementDeploymentReady();
	}

	protected IEnumerator LookAt (Vector3 point) {
		if (HexMetrics.Wrapping) {
			float xDistance = point.x - transform.localPosition.x;
			if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
			else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
		}

		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}

	public virtual int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction)
	{
		if (!IsValidDestination(toCell)) {
			return -1;
		}
		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) {
			return -1;
		}
		int moveCost = 0;
		if(toCell.HasRiver) {
			moveCost = 10;
		}
		if (edgeType == HexEdgeType.Flat) {
			moveCost = 5;
		}
		else {
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
		}
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 3;
		}
		return moveCost;
	}

	public void TakeDamage(uint damageAmount) {
		animator.Play("TakeDamage");
		healthBar.Damage(damageAmount);
	}

	public void Heal(HexUnit enemyUnit) {
		enemyUnit.Heal(damage);
		animator.Play("Attack");
	}

	public void Heal(uint healAmount) {
		healthBar.Heal(healAmount);
	}

	public Player GetPlayer() {
		return player;
	}

	public void SetPlayer(Player player) {
		this.player = player;
		Renderer[] skins = GetComponentsInChildren<Renderer>();
		foreach (Renderer skin in skins) {
			skin.material = TurnManager.GetInstance().GetUnitMaterial(player);
		}
	}

	public virtual void StartAttack(HexCell target, Command command) {
		
	}

	protected virtual IEnumerator Attack(HexCell target, Command command) {
		return null;
	}

	public virtual Command CreateAttackCommand(HexCell target) {
		return new AttackCommand(this, target);
	}

	protected bool Dead = false;
	public void Die () {
		Grid.RemoveUnit(this);
		Grid.ResetVisibility();
		location.Unit = null;
		animator.StopPlayback();
		animator.Play("Die");
		Dead = true;
		Destroy(gameObject, 3.0f);
	}

	public virtual void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
		writer.Write((int)player);
	}

	public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		int playerNumber = reader.ReadInt32();
		/*grid.AddUnit(
			Instantiate(HexMapEditor.GetHexUnitPrefab(UnitType.Swordsman)), grid.GetCell(coordinates), orientation, (Player)playerNumber
		);*/
	}

	protected void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
				Grid.IncreaseVisibility(location, VisionRange, player);
				Grid.DecreaseVisibility(currentTravelLocation, VisionRange, player);
				currentTravelLocation = null;
			}
		}
	}

	public void MakeVisible() {
		unitBody.gameObject.SetActive(true);
	}

	public void MakeInvisible() {
		unitBody.gameObject.SetActive(false);
	}
}