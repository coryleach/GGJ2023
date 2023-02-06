using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab;

    [SerializeField]
    private PathNode[] sourcePathNodes;

    [SerializeField]
    private float startDelay = 5f;

    [SerializeField]
    private float spawnInterval = 1f;

    [SerializeField]
    private int maxSpawns = 20;

    private float t = 0;

    private float delay = 0;

    private int spawnCount = 0;

    private List<EnemyController> CurrentEnemies = new List<EnemyController>();

    private List<RootsController> Trees = new List<RootsController>();

    private void Start()
    {
        delay = startDelay;
    }

    private void Update()
    {
        if (spawnCount >= maxSpawns)
        {
            return;
        }

        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        t += Time.deltaTime;
        if (t >= spawnInterval)
        {
            t -= spawnInterval;
            Spawn();
        }
    }

    [ServerCallback]
    private void Spawn()
    {
        //Only spawn enemies if the Network Server is up and running
        if (NetworkServer.active)
        {
            var firstPathNode = sourcePathNodes[Random.Range(0,sourcePathNodes.Length)];
            var spawnedEnemy = Instantiate(enemyPrefab, firstPathNode.Position, Quaternion.identity);
            NetworkServer.Spawn(spawnedEnemy.gameObject);
            var controller = spawnedEnemy.GetComponent<EnemyController>();
            spawnedEnemy.SetPath(firstPathNode);
            spawnedEnemy.OnDestroyed.AddListener(OnEnemyDestroyed);
            spawnCount++;
            CurrentEnemies.Add(spawnedEnemy);
            foreach(RootsController tree in Trees)
            {
                spawnedEnemy.OnDestroyed.AddListener(tree.OnEnemyDestroyed);
            }
        }
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        EnemyController.DeadMushrooms++;
        spawnCount--;
        CurrentEnemies.Remove(enemy);
    }

    public void RegisterTree(RootsController tree)
    {
        Trees.Add(tree);
        foreach(EnemyController enemy in CurrentEnemies)
        {
            enemy.OnDestroyed.AddListener(tree.OnEnemyDestroyed);
        }
    }

}
