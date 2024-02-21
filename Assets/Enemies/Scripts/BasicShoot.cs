using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicShoot : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        // Code from https://www.youtube.com/watch?v=HRX0pUSucW4
        agent = GetComponent<NavMeshAgent>();
        // Sets up agent to be in 2d
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // external code ends
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
