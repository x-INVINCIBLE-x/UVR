using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stats
{
    Health,
    Stamina,
    StaminaRegain,
    PhysicalAtk,
    FireAtk,
    ElectricAtk,
    PhysicalDef,
    FireDef,
    ElectricDef,
    FireRes,
    ElectricRes,
}

public enum AilmentType
{
    Fire,
    Electric,
}

public class CharacterStats : MonoBehaviour, IDamagable
{
    [SerializeField] private DifficultyProfile difficultyProfile;

    [Header("Common Abilities")]
    public Stat health;
    public Stat stamina;
    public Stat staminaRegain;

    [Header("Attack Abilities")]
    public Stat physicalAtk;
    public Stat fireAtk;
    public Stat electricAtk;

    [Header("Defence")]
    public Stat physicalDef;
    public Stat fireDef;
    public Stat electricDef;

    [Space]
    public Stat fireRes;
    public Stat electricRes;

    [Header("Ailment Status")]
    public AilmentStatus fireStatus;
    public AilmentStatus electricStatus;

    public float ailmentLimitOffset = 10;

    public float currentHealth;
    public float currentStamina;

    [field: SerializeField] public bool isInvincible { get; private set; } = false;
    public bool IsBlocking { get; private set; } = false;
    public bool IsPerfectBlock { get; private set; } = false;
    public bool IsConsumingStamina { get; private set; } = false;

    protected Dictionary<AilmentType, System.Action> ailmentActions;
    public Dictionary<Stats, Stat> statDictionary;

    private bool isDead = false;

    public event System.Action<float, float> OnDamageTaken;
    public event System.Action OnPlayerDeath;
    public event System.Action UpdateHUD;

    [System.Serializable]
    public class AilmentStatus
    {
        public float Value;
        public Stat resistance;
        public Stat defence;
        public bool isMaxed = false;
        public float ailmentLimit = 100;
        public event Action<AilmentStatus> ailmentEffectEnded;
        public IEnumerator ReduceValueOverTime()
        {
            while (Value > 0)
            {
                if (Value > 0)
                {
                    Value -= resistance.Value * Time.deltaTime;

                    if (Value <= 0)
                    {
                        if (isMaxed)
                            ailmentEffectEnded?.Invoke(this);

                        Value = 0;
                        isMaxed = false;
                    }

                    yield return new WaitForEndOfFrame();
                }
                yield return null;
            }
        }
    }

    protected virtual void Awake()
    {
        InitializeValues();

        ailmentActions = new Dictionary<AilmentType, System.Action>
        {
            { AilmentType.Fire, ApplyFireAilment },
            { AilmentType.Electric, ApplyElectricAilment }
        };
    }

    private void Start()
    {
        InitializeStatDictionary();
        if (difficultyProfile != null)
        {
            Debug.Log("upgrade");
            difficultyProfile.ApplyModifiers(statDictionary, DungeonManager.Instance.DifficultyLevel, this);
        }
    }

    private void InitializeValues()
    {
        currentHealth = health.Value;
        currentStamina = stamina.Value;
    }

    public void InitializeStatDictionary()
    {
        statDictionary = new Dictionary<Stats, Stat>
        {
            { Stats.Health,   health },
            { Stats.Stamina,  stamina },
            { Stats.StaminaRegain, staminaRegain },
            { Stats.PhysicalAtk, physicalAtk },
            { Stats.FireAtk, fireAtk },
            { Stats.ElectricAtk, electricAtk },
            { Stats.PhysicalDef, physicalDef },
            { Stats.FireDef, fireDef },
            { Stats.ElectricDef, electricDef },
            { Stats.FireRes, fireRes },
            { Stats.ElectricRes, electricRes }
        };
    }

    private void Update()
    {
        if (!IsConsumingStamina && currentStamina < stamina.Value)
        {
            currentStamina += staminaRegain.Value * Time.deltaTime;
            UpdateHUD?.Invoke();
        }
    }

    public void TakeDamage(AttackData attackData)
    {
        TakePhysicalDamage(attackData);
        TakeAilmentDamage(attackData);
    }

    private void TakePhysicalDamage(AttackData attackData)
    {
        float reducedDamage = Mathf.Max(0, attackData.physicalDamage.Value - physicalDef.Value);

        ReduceHealthBy(reducedDamage);
    }

    private void TakeAilmentDamage(AttackData attackData)
    {
        float _fireAtk = attackData.fireDamage.Value;
        float _electricAtk = attackData.electricalDamage.Value;

        float damage = _fireAtk + _electricAtk;

        if (damage == 0)
            return;

        if (_fireAtk > 0)
            TryApplyAilmentEffect(_fireAtk, ref fireStatus, AilmentType.Fire);
        else if (_electricAtk > 0)
            TryApplyAilmentEffect(_electricAtk, ref electricStatus, AilmentType.Electric);
    }

    //public virtual void DoDamage(CharacterStats targetStats)
    //{
    //    if (isPerfectBlock)
    //    {
    //        Debug.Log("Perfect Block Successful!");
    //        return;
    //    }

    //    TakePhysicalDamage(physicalAtk.Value);

    //    DoAilmentDamage(targetStats);
    //    UpdateHUD?.Invoke();
    //}

    //public void DoAilmentDamage(CharacterStats targetStats)
    //{
    //    float _fireAtk = fireAtk.Value;
    //    float _electricAtk = electricAtk.Value;

    //    float damage = _fireAtk + _electricAtk;

    //    if (damage == 0)
    //        return;

    //    if (_fireAtk > 0)
    //        TryApplyAilmentEffect(_fireAtk, ref fireStatus, AilmentType.Fire);
    //    else if (_electricAtk > 0)
    //        TryApplyAilmentEffect(_electricAtk, ref electricStatus, AilmentType.Electric);
    //}

    protected virtual void TryApplyAilmentEffect(float ailmentAtk, ref AilmentStatus ailmentStatus, AilmentType ailmentType)
    {
        if (ailmentStatus.isMaxed)
            return;

        float ailmentDefence = ailmentStatus.defence.Value;
        float effectAmount = ailmentAtk - ailmentDefence;
        ReduceHealthBy(effectAmount);

        ailmentStatus.Value = Mathf.Min(ailmentStatus.ailmentLimit + ailmentLimitOffset, ailmentStatus.Value + effectAmount);
        StartCoroutine(ailmentStatus.ReduceValueOverTime());

        // ------------------------ update UI -----------------------------------
        //UI.instance.ailmentSlider[((int)ailmentType)].gameObject.SetActive(true);
        //StartCoroutine(UI.instance.ailmentSlider[((int)ailmentType)].UpdateUI());

        if (ailmentStatus.Value < ailmentStatus.ailmentLimit)
            return;

        ApplyAilment(ailmentType);
        ailmentStatus.isMaxed = true;
        ailmentStatus.ailmentEffectEnded += AilmentEffectEnded;
    }

    private void ApplyAilment(AilmentType ailmentType)
    {
        if (ailmentActions.TryGetValue(ailmentType, out var ailmentEffect))
            ailmentEffect();
    }

    protected virtual void AilmentEffectEnded(AilmentStatus ailmentStatus)
    {
        ailmentStatus.ailmentEffectEnded -= AilmentEffectEnded;
    }

    #region Ailment Specific functions

    private void ApplyFireAilment()
    {

    }

    private void ApplyElectricAilment()
    {

    }

    #endregion

    public void TakePhysicalDamage(float damage)
    {
        float reducedDamage = Mathf.Max(0, damage - physicalDef.Value);

        ReduceHealthBy(reducedDamage);
    }

    public void ReduceHealthBy(float damage)
    {
        if (isInvincible || isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        OnDamageTaken?.Invoke(currentHealth, health.Value);

        if (currentHealth == 0f)
            KillPlayer();
    }

    private void KillPlayer()
    {
        isDead = true;
        OnPlayerDeath?.Invoke();
    }

    public void SetInvincibleFor(float time) => StartCoroutine(MakeInvincibleFor(time));

    private IEnumerator MakeInvincibleFor(float time)
    {
        isInvincible = true;

        yield return new WaitForSeconds(time);

        isInvincible = false;
    }

    public void SetInvincible(bool invincible) => isInvincible = invincible;

    public void SetBlocking(bool blocking) => IsBlocking = blocking;

    public void SetPerfectBlock(bool perfectBlock) => IsPerfectBlock = perfectBlock;

    public void SetConsumingStamina(bool status) => IsConsumingStamina = status;

    public bool HasEnoughStamina(float staminaAmount)
    {
        if (currentStamina > staminaAmount)
        {
            currentStamina -= staminaAmount;
            UpdateHUD?.Invoke();
            return true;
        }

        return false;
    }

    public (float, float) GetHealth() => (currentHealth, health.Value);
    public float GetCurrentStamina() => currentStamina;
}