using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public SlimeEnemy enemySlime;
    [SerializeField] private List<SlimeEnemy> slimeEnemies;

    [SerializeField] public TurtleEnemy enemyTurtle;
    [SerializeField] private List<TurtleEnemy> turtleEnemies;

    [SerializeField] public FlyingEnemy flyingEnemy;
    [SerializeField] private List<FlyingEnemy> flyingEnemies;

    [Range (0,200)]
    private int numEnemies = 5;
    private float range = 200;

    // Start is called before the first frame update
    void Start()
    {
        slimeEnemies = new List<SlimeEnemy>();
        turtleEnemies = new List<TurtleEnemy>();
        flyingEnemies = new List<FlyingEnemy>();

        // spawns numEnemy amount of each enemy
        for( int i = 0; i < numEnemies; i++ )
        {
            SlimeEnemy spawnedSlimeEnemy = Instantiate(enemySlime, RandomNavmeshLocation(range), Quaternion.identity) as SlimeEnemy;
            slimeEnemies.Add(spawnedSlimeEnemy);
            TurtleEnemy spawnedTurtleEnemy = Instantiate(enemyTurtle, RandomNavmeshLocation(range), Quaternion.identity) as TurtleEnemy;
            turtleEnemies.Add(spawnedTurtleEnemy);
            FlyingEnemy spawnedFlyingEnemy = Instantiate(flyingEnemy, RandomNavmeshLocation(range), Quaternion.identity) as FlyingEnemy;
            flyingEnemies.Add(spawnedFlyingEnemy);
        }
    }

    // Random location on a navigation mesh
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if( UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, 1) )
            finalPosition = hit.position;
        return finalPosition;
    }

}
