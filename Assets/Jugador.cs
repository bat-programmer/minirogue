using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    int movimientoHorizontal = 0;
    int movimientoVertical = 0;
    Vector2 mov = new Vector2(0, 0);

    [SerializeField] private float speed = 100;
    public float baseSpeed;
    float speedShift;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    public Transform firePoint;
    private float fireballSpeed = 10;

    [Header("UI")]
    public Transform heartsContainer; // Drag your container here in the Inspector


    [Header("Health System")]
    public List<int> hearts = new List<int>(); // Each heart can have values: 2 (full), 1 (half), 0 (empty)
    public int maxHearts = 3; // Maximum number of hearts

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 5f;
    [SerializeField] private float blinkInterval = 0.2f;
    private Vector2 lastDirection = Vector2.right; // Default direction
    private PoolBolaDeFuego poolBolaDeFuego;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        poolBolaDeFuego = FindObjectOfType<PoolBolaDeFuego>();

        baseSpeed = speed;
        speedShift = speed * 10;

        InitializeHearts(maxHearts);
        UpdateHeartUI();
    }

    void Update()
    {
        MovH();
        MovV();
        Sprint();
        ExecuteAttack();
        

        if (Input.GetKeyDown(KeyCode.I))
        {
            AddHealth(1); // Add half a heart for testing
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveHealth(1); // Remove half a heart for testing
        }

        mov = new Vector2(movimientoHorizontal, movimientoVertical).normalized;
        if (mov != Vector2.zero)
        {
            lastDirection = mov;
        }

        if (mov.x < 0)
            sr.flipX = true;
        else if (mov.x > 0)
            sr.flipX = false;

        transform.Translate(mov * speed * Time.deltaTime);
    }

    void MovH()
    {
        if (Input.GetKey(KeyCode.D))
            movimientoHorizontal = 1;
        else if (Input.GetKey(KeyCode.A))
            movimientoHorizontal = -1;
        else
            movimientoHorizontal = 0;
    }

    void MovV()
    {
        if (Input.GetKey(KeyCode.W))
            movimientoVertical = 1;
        else if (Input.GetKey(KeyCode.S))
            movimientoVertical = -1;
        else
            movimientoVertical = 0;
    }

    private void ExecuteAttack()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetTrigger("Attack");
            Fire(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetTrigger("Attack");
            Fire(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetTrigger("Attack");
            Fire(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetTrigger("Attack");
            Fire(Vector2.right);
        }
    }


    private void Fire(Vector2 direction)
    {
        if (poolBolaDeFuego != null)
        {
            poolBolaDeFuego.GetFireball(firePoint.position, direction.normalized);
        }
    }

    private void Sprint()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? speedShift : baseSpeed;
    }

    // Initialize hearts with full health
    private void InitializeHearts(int count)
    {
        hearts.Clear();
        for (int i = 0; i < count; i++)
        {
            hearts.Add(2); // Full heart
        }
    }

    // Add health (1 = half heart, 2 = full heart)
    public void AddHealth(int amount)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] < 2)
            {
                hearts[i] += amount;
                if (hearts[i] > 2) hearts[i] = 2; // Cap at full heart
                amount -= 1;
                if (amount <= 0) break;
            }
        }

        UpdateHeartUI();
        Debug.Log("Health added. Current hearts: " + string.Join(", ", hearts));
    }

    // Remove health (1 = half heart, 2 = full heart)
    public void RemoveHealth(int amount)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i] > 0)
            {
                hearts[i] -= amount;
                if (hearts[i] < 0) hearts[i] = 0; // Cap at empty heart
                amount -= 1;
                if (amount <= 0) break;
            }
        }
        UpdateHeartUI();
        Debug.Log("Health removed. Current hearts: " + string.Join(", ", hearts));
    }
    void UpdateHeartUI()
    {
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            var heartUI = heartsContainer.GetChild(i).GetComponent<HeartImage>();

            if (i < hearts.Count)
            {
                heartUI.SetState(hearts[i]);
            }
            else
            {
                heartUI.gameObject.SetActive(false); // Optional: hide extra hearts
            }
        }
    }
    public void ApplyHealth(int amount)
    {
        AddHealth(amount);
        if (IsDead())
        {
            // Handle player death (e.g., restart level, show game over screen)
            Debug.Log("Player is dead!");
        }
    }
    public void ApplyDamage(int damage)
    {
        if (isInvulnerable) return;   

        RemoveHealth(damage);
        if (IsDead())
        {
            // Handle player death (e.g., restart level, show game over screen)
            Debug.Log("Player is dead!");
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    public void AddMoney(int amount)
    {
        // Handle money collection or spending
        Debug.Log("Money handled: " + amount);
    }

    public void RemoveMoney(int amount)
    {
        // Handle money collection or spending
        Debug.Log("Money removed: " + amount);
    }

    public bool IsDead()
    {
        foreach (int heart in hearts)
        {
            if (heart > 0)
                return false; // Player is alive
        }
        return true; // Player is dead
    }
    private IEnumerator InvulnerabilityCoroutine()
    {
    isInvulnerable = true;

    float elapsed = 0f;
    while (elapsed < invulnerabilityDuration)
    {
        sr.enabled = !sr.enabled;
        yield return new WaitForSeconds(blinkInterval);
        elapsed += blinkInterval;
    }

    sr.enabled = true; // Make sure sprite is visible at end
    isInvulnerable = false;
    }

}