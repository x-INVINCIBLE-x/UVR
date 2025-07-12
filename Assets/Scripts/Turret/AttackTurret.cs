using System.Collections;
using UnityEngine;

public class AttackTurret : Turret
{
    public enum TurretPhase
    {
        Rest,
        PreAttack,
        Attack,
        Cooldown
    }

    [Header("Turret Timings")]
    [SerializeField] protected float initializeTime = 1f;
    [SerializeField] protected float cooldown = 1f;
    [SerializeField] protected float attackDuration = 1f;

    protected TurretPhase currentPhase = TurretPhase.Rest;
    protected float phaseTimer = 0f;

    public float PhaseProgressNormalized { get; protected set; } = 0f;
    public TurretPhase CurrentPhase => currentPhase;

    protected override void Activate(Collider activatingCollider)
    {
        StartCoroutine(PhaseRoutine());
    }

    protected override void Deactivate(Collider deactivatingCollider)
    {

    }

    private IEnumerator PhaseRoutine()
    {
        while (isActive)
        {
            yield return StartCoroutine(EnterPhase(TurretPhase.PreAttack, initializeTime));
            yield return StartCoroutine(EnterPhase(TurretPhase.Attack, attackDuration));
            yield return StartCoroutine(EnterPhase(TurretPhase.Cooldown, cooldown));
        }

        yield return StartCoroutine(EnterPhase(TurretPhase.Rest, 0f));
    }

    private IEnumerator EnterPhase(TurretPhase newPhase, float duration)
    {
        OnPhaseExit(currentPhase); // Notify phase exit
        currentPhase = newPhase;
        OnPhaseEnter(newPhase);    // Notify phase enter

        phaseTimer = 0f;
        PhaseProgressNormalized = 0f;

        while (phaseTimer < duration)
        {
            phaseTimer += Time.deltaTime;
            PhaseProgressNormalized = Mathf.Clamp01(phaseTimer / duration);
            yield return null;
        }

        PhaseProgressNormalized = 1f;
    }

    /// <summary>
    /// Override this in child classes to trigger animations, UI, VFX, sounds, etc.
    /// </summary>
    private void OnPhaseEnter(TurretPhase phase)
    {
        switch (phase)
        {
            case TurretPhase.PreAttack:
                OnPreAttackEnter();
                break;
            case TurretPhase.Attack:
                OnAttackEnter();
                break;
            case TurretPhase.Cooldown:
                OnCooldownEnter();
                break;
        }
    }

    /// <summary>
    /// Called just before leaving the current phase. Override in child classes.
    /// </summary>
    private void OnPhaseExit(TurretPhase phase)
    {
        switch (phase)
        {
            case TurretPhase.PreAttack:
                OnPreAttackExit();
                break;
            case TurretPhase.Attack:
                OnAttackExit();
                break;
            case TurretPhase.Cooldown:
                OnCooldownExit();
                break;
        }
    }

    protected virtual void OnPreAttackEnter()
    {

    }

    protected virtual void OnAttackEnter()
    {

    }

    protected virtual void OnCooldownEnter()
    {

    }

    protected virtual void OnPreAttackExit()
    {

    }

    protected virtual void OnAttackExit()
    {

    }

    protected virtual void OnCooldownExit()
    {

    }
}