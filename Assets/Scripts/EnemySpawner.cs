using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab;

    [SerializeField]
    private PathNode firstPathNode;

    [SerializeField]
    private float startDelay = 5f;

    [SerializeField]
    private float spawnInterval = 1f;

    private float t = 0;

    private float delay = 0;

    private void Start()
    {
        delay = startDelay;
    }

    private void Update()
    {
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

    private void Spawn()
    {
        var spawnedEnemy = Instantiate(enemyPrefab, firstPathNode.Position, Quaternion.identity);
        spawnedEnemy.SetPath(firstPathNode);
    }

}
