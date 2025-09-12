using UnityEngine;

public abstract class Dissolver : MonoBehaviour
{
    public abstract void StartImpactDissolve(float duration);//Used for Dissolving and Redissolving the mesh in given time(Partial Dissolve only)
    public abstract void ImpactPartialDissolve();//Used for Partial Dissolve
    public abstract void ImpactPartialRedissolve();//Used for Partial Redissolve
    public abstract void StartDissolve();//Used for Complete Dissolvation
}
