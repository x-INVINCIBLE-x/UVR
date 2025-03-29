using System;
using UnityEngine;
using UnityHFSM;
using UnityEngine.Pool;

public class ShootState : EnemyStateBase
{
    private ShootEnemy Prefab;
    private ObjectPool<ShootEnemy> Pool;
    


    public ShootState(
        bool needsExitTime,
        Enemy Enemy,
        ShootEnemy Prefab,
        Action<State<EnemyState,StateEvent>> onEnter,
        float ExitTime = 0.33f): base(needsExitTime , Enemy , ExitTime, onEnter)
    {
        this.Prefab = Prefab;
        Pool = new(CreateObject, GetObject, ReleaseObject);

    }

    private ShootEnemy CreateObject()
    {
        return GameObject.Instantiate(Prefab);
    }

    private void GetObject(ShootEnemy Instance)
    {
        Instance.transform.forward = Enemy.transform.forward;
        Instance.transform.position = Enemy.transform.position + Enemy.transform.forward + Vector3.up * 1.5f;
        Instance.gameObject.SetActive(true);
        
    }

    private void ReleaseObject(ShootEnemy Instance)
    {
        Instance.gameObject.SetActive(false);
    }
    public override void OnEnter()
    {
        Agent.isStopped = true;
        base.OnEnter();
        Animator.Play("Fire");
        Pool.Get();
    }

}
