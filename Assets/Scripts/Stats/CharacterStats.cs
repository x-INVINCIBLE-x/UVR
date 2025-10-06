using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum Stats
{
    Health,
    Stamina,
    StaminaRegain,
    PhysicalAtk,
    IgnisAtk,
    FrostAtk,
    BlitzAtk,
    HexAtk,
    RadianceAtk,
    GaiaAtk,
    PhysicalDef,
    IgnisDef,
    FrostDef,
    BlitzDef,
    HexDef,
    RadianceDef,
    GaiaDef,
    IgnisRes,
    FrostRes,
    BlitzRes,
    HexRes,
    RadianceRes,
    GaiaRes
}

public enum AilmentType
{
    Ignis,
    Frost,
    Blitz,
    Hex,
    Radiance,
    Gaia,
    None
}

[System.Serializable]
public class AilmentStatus
{
    [HideInInspector] public AilmentType Type;
    public float Value;
    public Stat resistance;
    public Stat defence;
    public bool isMaxed = false;
    [HideInInspector] public float ailmentLimit = 100;
    public event Action<AilmentType> AilmentEffectEnded;
    [HideInInspector] public bool isActive = false;

    public IEnumerator ReduceValueOverTime(float resistance = -1)
    {
        if (resistance < 0)
            resistance = this.resistance.Value;

        while (Value > 0)
        {
            isActive = true;
            if (Value > 0)
            {
                Value -= resistance * Time.deltaTime;

                if (Value <= 0)
                {
                    if (isMaxed)
                        AilmentEffectEnded?.Invoke(Type);

                    Value = 0;
                    isMaxed = false;
                }

                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        isActive = false;
    }

    public void Reset()
    {
        Value = 0;
        isMaxed = false;
    }
}
[System.Serializable]
public class DeathDefice
{
    public DeathDefice(int amount, object source)
    {
        healthAmount = amount;
        this.source = source;
    }

    public object source;
    public int healthAmount;
}
public class CharacterStats : MonoBehaviour, IDamageable
{
    [SerializeField] protected DifficultyProfile difficultyProfile;

    [Header("Common Abilities")]
    public Stat health;
    public Stat stamina;
    public Stat staminaRegain;

    [Header("Attack Abilities")]
    public Stat physicalDamage;
    public Stat ignisDamage;
    public Stat frostDamage;
    public Stat blitzDamage;
    public Stat hexDamage;
    public Stat radianceDamage;
    public Stat gaiaDamage;

    [Header("Defence")]
    public Stat physicalDef;
    //public Stat fireDef;
    //public Stat electricDef;

    [Space]
    //public Stat fireRes;
    //public Stat electricRes;

    [Header("Ailment Status")]
    public AilmentStatus ignisStatus;
    public AilmentStatus frostStatus;
    public AilmentStatus blitzStatus;
    public AilmentStatus hexStatus;
    public AilmentStatus radianceStatus;
    public AilmentStatus gaiaStatus;

    public float ailmentLimitOffset = 10;
    public float maxResistance = 75f;

    private float vulnerability = 1f;
    public float Vulnerability
    {
        get => vulnerability;
        set => vulnerability = value;
    }

    public float currentHealth;
    public float currentStamina;

    [field: SerializeField] public bool IsInvincible { get; private set; } = false;
    public bool IsBlocking { get; private set; } = false;
    public bool IsPerfectBlock { get; private set; } = false;
    public bool IsConsumingStamina { get; private set; } = false;

    protected Dictionary<AilmentType, System.Action<float>> ailmentActions;
    protected Dictionary<AilmentType, AilmentStatus> ailmentStatuses;
    public Dictionary<Stats, Stat> statDictionary;

    [SerializeField] private List<DeathDefice> deathDefices = new();
    private bool isDead = false;
    private bool deadResult = false;

    public event System.Action OnDeathDeficeUsed;
    public event System.Action OnDeath;
    public event Action<float> OnHealthChanged;
    public event Action<float> OnDamageTaken;
    public event Action<DamageResult> OnDamageGiven;
    public event Action<AilmentType, bool, float> OnAilmentStatusChange;

    private float damageTakenBuffer = 0.1f;
    private float lastDamageTakenTime = 0f;

    private bool hasAilment = false;
    private WaitForSeconds burnDelay = new WaitForSeconds(0.5f);

    //public event System.Action UpdateHUD;

    protected virtual void Awake()
    {
        InitializeValues();

        ignisStatus.Type = AilmentType.Ignis;
        frostStatus.Type = AilmentType.Frost;
        blitzStatus.Type = AilmentType.Blitz;
        hexStatus.Type = AilmentType.Hex;
        radianceStatus.Type = AilmentType.Radiance;
        gaiaStatus.Type = AilmentType.Gaia;

        ailmentActions = new Dictionary<AilmentType, System.Action<float>>
        {
            { AilmentType.Ignis, ApplyFireAilment },
            { AilmentType.Frost, ApplyFrostAilment },
            { AilmentType.Blitz, ApplyBlitzAilment },
            { AilmentType.Hex, ApplyHexAilment },
            { AilmentType.Radiance, ApplyRadianceAilment },
            { AilmentType.Gaia, ApplyGaiaAilment }
        };

        ailmentStatuses = new Dictionary<AilmentType, AilmentStatus>
        {
            { AilmentType.Ignis, ignisStatus },
            { AilmentType.Frost, frostStatus },
            { AilmentType.Blitz, blitzStatus },
            { AilmentType.Hex, hexStatus },
            { AilmentType.Radiance, radianceStatus },
            { AilmentType.Gaia, gaiaStatus }
        };
    }

    protected virtual void Start()
    {
        InitializeStatDictionary();

        OnHealthChanged?.Invoke(currentHealth / health.Value);
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
            //{ Stats.Stamina,  stamina },
            //{ Stats.StaminaRegain, staminaRegain },
            { Stats.PhysicalAtk, physicalDamage },
            { Stats.IgnisAtk, ignisDamage },
            { Stats.FrostAtk, frostDamage },
            { Stats.BlitzAtk, blitzDamage },
            { Stats.HexAtk, hexDamage },
            { Stats.RadianceAtk, radianceDamage },
            { Stats.GaiaAtk, gaiaDamage },
            { Stats.PhysicalDef, physicalDef },
            { Stats.IgnisDef, ignisStatus.defence},
            { Stats.FrostDef, frostStatus.defence},
            { Stats.BlitzDef, blitzStatus.defence },
            { Stats.HexDef, hexStatus.defence},
            { Stats.RadianceDef, radianceStatus.defence},
            { Stats.GaiaDef, gaiaStatus.defence},
            { Stats.IgnisRes, ignisStatus.resistance},
            { Stats.FrostRes, frostStatus.resistance},
            { Stats.BlitzRes, blitzStatus.resistance},
            { Stats.HexRes, hexStatus.resistance},
            { Stats.RadianceRes, radianceStatus.resistance},
            { Stats.GaiaRes, gaiaStatus.resistance}

        };
    }

    public void RaiseOnDamageGiven(DamageResult result)
    {
        if (result.killed)
            OnDamageGiven?.Invoke(result);
    }

    public DamageResult TakeDamage(AttackData attackData)
    {
        if (attackData.hexDamage.Value > 0)
            Debug.Log(attackData.name  + "damage" + attackData.hexDamage.Value);
        AilmentType ailmentType = GetAilmentType(attackData);
        TakeAilmentDamage(attackData, ailmentType);
        return TakePhysicalDamage(attackData, ailmentType);
    }

    private AilmentType GetAilmentType(AttackData attackData)
    {
        if (attackData.ignisDamage.Value > 0)
            return AilmentType.Ignis;
        else if (attackData.frostDamage.Value > 0)
            return AilmentType.Frost;
        else if (attackData.blitzDamage.Value > 0)
            return AilmentType.Blitz;
        else if (attackData.hexDamage.Value > 0)
            return AilmentType.Hex;
        else if (attackData.radianceDamage.Value > 0)
            return AilmentType.Radiance;
        else if (attackData.gaiaDamage.Value > 0)
            return AilmentType.Gaia;
        else
            return AilmentType.None;
    }

    private DamageResult TakePhysicalDamage(AttackData attackData, AilmentType ailment)
    {
        if (attackData.physicalDamage.Value < 0)
        {
            IncreaseHealthBy(-attackData.physicalDamage.Value);
            return null;
        }

        float finalDamage = attackData.physicalDamage.Value - physicalDef.Value;
        if (ailment != AilmentType.None)
        {
            finalDamage -= GetAilmentStatus(ailment).defence.Value;
        }

        finalDamage = Mathf.Max(1, finalDamage);

        return ReduceHealthBy(finalDamage);
    }

    private void TakeAilmentDamage(AttackData attackData, AilmentType ailment)
    {
        //float _ignisAtk = attackData.ignisDamage.Value;
        //float _frostAtk = attackData.frostDamage.Value;
        //float _blitzAtk = attackData.blitzDamage.Value;
        //float _hexAtk = attackData.hexDamage.Value;
        //float _radianceAtk = attackData.radianceDamage.Value;
        //float _gaiaAtk = attackData.gaiaDamage.Value;

        //float damage = _ignisAtk + _frostAtk + _blitzAtk + _hexAtk + _radianceAtk + _gaiaAtk;

        if (ailment == AilmentType.None)
            return;

        if (ailment == AilmentType.Ignis)
            TryApplyAilmentEffect(attackData.ignisDamage.Value, attackData.burnDamage, ref ignisStatus, AilmentType.Ignis);
        else if (ailment == AilmentType.Frost)
            TryApplyAilmentEffect(attackData.frostDamage.Value, attackData.frostSpeedReduction, ref frostStatus, AilmentType.Frost);
        else if (ailment == AilmentType.Blitz)
            TryApplyAilmentEffect(attackData.blitzDamage.Value, attackData.blitzSurroundingDamage, ref blitzStatus, AilmentType.Blitz);
        else if (ailment == AilmentType.Hex)
            TryApplyAilmentEffect(attackData.hexDamage.Value, 0f, ref hexStatus, AilmentType.Hex);
        else if (ailment == AilmentType.Gaia)
            TryApplyAilmentEffect(attackData.gaiaDamage.Value, attackData.gaiaHealAmount, ref gaiaStatus, AilmentType.Gaia);
        else if (ailment == AilmentType.Radiance)
            TryApplyAilmentEffect(attackData.radianceDamage.Value, 0f, ref radianceStatus, AilmentType.Radiance);
    }

    protected virtual void TryApplyAilmentEffect(float ailmentAtk, float ailmentEffect, ref AilmentStatus ailmentStatus, AilmentType ailmentType)
    {
        if (ailmentStatus.isMaxed)
            return;

        float ailmentDefence = ailmentStatus.defence.Value;
        float effectAmount = ailmentAtk - ailmentDefence;
        //ReduceHealthBy(effectAmount); // This line is commented out to prevent direct health reduction from ailment effects

        if (effectAmount <= 0)
            return;

        ailmentStatus.Value = Mathf.Min(ailmentStatus.ailmentLimit + ailmentLimitOffset, ailmentStatus.Value + effectAmount);

        if (!ailmentStatus.isActive)
            StartCoroutine(ailmentStatus.ReduceValueOverTime());

        OnAilmentStatusChange?.Invoke(ailmentType, false, 0f);

        if (hasAilment || ailmentStatus.Value < ailmentStatus.ailmentLimit)
            return;

        OnAilmentStatusChange?.Invoke(ailmentType, true, ailmentEffect);

        StopCoroutine(ailmentStatus.ReduceValueOverTime());
        StartCoroutine(ailmentStatus.ReduceValueOverTime(maxResistance));

        ApplyAilment(ailmentType, ailmentEffect);
        ailmentStatus.isMaxed = true;
        ailmentStatus.AilmentEffectEnded += AilmentEffectEnded;
    }

    private void ApplyAilment(AilmentType ailmentType, float amount)
    {
        hasAilment = true;
        if (ailmentActions.TryGetValue(ailmentType, out var ailmentEffect))
            ailmentEffect(amount);
    }

    protected virtual void AilmentEffectEnded(AilmentType ailmentStatus)
    {
        hasAilment = false;
        OnAilmentStatusChange?.Invoke(ailmentStatus, false, 0f);
    }

    #region Ailment Specific functions

    private void ApplyFireAilment(float amount)
    {
        StartCoroutine(ContinousDamage(amount));
    }

    private IEnumerator ContinousDamage(float amount)
    {
        while (ignisStatus.Value > 0)
        {
            ReduceHealthBy(amount);
            yield return burnDelay;
        }
    }

    private void ApplyFrostAilment(float amount)
    {
        // Listen to frost event and slow down the character
        // handled by subscribers
    }

    private void ApplyBlitzAilment(float amount)
    {
        // Check if blitz event applied and damage is taken in character -> shoot lightning homing projectilee
        // handled by subscribers
    }

    private void ApplyHexAilment(float amount)
    {
        ReduceHealthBy(health.Value);
    }

    private void ApplyRadianceAilment(float amount)
    {
        ReduceHealthBy(health.Value);
    }

    private void ApplyGaiaAilment(float amount)
    {
        // Check if gaia event applied and shooting healing projectile to player
    }

    #endregion

    public DamageResult ReduceHealthBy(float damage)
    {
        if (isDead && deadResult == false)
        {
            deadResult = true;
            return new DamageResult(true, true, 0, this);
        }

        if (IsInvincible || isDead)
            return null;

        DamageResult result;

        damage *= vulnerability;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        OnHealthChanged?.Invoke(currentHealth / health.Value);

        result = new(true, false, damage, this);

        if (currentHealth == 0f)
        {
            if (deathDefices.Count > 0)
            {
                IncreaseHealthBy(deathDefices[0].healthAmount);
                deathDefices.RemoveAt(0);
                OnDeathDeficeUsed?.Invoke();
                return result;
            }

            KillCharacter();

            result.killed = true;
            return result;
        }

        if (damageTakenBuffer + lastDamageTakenTime < Time.time)
        {
            lastDamageTakenTime = Time.time;
            OnDamageTaken?.Invoke(damage);
        }

        return result;
    }

    private void IncreaseHealthBy(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Min(health.Value, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth / health.Value);
    }

    public virtual void KillCharacter()
    {
        isDead = true;
        currentHealth = 0f;
        OnDeath?.Invoke();
    }

    public AilmentStatus GetAilmentStatus(AilmentType type)
    {
        if (ailmentStatuses.TryGetValue(type, out var status))
        {
            return status;
        }
        return null;
    }

    public void SetInvincibleFor(float time) => StartCoroutine(MakeInvincibleFor(time));

    private IEnumerator MakeInvincibleFor(float time)
    {
        IsInvincible = true;

        yield return new WaitForSeconds(time);

        IsInvincible = false;
    }

    public void AddDeathDefice(int healthAmount, object source) => deathDefices.Add(new(healthAmount, source));

    public void RemoveDeathDeficesFromSource(object source)
    {
        deathDefices.RemoveAll(defice => defice.source == source);
    }

    public void SetInvincible(bool invincible) => IsInvincible = invincible;

    public void SetBlocking(bool blocking) => IsBlocking = blocking;

    public void SetPerfectBlock(bool perfectBlock) => IsPerfectBlock = perfectBlock;

    public void SetConsumingStamina(bool status) => IsConsumingStamina = status;

    public (float, float) GetHealth() => (currentHealth, health.Value);
    public float GetCurrentStamina() => currentStamina;

    public virtual void RestoreStats()
    {
        isDead = false;
        deadResult = false;

        currentHealth = health.Value;
        ignisStatus.Reset();
        frostStatus.Reset();
        blitzStatus.Reset();
        hexStatus.Reset();
        gaiaStatus.Reset();
        radianceStatus.Reset();
        // TODO: Stop Ailment

    }

    public void Heal(float amount) => IncreaseHealthBy(amount);
}