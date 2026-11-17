using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnHealthChange;
    public event EventHandler OnDead;
    private int healthPoint = 100;
    private int maxHealthPoint = 100;

    public float GetHealthPointNormalized() => (float)healthPoint / maxHealthPoint;

    public void Damage(int damageAmount)
    {
        healthPoint -= damageAmount;

        OnHealthChange?.Invoke(this, EventArgs.Empty);

        if (healthPoint <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }
}
