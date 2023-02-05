using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

[Serializable]
public class EnemyEvent : UnityEvent<EnemyController> {}

public class EnemyController : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float nextNodeDistance = 0.1f;

    [SerializeField]
    private Rigidbody2D _rigidbody2D = null;

    [SerializeField]
    private Animator anim = null;

    private PathNode currentNode = null;

    [SyncVar]
    public Vector3 currentNodePosition = Vector3.zero;

    public int CurrentHealth = 10;

    public EnemyEvent OnDestroyed { get; } = new EnemyEvent();

    private void OnDestroy()
    {
        OnDestroyed.Invoke(this);
    }

    [ServerCallback]
    public void GetHit(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    public void SetPath(PathNode node)
    {
        currentNode = node;
        currentNodePosition = currentNode.Position;
    }


    private void Update()
    {
        if (isServer)
        {
            if (currentNode == null)
            {
                return;
            }

            var distSqrd = (currentNode.Position - transform.position).sqrMagnitude;
            if (distSqrd <= (nextNodeDistance * nextNodeDistance))
            {
                currentNode = currentNode.NextNode();
                currentNodePosition = currentNode.Position;
            }
        }
        if(anim != null)
        {
            anim.SetInteger("WalkCycle", (int)(UnityEngine.Random.value * 2.9999f));
        }
    }

    private void FixedUpdate()
    {
        var dir = currentNodePosition - transform.position;
        _rigidbody2D.velocity = dir.normalized * moveSpeed;
    }

}
