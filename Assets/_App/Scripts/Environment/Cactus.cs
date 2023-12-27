using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
	public int damage;
	public float damageRate;

	private List<IDamageable> thingsToDamage = new List<IDamageable>();

	void Start()
	{
		StartCoroutine(DealDamage());
	}

	IEnumerator DealDamage()
	{
		while (true)
		{
			for (int i = 0; i < thingsToDamage.Count; i++)
			{
				thingsToDamage[i].TakePhysicalDamage(damage);
			}
			yield return new WaitForSeconds(damageRate);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<IDamageable>() != null)
		{
			thingsToDamage.Add(other.gameObject.GetComponent<IDamageable>());
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.GetComponent<IDamageable>() != null)
		{
			thingsToDamage.Remove(other.gameObject.GetComponent<IDamageable>());
		}
	}
}
