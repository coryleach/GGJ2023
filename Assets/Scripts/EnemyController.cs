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

    
    private PathNode currentNode = null;

    [SyncVar]
    public Vector3 currentNodePosition = Vector3.zero;

    public EnemyEvent OnDestroyed => new EnemyEvent();

    private void OnDestroy()
    {
        OnDestroyed.Invoke(this);
    }

    public void SetPath(PathNode node)
    {
        currentNode = node;
        currentNodePosition = currentNode.Position;
    }

    [ServerCallback]
    private void Update()
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

    
    private void FixedUpdate()
    {
        if (currentNode == null)
        {
            //Debug.Log("Node is null!");
            //return;
        }

        var dir = currentNodePosition - transform.position;
        _rigidbody2D.velocity = dir.normalized * moveSpeed;
    }

}
