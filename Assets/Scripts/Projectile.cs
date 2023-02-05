using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private Rigidbody2D _rigidbody2D = null;

    [SerializeField]
    private float lifetime = 1f;

    [SerializeField]
    private AudioSource Audio;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    [SyncVar]
    public Vector2 Direction = Vector2.zero;

    private float t = 0;

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = Speed * Direction;
    }
}
