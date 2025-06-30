using UnityEngine;

public class AttackStateBehaviorNavEnemy : StateMachineBehaviour
{
    private NavEnemyBase enemyScript;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyScript == null)
            enemyScript = animator.GetComponentInParent<NavEnemyBase>();

        if (enemyScript != null)
            enemyScript.OnAttackStart();
        else
            Debug.LogWarning("NavEnemyBase not found in parent hierarchy.");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyScript != null)
            enemyScript.OnAttackEnd();
    }
}


