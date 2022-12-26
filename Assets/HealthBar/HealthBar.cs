using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStats))]
public class HealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Transform target;

    private Canvas worldSpaceCanvas;
    private CharacterStats cs;
    private Transform healthUi;
    private Image healthSlider;

    private void Start()
    {
        worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas").GetComponent<Canvas>();
        healthUi = Instantiate(healthBarPrefab, worldSpaceCanvas.transform).transform;
        healthSlider = healthUi.GetChild(0).GetComponent<Image>();
        cs = GetComponent<CharacterStats>();
        cs.onHealthChanged += onHealthChanged;
        cs.onDeath += onDeath;
    }

    private void OnDestroy()
    {
        if (healthUi) Destroy(healthUi.gameObject);
    }

    private void LateUpdate()
    {
        if (!healthUi) return;
        healthUi.position = target.position;
        healthUi.forward = Camera.main.transform.forward;
    }

    private void onHealthChanged(int previous, int current, int max, GameObject by)
    {
        if (!healthUi) healthUi = Instantiate(healthBarPrefab, worldSpaceCanvas.transform).transform;
        if (current == 0) return;
        healthUi.gameObject.SetActive(true);
        healthSlider.fillAmount = current / (float) max;
    }

    private void onDeath()
    {
        healthUi.gameObject.SetActive(false);
    }

}
