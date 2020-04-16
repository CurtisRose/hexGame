using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	private HexUnit hexUnit;
	public uint Health {
		get {
			return health;
		}
	}
	[SerializeField]
	uint health;
	uint maxHealth = 100;

	private void Awake() {
		health = maxHealth;
		hexUnit = GetComponent<HexUnit>();
	}

	public void Damage(uint damageAmount) {
		if (health <= damageAmount) {
			health = 0;
			hexUnit.Die();
		}
		else {
			health -= damageAmount;
		}
	}

	public void Heal(uint healAmount) {
		health += healAmount;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}
}
