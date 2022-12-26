using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    CharacterStats cs;
    public GameObject deathParticle;

    void Start()
    {
        cs = GetComponent<CharacterStats>();
        cs.onDeath += onDeath;
    }

    private void OnDestroy()
    {
        cs.onDeath -= onDeath;
    }

    void onDeath()
    {
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);
        Destroy(this.gameObject);
    }
}
