using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int maxShield = 50;
    public int currentHealth;
    public int shieldHealth { get; private set; }
    public bool isDead = false;

    public event System.Action<int, int, int, GameObject> onHealthChanged;
    public event System.Action<int, int, int, GameObject> onShieldChange;
    public event System.Action<int, int, int, GameObject> onDamage;
    public event System.Action onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void damage(int damage, GameObject by)
    {
        if (isDead) return;
        TakeDamage(damage, by);
    }

    protected void TakeDamage(int damage, GameObject by)
    {
        int previousHealth = currentHealth;
        int previousShield = shieldHealth;

        // calculate new shield health
        shieldHealth = Mathf.Max(previousShield - damage, 0);

        // calculate damage
        damage = Mathf.Max(damage - previousShield, 0);
        currentHealth -= damage;

        if (onHealthChanged != null && previousHealth != currentHealth) onHealthChanged(previousHealth, currentHealth, maxHealth, by);
        if (onDamage != null && currentHealth < previousHealth) onDamage(previousHealth, currentHealth, maxHealth, by);
        if (onShieldChange != null && previousShield != shieldHealth) onShieldChange(previousShield, shieldHealth, maxShield, by);
        if (currentHealth <= 0)
        {
            die();
            this.isDead = true;
            this.currentHealth = 0;
        }
    }

    public void resetHealth()
    {
        addHealth(maxHealth);
    }

    public void addMaxHealth(int amount)
    {
        this.maxHealth += amount;
        addHealth(this.maxHealth);
    }

    public void addHealth(int health)
    {
        int previous = currentHealth;
        this.currentHealth = Mathf.Min(this.currentHealth + health, maxHealth);
        this.isDead = false;
        if (onHealthChanged != null) onHealthChanged(previous, currentHealth, maxHealth, null);
    }

    public void addShield(int amount)
    {
        int previous = this.shieldHealth;
        this.shieldHealth = Mathf.Min(this.shieldHealth + amount, this.maxShield);
        onShieldChange(previous, shieldHealth, maxShield, null);
    }

    protected virtual void die()
    {
        if (onDeath != null) onDeath();
    }
}
