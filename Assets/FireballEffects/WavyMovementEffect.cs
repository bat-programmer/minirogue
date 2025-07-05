// Wavy movement effect
using UnityEngine;

public class WavyMovementEffect : FireballEffect
{
    [SerializeField] private float waveAmplitude = 1f;
    [SerializeField] private float waveFrequency = 10f;
    private Vector2 originalDirection;
    private Vector2 basePosition;
    private float timeElapsed = 0f;
    private Vector2 lastPosition;

    public override void ApplyEffect()
    {
        originalDirection = fireball.GetDirection();
        basePosition = fireball.transform.position;
        lastPosition = basePosition;
    }

    public override void UpdateEffect()
    {
        timeElapsed += Time.deltaTime;

        // Calculate how far we should have moved in the original direction
        float forwardDistance = fireball.speed * timeElapsed;
        Vector2 forwardMovement = originalDirection * forwardDistance;

        // Create perpendicular vector for wave motion
        Vector2 perpendicular = new Vector2(-originalDirection.y, originalDirection.x);
        float waveOffset = Mathf.Sin(timeElapsed * waveFrequency) * waveAmplitude;

        // Calculate new position
        Vector2 newPosition = basePosition + forwardMovement + perpendicular * waveOffset;

        // Update fireball position directly
        fireball.transform.position = newPosition;

        // Update direction based on movement (for sprite flipping)
        Vector2 movementDirection = (newPosition - lastPosition).normalized;
        if (movementDirection != Vector2.zero)
        {
            fireball.SetDirection(movementDirection);
        }

        lastPosition = newPosition;
    }
}

// Speed and damage modifier effect
public class SpeedDamageModifierEffect : FireballEffect
{
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float damageMultiplier = 1f;
    private float originalSpeed;
    private int originalDamage;

    public void SetModifiers(float speedMod, float damageMod)
    {
        speedMultiplier = speedMod;
        damageMultiplier = damageMod;
    }

    public override void ApplyEffect()
    {
        originalSpeed = fireball.speed;
        originalDamage = fireball.damage;

        fireball.speed *= speedMultiplier;
        fireball.damage = Mathf.RoundToInt(fireball.damage * damageMultiplier);
    }

    public override void RemoveEffect()
    {
        fireball.speed = originalSpeed;
        fireball.damage = originalDamage;
        isActive = false;
    }
}

// Tracking effect
public class TrackingEffect : FireballEffect
{
    [SerializeField] private float trackingRange = 10f;
    [SerializeField] private float trackingStrength = 2f;
    private Transform target;

    public override void ApplyEffect()
    {
        FindNearestEnemy();
    }

    public override void UpdateEffect()
    {
        if (target == null)
        {
            FindNearestEnemy();
            return;
        }

        // Calculate direction to target
        Vector2 directionToTarget = (target.position - fireball.transform.position).normalized;
        Vector2 currentDirection = fireball.GetDirection();

        // Blend current direction with target direction
        Vector2 newDirection = Vector2.Lerp(currentDirection, directionToTarget, trackingStrength * Time.deltaTime);
        fireball.SetDirection(newDirection.normalized);
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = trackingRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(fireball.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.transform;
            }
        }
    }
}

// Bounce effect
public class BounceEffect : FireballEffect
{
    [SerializeField] private int maxBounces = 3;
    private int currentBounces = 0;

    public override void ApplyEffect()
    {
        // Modify the fireball's collision behavior
        fireball.OnWallHit += HandleBounce;
    }

    private void HandleBounce(Vector2 hitNormal)
    {
        if (currentBounces < maxBounces)
        {
            Vector2 currentDirection = fireball.GetDirection();
            Vector2 bounceDirection = Vector2.Reflect(currentDirection, hitNormal);
            fireball.SetDirection(bounceDirection);
            currentBounces++;
        }
    }

    public override void RemoveEffect()
    {
        fireball.OnWallHit -= HandleBounce;
        isActive = false;
    }
}