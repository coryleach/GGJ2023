using System;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField]
    private int amount = 1;

    private void OnCollisionEnter2D(Collision2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {

        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {

        }
        Destroy(gameObject);
    }
}
