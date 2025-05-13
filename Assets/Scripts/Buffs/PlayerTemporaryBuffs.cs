using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemporaryBuffs : MonoBehaviour
{
    private CharacterStats stats;

    [SerializeField] private List<Buff> oneShotBuffs = new();
    [SerializeField] private List<Buff> onStartBuffs = new();
    [SerializeField] private List<Buff> onDamageTakenBuffs = new();

    private Dictionary<Effect, float> effectCache = new Dictionary<Effect, float>();

    private void Start()
    {
        stats = PlayerManager.instance.player.stats;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActivateOnStartBuffs(); 
        }
    }

    private void ActivateOnStartBuffs() => ActivateBuffsOf(onStartBuffs);

    private void RemoveOneShotBuffs()
    {
        foreach (Buff buff in oneShotBuffs)
        {
            foreach (Stat stat in stats.statDictionary.Values)
            {
                stat.RemoveAllModifiersFromSource(buff);
            }
        }
    }

    private void ActiveOnDamageTakenBuffs() => ActivateBuffsOf(onDamageTakenBuffs);

    private void ActivateBuffsOf(List<Buff> buffs)
    {
        foreach (Buff buff in buffs)
        {
            ApplyBuff(buff);
        }
    }

    private void ApplyBuff(Buff buff)
    {
        ApplyModifiers(buff);
        ApplyEffects(buff);
    }

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

        if (buff.timer > 0f)
            CoroutineManager.instance.StartRoutine(RemoveModifiers(stats, buff));
    }

    private IEnumerator RemoveModifiers(CharacterStats stats, Buff buff)
    {
        yield return new WaitForSeconds(buff.timer);

        foreach (Stat stat in stats.statDictionary.Values)
        {
            stat.RemoveAllModifiersFromSource(buff);
        }
    }

    private void ApplyEffects(Buff buff)
    {
        foreach (Effect effect in buff.effects)
        {
            if (effectCache.TryGetValue(effect, out float lastTimeActivated))
            {
                if (lastTimeActivated + effect.cooldownDuration < Time.time)
                {
                    effect.Execute();
                    effectCache[effect] = Time.time;
                }
            }
            else
            {
                effect.Execute();
                effectCache[effect] = Time.time;
            }
        }
    }

    public void AddOneShotBuff(Buff buff) => oneShotBuffs.Add(buff);
    public void AddActivateOnStartBuff(Buff buff) => onStartBuffs.Add(buff);
}