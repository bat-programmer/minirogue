using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EsqueleMago : EnemyBase
{
    protected override void Attack()
    {
        if (isInAttackState || Time.time - lastAttackTime < attackCooldown)
            return;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        FacePlayer();
        yield return StartCoroutine(FlickerSprite(0.1f, 6, Color.red));

        animator.SetTrigger("IsAttacking");
        lastAttackTime = Time.time;    
    }


    private IEnumerator FlickerSprite(float flickerInterval, int flickerCount, Color flickerColor)
    {
        Color originalColor = Color.white;

        for (int i = 0; i < flickerCount; i++)
        {
            spriteRenderer.color = (i % 2 == 0) ? flickerColor : originalColor;
            yield return new WaitForSeconds(flickerInterval);
        }
        spriteRenderer.color = originalColor;
    }
}
