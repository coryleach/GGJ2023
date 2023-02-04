using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [SerializeField]
    private Transform projectileSource = null;

    public float cooldown = 1f;

    public Projectile projectilePrefab;

    private float cooldownTime = 0;

    public bool IsReady => cooldownTime <= 0;

    public void Fire(Transform target)
    {
        if ( projectileSource == null )
        {
            Debug.LogError("Projectile source is missing");
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("You forgot to set the projectile prefab on the projecitle weapon script dummy!");
            return;
        }

        cooldownTime = cooldown;
        var projecitle = Instantiate(projectilePrefab, projectileSource.position, Quaternion.identity);
        projecitle.Direction = target.position - projectileSource.position;
    }

    public void Tick()
    {
        cooldownTime -= Time.deltaTime;
    }
}
