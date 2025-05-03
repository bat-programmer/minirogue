using UnityEngine;

public class AttackStateBehavior : StateMachineBehaviour
{
    // Reference to the enemy script
    private EsqueletoEspada enemyScript;

    // This is called when the state is entered
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyScript == null)
            enemyScript = animator.GetComponent<EsqueletoEspada>();

        // Tell the enemy script that an attack has started
        enemyScript.OnAttackStart();
    }

    // This is called when the state is exited
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Tell the enemy script that the attack has ended
        enemyScript.OnAttackEnd();
    }
}
