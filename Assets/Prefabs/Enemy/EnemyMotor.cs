using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private Rigidbody rb;
    public new bool enabled { get { return base.enabled; } set { base.enabled = value; this.agent.enabled = value; } }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
            Quaternion lerpRot = Quaternion.Lerp(transform.rotation, targetRot, .05f);

            transform.rotation = lerpRot;
        }
    }

    public bool isReady()
    {
        return agent != null;
    }

    public void move(Vector3 loc)
    {
        if (!agent.enabled) return;
        agent.isStopped = false;
        agent.destination = loc;
    }
    public void stop()
    {
        if (!agent.enabled) return;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }

    public Transform getTarget()
    {
        return this.target;
    }
}
