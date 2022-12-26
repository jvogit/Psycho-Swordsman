using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDrop : MonoBehaviour
{
    public int chance;
    public GameObject drop;

    CharacterStats cs;

    private void Start()
    {
        cs = GetComponent<CharacterStats>();

        cs.onDeath += onDeath;
    }

    public void onDeath()
    {
        if (Random.Range(0, 100) <= chance)
        {
            Instantiate(drop, transform.position + Vector3.up * 1, Quaternion.identity);
        }
    }
}
