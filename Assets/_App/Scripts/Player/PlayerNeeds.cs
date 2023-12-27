using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerNeeds : MonoBehaviour, IDamageable
{
	public Need health;
	public Need hunger;
	public Need thirst;
	public Need sleep;

	// How much does our health decay when hunger and thirst are empty
	public float noHungerHealthDecay;
	public float noThirstHealthDecay;

	public UnityEvent onTakeDamage;

	void Start()
	{
		health.curValue = health.startValue;
		hunger.curValue = hunger.startValue;
		thirst.curValue = thirst.startValue;
		sleep.curValue = sleep.startValue;
	}

	void Update()
	{
		// Decay/regen over time
		hunger.Subtract(hunger.decayRate * Time.deltaTime);
		thirst.Subtract(thirst.decayRate * Time.deltaTime);
		sleep.Add(sleep.regenRate * Time.deltaTime);

		if (hunger.curValue == 0f)
		{
			health.Subtract(noHungerHealthDecay * Time.deltaTime);
		}
		if (thirst.curValue == 0f)
		{
			health.Subtract(noThirstHealthDecay * Time.deltaTime);
		}

		// Check if player is dead
		if (health.curValue == 0f)
		{
			Die();
		}

		// Update UI bars
		health.uiBar.fillAmount = health.GetPercentage();
		hunger.uiBar.fillAmount = hunger.GetPercentage();
		thirst.uiBar.fillAmount = thirst.GetPercentage();
		sleep.uiBar.fillAmount = sleep.GetPercentage();
	}

	public void Heal(float amount)
	{
		health.Add(amount);
	}

	public void Eat(float amount)
	{
		health.Add(amount);
	}

	public void Drink(float amount)
	{
		health.Add(amount);
	}

	public void Sleep(float amount)
	{
		health.Subtract(amount);
	}

	public void TakePhysicalDamage(int amount)
	{
		health.Subtract(amount);
		onTakeDamage?.Invoke();
	}

	public void Die()
	{
		Debug.Log("Player is dead!");
	}
}

[System.Serializable]
public class Need
{
	[HideInInspector]
	public float curValue;
	public float maxValue;
	public float startValue;
	public float regenRate;
	public float decayRate;
	public Image uiBar;

	public void Add(float amount)
	{
		curValue = Mathf.Min(curValue + amount, maxValue);
	}

	public void Subtract(float amount)
	{
		curValue = Mathf.Max(curValue - amount, 0f);
	}

	public float GetPercentage()
	{
		return curValue / maxValue;
	}
}

public interface IDamageable
{
	void TakePhysicalDamage(int damageAmount);
}