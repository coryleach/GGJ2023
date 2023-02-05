using Mirror;
using UnityEngine;

public class SlowOnHit : MonoBehaviour
{
    [SerializeField]
    private bool aoe = false;

    [SerializeField]
    private float aoeRadius = 1f;

    [SerializeField] private float amount = 0.5f;

    [SerializeField] private float duration = 1f;

    [SerializeField] private bool destroyOnHit = false;

    private bool applied = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.gameObject.GetComponent<EnemyController>();
        Apply(enemy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemy = collision.gameObject.GetComponent<EnemyController>();
        Apply(enemy);
    }

    private readonly Collider2D[] results = new Collider2D[10];

    private void Apply(EnemyController enemy)
    {
        if (applied)
        {
            return;
        }

        applied = true;

        if (enemy != null)
        {
            if (aoe)
            {
                var size = Physics2D.OverlapCircleNonAlloc(transform.position, aoeRadius, results);
                for ( var i = 0; i < size; i++)
                {
                    var result = results[i];
                    var target = result.gameObject.GetComponentInParent<EnemyController>();
                    if (target != null)
                    {
                        target.SetSpeedModifier(amount, duration);
                    }
                }
            }
            else
            {
                enemy.SetSpeedModifier(amount,duration);
            }
        }

        if (destroyOnHit)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (aoe)
        {
            Gizmos.color = new Color(1f, 0, 0, 0.25f);
            Gizmos.DrawSphere(transform.position, aoeRadius);
        }
    }

}
