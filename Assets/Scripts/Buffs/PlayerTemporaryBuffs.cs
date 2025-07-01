//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerTemporaryBuffs : MonoBehaviour
//{
//    private CharacterStats stats;

//    [SerializeField] private List<Buff> onStartBuffs = new();
//    [SerializeField] private List<Buff> activeBuffs = new();
//    [SerializeField] private List<Buff> onHitBuffs = new();

//    private Dictionary<Effect, float> effectCache = new Dictionary<Effect, float>();

//    private void Start()
//    {
//        stats = PlayerManager.instance.Core.stats;
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Tab))
//        {
//            ActivateOnStartBuffs();
//        }
//    }

//    public void AddBuff(Buff buff)
//    {
//        switch (buff.ActivationType)
//        {
//            case ActivationType.AlwaysActive:
//                AddActiveBuff(buff);
//                return;
//            case ActivationType.OnHit:
//                AddOnHitBuff(buff);
//                return;
//            case ActivationType.OnStart:
//                AddOnStartBuff(buff);
//                return;
//        }
//    }

//    #region Apply Buffs

//    private void ActivateOnStartBuffs() => ActivateBuffsOf(onStartBuffs);
//    private void ActiveOnDamageTakenBuffs() => ActivateBuffsOf(onHitBuffs);

//    private void ActivateBuffsOf(List<Buff> buffs)
//    {
//        foreach (Buff buff in buffs)
//        {
//            ApplyBuff(buff);
//        }
//    }

//    private void ApplyBuff(Buff buff)
//    {
//        ApplyModifiers(buff);
//        ApplyEffects(buff);
//    }

//    #endregion

//    #region Implement Modifiers and Effectors

//    private void ApplyModifiers(Buff buff)
//    {
//        foreach (Modifier modifier in buff.statsToBuff)
//        {
//            if (stats.statDictionary.TryGetValue(modifier.stat, out Stat stat))
//            {
//                StatModifier statMod = new StatModifier(modifier.value, modifier.modType, buff);
//                stat.AddModifier(statMod);
//            }
//            else
//            {
//                Debug.LogWarning("Stat not found" + modifier.stat);
//            }
//        }

//        if (buff.timer > 0f)
//            CoroutineManager.instance.StartRoutine(RemoveModifiers(stats, buff));
//    }

//    private void ApplyEffects(Buff buff)
//    {
//        foreach (Effect effect in buff.effects)
//        {
//            if (effectCache.TryGetValue(effect, out float lastTimeActivated))
//            {
//                if (lastTimeActivated + effect.cooldownDuration < Time.time)
//                {
//                    effect.Apply();
//                    effectCache[effect] = Time.time;
//                }
//            }
//            else
//            {
//                effect.Apply();
//                effectCache[effect] = Time.time;
//            }
//        }
//    }
//    #endregion

//    #region Remove Modifiers and Effectors
//    private void RemoveBuffFrom(List<Buff> buffs)
//    {
//        foreach (Buff buff in buffs)
//        {
//            RemoveEffectors(buff);
//            foreach (Stat stat in stats.statDictionary.Values)
//            {
//                stat.RemoveAllModifiersFromSource(buff);
//            }
//        }
//    }

//    private IEnumerator RemoveModifiers(CharacterStats stats, Buff buff)
//    {
//        yield return new WaitForSeconds(buff.timer);

//        foreach (Stat stat in stats.statDictionary.Values)
//        {
//            stat.RemoveAllModifiersFromSource(buff);
//        }
//    }

//    private void RemoveEffectors(Buff buff)
//    {
//        foreach (Effect effect in buff.effects)
//        {
//            effect.Remove();
//        }
//    }

//    #endregion

//    public void AddOnHitBuff(Buff buff) => onHitBuffs.Add(buff);
//    public void AddOnStartBuff(Buff buff) => onStartBuffs.Add(buff);
//    public void AddActiveBuff(Buff buff)
//    {
//        activeBuffs.Add(buff);
//        ApplyBuff(buff);
//    }
//}