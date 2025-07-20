using UnityEngine;

public class AilmentTurret : Turret
{
    [SerializeField] private DamageOnTouch damageOnTouch;
    [SerializeField] private AttackData attackData;
    [SerializeField] private float damageRate = 0.2f;

    private LayerMask playerLayer;

    private void Start()
    {
        damageOnTouch.gameObject.SetActive(false);
        playerLayer = LayerMask.GetMask("Player");
    }

    protected override void Activate(Collider activatingCollider)
    {
        damageOnTouch.Setup(attackData, damageRate, playerLayer);
        damageOnTouch.gameObject.SetActive(true);
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {
        damageOnTouch.gameObject.SetActive(false);
    }
}
