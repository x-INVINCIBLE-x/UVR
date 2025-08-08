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
    Gaia
}

[System.Serializable]
public class AilmentStatus
{
    public AilmentType Type;
    public float Value;
    public Stat resistance;
    public Stat defence;
    public bool isMaxed = false;
    public float ailmentLimit = 100;
    public event Action<AilmentType> AilmentEffectEnded;
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
                        AilmentEffectEnded?.Invoke(Type);

                    Value = 0;
                    isMaxed = false;
                }

                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
    }

    public void Reset()
    {
        Value = 0;
        isMaxed = false;
    }
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

    private bool isDead = false;

    public event System.Action OnDeath;
    public event Action<float> OnHealthChanged;
    public event Action<float, float> OnDamageTaken;
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

    public void TakeDamage(AttackData attackData)
    {
        TakePhysicalDamage(attackData);
        TakeAilmentDamage(attackData);
    }

    private void TakePhysicalDamage(AttackData attackData)
    {
        if (attackData.physicalDamage.Value < 0)
        {
            IncreaseHealthBy(-attackData.physicalDamage.Value);
            return;
        }

        float reducedDamage = Mathf.Max(0, attackData.physicalDamage.Value - physicalDef.Value);

        ReduceHealthBy(reducedDamage);
    }

    private void TakeAilmentDamage(AttackData attackData)
    {
        float _ignisAtk = attackData.ignisDamage.Value;
        float _frostAtk = attackData.frostDamage.Value;
        float _blitzAtk = attackData.blitzDamage.Value;
        float _hexAtk = attackData.hexDamage.Value;
        float _radianceAtk = attackData.radianceDamage.Value;
        float _gaiaAtk = attackData.gaiaDamage.Value;

        float damage = _ignisAtk + _frostAtk + _blitzAtk + _hexAtk + _radianceAtk + _gaiaAtk;

        if (damage == 0)
            return;

        if (_ignisAtk > 0)
            TryApplyAilmentEffect(_ignisAtk, attackData.burnDamage, ref ignisStatus, AilmentType.Ignis);
        else if (_frostAtk > 0)
            TryApplyAilmentEffect(_frostAtk, attackData.frostSpeedReduction, ref frostStatus, AilmentType.Frost);
        else if (_blitzAtk > 0)
            TryApplyAilmentEffect(_blitzAtk, attackData.blitzSurroundingDamage, ref blitzStatus, AilmentType.Blitz);
        else if (_hexAtk > 0)
            TryApplyAilmentEffect(_hexAtk, 0f, ref hexStatus, AilmentType.Hex);
        else if (_gaiaAtk > 0)
            TryApplyAilmentEffect(_gaiaAtk, attackData.gaiaHealAmount, ref gaiaStatus, AilmentType.Gaia);
        else if (_radianceAtk > 0)
            TryApplyAilmentEffect(_radianceAtk, 0f, ref radianceStatus, AilmentType.Radiance);
    }

    protected virtual void TryApplyAilmentEffect(float ailmentAtk, float ailmentEffect, ref AilmentStatus ailmentStatus, AilmentType ailmentType)
    {
        if (ailmentStatus.isMaxed)
            return;

        float ailmentDefence = ailmentStatus.defence.Value;
        float effectAmount = ailmentAtk - ailmentDefence;
        //ReduceHealthBy(effectAmount); // This line is commented out to prevent direct health reduction from ailment effects

        ailmentStatus.Value = Mathf.Min(ailmentStatus.ailmentLimit + ailmentLimitOffset, ailmentStatus.Value + effectAmount);
        StartCoroutine(ailmentStatus.ReduceValueOverTime());

        OnAilmentStatusChange?.Invoke(ailmentType, false, 0f);

        if (hasAilment || ailmentStatus.Value < ailmentStatus.ailmentLimit)
            return;

        OnAilmentStatusChange?.Invoke(ailmentType, true, ailmentEffect);

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
        KillCharacter();
    }

    private void ApplyRadianceAilment(float amount)
    {
        KillCharacter();
    }

    private void ApplyGaiaAilment(float amount)
    {
        // Check if gaia event applied and shooting healing projectile to player
    }

    #endregion

    public void TakePhysicalDamage(float damage)
    {
        float reducedDamage = Mathf.Max(0, damage - physicalDef.Value);

        ReduceHealthBy(reducedDamage);
    }

    public void ReduceHealthBy(float damage)
    {
        if (IsInvincible || isDead)
            return;

        damage *= vulnerability;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        OnHealthChanged?.Invoke(currentHealth/health.Value);

        if (currentHealth == 0f)
        {
            KillCharacter();
            return;
        }

        // If the character is not dead, invoke the damage taken event
        if (damageTakenBuffer + lastDamageTakenTime > Time.time)
            return;
        // If the damage taken is too close to the last damage taken, ignore it
        
        lastDamageTakenTime = Time.time;
        OnDamageTaken?.Invoke(currentHealth, health.Value);
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

    protected virtual void KillCharacter()
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

    public void SetInvincible(bool invincible) => IsInvincible = invincible;

    public void SetBlocking(bool blocking) => IsBlocking = blocking;

    public void SetPerfectBlock(bool perfectBlock) => IsPerfectBlock = perfectBlock;

    public void SetConsumingStamina(bool status) => IsConsumingStamina = status;

    public (float, float) GetHealth() => (currentHealth, health.Value);
    public float GetCurrentStamina() => currentStamina;

    public void RestoreStats()
    {
        isDead = false;

        if (statDictionary != null)
        {
            foreach (Stat stat in statDictionary.Values)
            {
                stat.RemoveAllModifiersFromSource(this);
            }
        }

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