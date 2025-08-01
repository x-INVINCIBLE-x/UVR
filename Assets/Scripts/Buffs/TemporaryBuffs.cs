using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryBuffs : MonoBehaviour
{
    public List<Buff> oneOffBufffs = new(); // => Temporary Buff always working, Stats + 10
    public List<Buff> timerBuffs = new(); // => Activates after certain period of time, if always active then calls itself
    public List<BuffInfo> onHitBuffs = new();
    public List<BuffInfo> onDamageBuffs = new();

    public List<BuffInfo> disabledHitBuffs = new();
    public List<BuffInfo> disabledDamageBuffs = new();

    [System.Serializable]
    public class BuffInfo
    {
        public Buff buff;
        public float lastTimeUsed;

        public BuffInfo(Buff _buff, float _lastTimeUsed)
        {
            buff = _buff;
            lastTimeUsed = _lastTimeUsed;
        }
    }

    private CharacterStats stats;
    private HashSet<Buff> activeBuffs = new();

    private void Start()
    {
        stats = PlayerManager.instance.Player.stats;
        InitializeDungeonSetup();
    }

    private void InitializeDungeonSetup()
    {
        ChallengeManager.instance.OnChallengeStart += ActivateAllBuffs;
        ChallengeManager.instance.OnChallengeSuccess += RemoveAllBuffs;
        ChallengeManager.instance.OnChallengeFail += ClearAllBuffs;

        stats.OnDamageTaken += HandleHit;
    }

    private void ActivateAllBuffs(ChallengeType type)
    {
        StartTimerBuff();
        onDamageBuffs.AddRange(disabledDamageBuffs);
        onHitBuffs.AddRange(disabledHitBuffs);

        disabledDamageBuffs.Clear();
        onHitBuffs.Clear();
    }

    public void AddBuff(Buff buff)
    {
        switch (buff.ActivationType)
        {
            case ActivationType.OneOff:
                AddOneOffBuff(buff);
                return;
            case ActivationType.Timer:
                AddTimerBuff(buff);
                return;
            case ActivationType.OnDamage:
                AddDamageBuff(buff);
                return;
            case ActivationType.OnHit:
                AddHitBuff(buff);
                return;
        }
    }

    private void StartTimerBuff()
    {
        foreach (Buff buff in timerBuffs)
        {
            StartCoroutine(TimerBuffCoroutine(buff));
        }
    }

    private IEnumerator TimerBuffCoroutine(Buff buff)
    {
        if (buff.timer > 0)
            yield return new WaitForSeconds(buff.timer);

        ActivateBuff(buff);

        yield return new WaitForSeconds(buff.activeDuration);

        RemoveBuffFrom(timerBuffs);

        if (buff.ActivationStatus == ActivationStatus.AlwaysActive)
        {
            yield return new WaitForSeconds(buff.cooldownDuration);

            StartCoroutine(TimerBuffCoroutine(buff));
        }
    }


    private void HandleHit(float currentHealth, float maxHealth)
    {
        foreach (BuffInfo buffInfo in onHitBuffs)
        {
            if (buffInfo.buff.ActivationStatus == ActivationStatus.OneShot)
            {
                ActivateBuff(buffInfo.buff);
                disabledHitBuffs.Add(buffInfo);
                onHitBuffs.Remove(buffInfo);
            }
            else if (buffInfo.buff.ActivationStatus == ActivationStatus.AlwaysActive)
            {
                if (buffInfo.lastTimeUsed + buffInfo.buff.cooldownDuration < Time.time)
                {
                    ActivateBuff(buffInfo.buff);
                    buffInfo.lastTimeUsed = Time.time;
                }
            }
        }
    }

    //TODO: Handle Damage Buff Activation
    private void HandleDamage()
    {
        foreach (BuffInfo buffInfo in onDamageBuffs)
        {
            if (buffInfo.buff.ActivationStatus == ActivationStatus.OneShot)
            {
                ActivateBuff(buffInfo.buff);
                disabledDamageBuffs.Add(buffInfo);
                onDamageBuffs.Remove(buffInfo);
            }
            else if (buffInfo.buff.ActivationStatus == ActivationStatus.AlwaysActive)
            {
                if (buffInfo.lastTimeUsed + buffInfo.buff.cooldownDuration < Time.time)
                {
                    ActivateBuff(buffInfo.buff);
                    buffInfo.lastTimeUsed = Time.time;
                }
            }
        }
    }

    private void ActivateBuff(Buff buff)
    {
        if (buff.activeDuration > 0 && buff.ActivationType != ActivationType.Timer 
            && buff.ActivationType != ActivationType.OneOff)
        {
            if (activeBuffs.Contains(buff))
            {
                RemoveBuff(buff);
            }

            StartCoroutine(RemoveBuffCoroutine(buff));
        }

        ApplyModifiers(buff);
        ApplyEffects(buff);
        activeBuffs.Add(buff);
    }

    #region Apply Modifiers and Effectors
    private void ApplyModifiers(Buff buff)
    {
        foreach (Modifier modifier in buff.statsToBuff)
        {
            if (stats.statDictionary.TryGetValue(modifier.stat, out Stat stat))
            {
                StatModifier statMod = new StatModifier(modifier.value, modifier.modType, buff);
                stat.AddModifier(statMod);
            }
            else
            {
                Debug.LogWarning("Stat not found" + modifier.stat);
            }
        }
    }

    private void ApplyEffects(Buff buff)
    {
        foreach (Effect effect in buff.effects)
        {
            effect.Apply();
        }
    }
    #endregion

    private void RemoveAllBuffs()
    {
        RemoveTimerBuffs();
        RemoveBuffFrom(activeBuffs);
        activeBuffs.Clear();
    }

    private void ClearAllBuffs()
    {
        RemoveAllBuffs();

        oneOffBufffs.Clear();
        onHitBuffs.Clear();
        onDamageBuffs.Clear();
        timerBuffs.Clear();
    }

    private void RemoveTimerBuffs()
    {
        StopAllCoroutines();
        RemoveBuffFrom(timerBuffs);
    }

    private IEnumerator RemoveBuffCoroutine(Buff buff)
    {
        yield return new WaitForSeconds(buff.activeDuration);
        RemoveBuff(buff);
    }

    private void RemoveBuffFrom(IEnumerable<Buff> buffs)
    {
        foreach (Buff buff in new List<Buff>(buffs))
        {
            RemoveBuff(buff);
        }
    }

    private void RemoveBuff(Buff buff)
    {
        activeBuffs.Remove(buff);
        RemoveEffectors(buff);
        foreach (Stat stat in stats.statDictionary.Values)
        {
            stat.RemoveAllModifiersFromSource(buff);
        }
    }

    private void RemoveEffectors(Buff buff)
    {
        foreach (Effect effect in buff.effects)
        {
            effect.Remove();
        }
    }

    private void AddOneOffBuff(Buff buff)
    {
        oneOffBufffs.Add(buff);
        ActivateBuff(buff);
    }

    private void AddTimerBuff(Buff buff) => timerBuffs.Add(buff);
    private void AddHitBuff(Buff buff) => onHitBuffs.Add(new BuffInfo(buff, -10f));
    private void AddDamageBuff(Buff buff) => onDamageBuffs.Add(new BuffInfo(buff, -10f));

    public IEnumerable<Buff> GetCurrentBuffs()
    {
        foreach (var buff in oneOffBufffs)
            yield return buff;

        foreach (var buff in timerBuffs)
            yield return buff;

        foreach (BuffInfo buffInfo in onHitBuffs)
            yield return buffInfo.buff;

        foreach (BuffInfo buffInfo in onDamageBuffs)
            yield return buffInfo.buff;
    }

    private void OnDestroy()
    {
        ChallengeManager.instance.OnChallengeStart -= ActivateAllBuffs;
        ChallengeManager.instance.OnChallengeSuccess -= RemoveAllBuffs;
        ChallengeManager.instance.OnChallengeFail -= ClearAllBuffs;
    }
}
