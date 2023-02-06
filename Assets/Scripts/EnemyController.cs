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

    [SerializeField]
    private AudioSource Audio;

    public AudioClip stepSound;

    private PathNode currentNode = null;

    public static float DeadMushrooms = 0;

    [SyncVar]
    public Vector3 currentNodePosition = Vector3.zero;

    [SyncVar(hook = nameof(OnHealthChanged))]
    public int CurrentHealth = 10;

    private static readonly int Hit = Animator.StringToHash("Hit");

    public EnemyEvent OnDestroyed { get; } = new EnemyEvent();

    private void Start()
    {
        CurrentHealth += (int)Mathf.Log(DeadMushrooms);
        Audio.pitch = 0.8f + (UnityEngine.Random.value * 0.4f);
    }

    private void OnDestroy()
    {
        OnDestroyed.Invoke(this);
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (oldHealth < newHealth)
        {
            return;
        }
        //Do some damaged animation
        anim.SetTrigger(Hit);
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


    private float speedMultiplier = 1f;
    private float speedMultiplierDuration = 0f;

    public void SetSpeedModifier(float amount, float duraiton)
    {
        speedMultiplier = Mathf.Min(amount, speedMultiplier);
        speedMultiplierDuration = duraiton;
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

            //Reset speed multiplier when the duration is up
            if (speedMultiplierDuration > 0)
            {
                speedMultiplierDuration -= Time.deltaTime;
                if (speedMultiplierDuration <= 0)
                {
                    speedMultiplier = 1;
                }
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
        _rigidbody2D.velocity = dir.normalized * moveSpeed * speedMultiplier;
    }

}
