using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private Rigidbody2D _rigidbody2D = null;

    [SerializeField]
    private float lifetime = 1f;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public Vector2 Direction
    {
        get;
        set;
    }

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
