using System;
using UnityEngine;
using Mirror;

public class Damager : NetworkBehaviour
{
    [SerializeField]
    private int amount = 1;

    private void OnCollisionEnter2D(Collision2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.GetHit(amount);
        }
        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.GetHit(amount);
        }
        NetworkServer.Destroy(gameObject);
    }
}
