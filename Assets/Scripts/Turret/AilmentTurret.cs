using UnityEngine;

public class AilmentTurret : Turret
{
    [SerializeField] private DamageOnTouch damageOnTouch;
    [SerializeField] private AttackData attackData;
    [SerializeField] private float damageRate = 0.2f;

    private void Start()
    {
        damageOnTouch.gameObject.SetActive(false);
    }

    protected override void Activate(Collider activatingCollider)
    {
        damageOnTouch.Setup(attackData, damageRate);
        damageOnTouch.gameObject.SetActive(true);
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {
        damageOnTouch.gameObject.SetActive(false);
    }
}
