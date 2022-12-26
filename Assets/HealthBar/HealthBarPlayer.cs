using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStats))]
public class HealthBarPlayer : MonoBehaviour
{
    public Image healthSlider;
    public Image shieldSlider;
    private CharacterStats cs;
    // Start is called before the first frame update
    void Start()
    {
        cs = GetComponent<CharacterStats>();
        cs.onHealthChanged += onHealthChanged;
        cs.onShieldChange += onShieldChange;
    }

    void onShieldChange(int previous, int current, int max, GameObject by)
    {
        if (!this.enabled) return;
        shieldSlider.fillAmount = current / (float) cs.maxHealth;
    }

    void onHealthChanged(int previous, int current, int max, GameObject by)
    {
        if (!this.enabled) return;
        healthSlider.fillAmount = current / (float) max;
    }
}
