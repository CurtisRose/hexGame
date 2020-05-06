using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Swordsman : HexUnit {
	public new int Speed {
		get {
			return 24;
		}
	}

	public new int VisionRange {
		get {
			return 3;
		}
	}

	public override bool IsValidDestination (HexCell cell) {
		return cell.IsExplored(player) && !cell.IsUnderwater /*&& !cell.Unit*/;
	}

	public override int GetMoveCost (
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

	public override void StartAttack(HexCell target, Command command) {
		StopAllCoroutines();
		StartCoroutine(Attack(target, command));
	}

	protected override IEnumerator Attack(HexCell target, Command command) {
		animator.SetBool("isIdle", false);
		animator.SetBool("isWalking", true);
		yield return LookAt(target.Position);
		animator.SetBool("isIdle", true);
		animator.SetBool("isWalking", false);
		yield return animator.IsInTransition(0);

		if (target.Unit != null) {
			target.Unit.TakeDamage(damage);
		}
		animator.Play("Attack");

		command.IncrementDeploymentReady();
	}

	public override Command CreateAttackCommand(HexCell target) {
		return new MeleeAttackCommand(this, target);
	}

	public override void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
		writer.Write((int)player);
	}
}