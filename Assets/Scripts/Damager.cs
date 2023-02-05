using UnityEngine;
using Mirror;

public class Damager : NetworkBehaviour
{
    [SerializeField] private bool aoe = false;

    [SerializeField] private float aoeRadius = 1f;

    [SerializeField] private int amount = 1;

    [SerializeField] private bool destroyOnHit = true;

    [SerializeField] private bool applyOnlyOnce = true;

    private bool applied = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        Apply(enemy);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.gameObject.GetComponent<EnemyController>();
        Apply(enemy);
    }

    private readonly Collider2D[] results = new Collider2D[10];

    private void Apply(EnemyController enemy)
    {
        if (applied && applyOnlyOnce)
        {
            return;
        }

        applied = true;

        if (enemy != null)
        {
            if (aoe)
            {
                var size = Physics2D.OverlapCircleNonAlloc(transform.position, aoeRadius, results);
                for (var i = 0; i < size; i++)
                {
                    var result = results[i];
                    var target = result.gameObject.GetComponentInParent<EnemyController>();
                    if (target != null)
                    {
                        target.GetHit(amount);
                    }
                }
            }
            else
            {
                enemy.GetHit(amount);
            }
        }

        if (destroyOnHit)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
