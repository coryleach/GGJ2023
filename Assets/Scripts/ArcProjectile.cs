using System;
using Mirror;
using UnityEngine;

public class ArcProjectile : Projectile
{
    [SerializeField] private GameObject shadow;

    public float height = 5;

    private Vector2 startPt;
    protected float duration = 1f;

    private float fixedTime = 0;

    private bool complete = false;
    private bool applied = false;

    protected override void OnTargetSet(Vector2 oldPt, Vector2 newPt)
    {
        base.OnTargetSet(oldPt, newPt);
        duration = (Target - startPt).magnitude / speed;
    }

    private void Start()
    {
        EnableColliders(false);
        _rigidbody2D.isKinematic = true;
        startPt = _rigidbody2D.position;
        duration = (Target - startPt).magnitude / speed;
    }

    protected override void Update()
    {
        var lerpTime = Mathf.InverseLerp(0, duration, fixedTime);
        var pt = Vector2.Lerp(startPt, Target, lerpTime);
        shadow.transform.position = pt;
        shadow.SetActive(true);

        if (isServer)
        {
            if (complete && !applied)
            {
                applied = true;
                EnableColliders(true);
            }
            else if (complete)
            {
                //This will tick the lifetime of the projectile and kill it on end
                //WE don't start this until the path is complete
                base.Update();
            }
        }
    }

    protected void EnableColliders(bool value)
    {
        var colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = value;
        }
    }

    protected override void FixedUpdate()
    {
        if (complete)
        {
            return;
        }

        fixedTime += Time.fixedDeltaTime;

        var lerpTime = Mathf.InverseLerp(0, duration, fixedTime);

        var pt = Vector2.Lerp(startPt, Target, lerpTime);

        //Make it curve upwards
        pt += Vector2.up * Mathf.Sin(lerpTime * Mathf.PI) * height * 0.5f;

        _rigidbody2D.position = pt;

        if (lerpTime >= 1)
        {
            complete = true;
            _rigidbody2D.isKinematic = false;
            _rigidbody2D.velocity = Vector3.zero;
        }
    }
}
