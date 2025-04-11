using UnityEngine;

public class EnableDisable_AnimatorController : MonoBehaviour
{
    public Animator animator;
    public string enableTrigger;
    public string disableTrigger;

    private void OnEnable()
    {
        if (animator == null)
            return;

        animator.SetTrigger(enableTrigger);
    }

    private void OnDisable()
    {
        if (animator == null) return;
        float currenTime = Time.time;
        animator.SetTrigger(disableTrigger);
        while (currenTime + 2 < Time.time)
        {

        }
    }
}
