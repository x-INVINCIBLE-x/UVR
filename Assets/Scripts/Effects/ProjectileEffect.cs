using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile_Effect", menuName = "Effects/Projectile")]
public class ProjectileEffect : Effect
{
    [Header("Projectile Settings")]
    [SerializeField] private LayerMask includeLayer;
    [SerializeField] private HomingMissile homingProjectile;

    [Tooltip("Uses Player base attack to give damage")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float damageMultiplier = 0.1f;
    [SerializeField, Min(1f)] private float hitRadius = 1.0f;
    //[SerializeField, Min(1)] private int amountToSpawn = 1;
    [SerializeField] private float projectileLifeTime = 10f;

    public override void Apply()
    {
        base.Apply();

        Vector3 spawnPosition = PlayerManager.instance.PlayerOrigin.transform.position;

        HashSet<Transform> hitTransforms = new();
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, hitRadius, includeLayer);

        AttackData attackData = CreateInstance<AttackData>();
        attackData.Init();

        attackData = stats.CombineWith(attackData, damageMultiplier);

        foreach (var col in colliders)
        {
            if (hitTransforms.Contains(col.transform.root)) continue;

            hitTransforms.Add(col.transform.root);
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                HomingMissile newMissile = ObjectPool.instance.GetObject(homingProjectile.gameObject, spawnPosition).GetComponent<HomingMissile>();
                newMissile.Setup(col.GetComponent<Rigidbody>(), attackData, projectileSpeed, 4, projectileLifeTime, projectileLifeTime, includeLayer);
            }
        }
    }

    public override void Remove()
    {
        base.Remove();
    }
}