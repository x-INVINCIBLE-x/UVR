using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LaserAttackTurret : AttackTurret
{
    [SerializeField] private Transform rotator;
    [SerializeField] private DamageOnTouch[] lasers;
    [SerializeField] private AttackData attackData;
    [SerializeField] private float damageRate;
    [SerializeField] private float rotationSpeed;

    private Coroutine currentRoutine;
    private LayerMask playerLayer;

    private void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
    }

    protected override void OnPreAttackEnter()
    {
        base.OnPreAttackEnter();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].Setup(attackData, damageRate, playerLayer);
        }
    }

    protected override void OnPreAttackExit()
    {
        base.OnPreAttackExit();
    }

    protected override void OnAttackEnter()
    {
        base.OnAttackEnter();

        currentRoutine = StartCoroutine(RotationCoroutine());

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].gameObject.SetActive(true);
            lasers[i].enabled = true;
        }
    }

    protected override void OnAttackExit()
    {
        base.OnAttackExit();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].gameObject.SetActive(false);
            lasers[i].enabled = false;
        }
    }

    protected override void OnCooldownEnter()
    {
        base.OnCooldownEnter();

        GetComponent<Renderer>().material.color = Color.red;
    }

    protected override void OnCooldownExit()
    {
        base.OnCooldownExit();

        GetComponent<Renderer>().material.color = Color.white;
    }

    private IEnumerator RotationCoroutine()
    {
        while (isActive)
        {
            rotator.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            yield return null;
        }

        currentRoutine = null;
    }
}
