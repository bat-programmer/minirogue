using UnityEngine;

public class AttackStateBehavior : StateMachineBehaviour
{
    // Reference to the enemy script (using base class)
    private NavEnemyBase enemyScript;

    // This is called when the state is entered
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyScript == null)
        {
            // Try current GameObject first, then parent
            enemyScript = animator.GetComponent<NavEnemyBase>();
            if (enemyScript == null)
            {
                enemyScript = animator.GetComponentInParent<NavEnemyBase>();
            }
        }

        if (enemyScript != null)
        {
            enemyScript.OnAttackStart();
        }
        else
        {
            Debug.LogError($"NavEnemyBase script not found on {animator.gameObject.name} or its parents!");
        }
    }

    // This is called when the state is exited
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyScript != null)
        {
            enemyScript.OnAttackEnd();
        }
    }
}
