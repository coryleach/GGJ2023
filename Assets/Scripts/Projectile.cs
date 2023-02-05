using System;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    protected float speed = 2f;

    [SerializeField]
    protected Rigidbody2D _rigidbody2D = null;

    [SerializeField]
    protected float lifetime = 1f;

    [SerializeField]
    protected AudioSource Audio;

    [SerializeField] private GameObject clientSpawnOnDestroy;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    [SyncVar]
    public Vector2 Direction = Vector2.zero;

    [SyncVar(hook = nameof(OnTargetSet))]
    public Vector2 Target = Vector2.zero;

    private void OnDestroy()
    {
        if (isClient && clientSpawnOnDestroy != null)
        {
            Instantiate(clientSpawnOnDestroy, transform.position, Quaternion.identity);
        }
    }

    protected virtual void OnTargetSet(Vector2 oldPt, Vector2 newPt)
    {

    }

    protected float t = 0;
    protected virtual void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void FixedUpdate()
    {
        _rigidbody2D.velocity = Speed * Direction;
    }
}
