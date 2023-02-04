using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float nextNodeDistance = 0.1f;

    [SerializeField]
    private Rigidbody2D _rigidbody2D = null;

    private PathNode currentNode = null;

    public void SetPath(PathNode node)
    {
        currentNode = node;
    }

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
        }
    }

    private void FixedUpdate()
    {
        if (currentNode == null)
        {
            return;
        }

        var dir = currentNode.Position - transform.position;
        _rigidbody2D.velocity = dir.normalized * moveSpeed;
    }

}
