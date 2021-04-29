using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleEnemy : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private float speed = 3f;

    private void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }
}
