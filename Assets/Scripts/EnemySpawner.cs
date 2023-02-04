using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab;

    [SerializeField]
    private PathNode firstPathNode;

    [SerializeField]
    private float startDelay = 5f;

    [SerializeField]
    private float spawnInterval = 1f;

    [SerializeField]
    private int maxSpawns = 20;

    private float t = 0;

    private float delay = 0;

    [SerializeField]
    private int spawnCount = 0;

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
            var spawnedEnemy = Instantiate(enemyPrefab, firstPathNode.Position, Quaternion.identity);
            NetworkServer.Spawn(spawnedEnemy.gameObject);
            var controller = spawnedEnemy.GetComponent<EnemyController>();
            spawnedEnemy.SetPath(firstPathNode);
            spawnedEnemy.OnDestroyed.AddListener(OnEnemyDestroyed);
            spawnCount++;
        }
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        spawnCount--;
    }

}
