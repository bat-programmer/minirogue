using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jugador : MonoBehaviour
{
    private MovementController movementController;
    private Animator anim;
    private SpriteRenderer sr;
    private PlayerAttackController attackController;
    private PlayerHealth playerHealth;

    [Header("UI")]
    public Transform heartsContainer; // Drag your container here in the Inspector

    [Header("Events")]
    public System.Action OnDamageTaken;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        movementController = gameObject.AddComponent<MovementController>();
        movementController.Initialize(GetComponent<Rigidbody2D>());
        movementController.Speed = 10f;

        playerHealth = gameObject.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = gameObject.AddComponent<PlayerHealth>();
            playerHealth.heartsContainer = heartsContainer;
            playerHealth.maxHearts = 3;
        }

        attackController = gameObject.AddComponent<PlayerAttackController>();
        attackController.Initialize(anim, transform.Find("FirePoint"), FindObjectOfType<PoolBolaDeFuego>());

        GameManager.Instance.playerTransform = transform; // Set player transform in GameManager
    }

    void Update()
    {
        movementController.UpdateMovement();
        attackController.HandleAttackInput();
        usePotion();
    }

    private KeyCode GetKeyBinding(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
            return (KeyCode)PlayerPrefs.GetInt(prefKey);
        return defaultKey;
    }

    private void usePotion()
    {
        KeyCode potionKey = GetKeyBinding("UsePotion", KeyCode.E);
        if (Input.GetKeyDown(potionKey))
        {
            GameManager.Instance.UsePotion(this);
        }
    }

    public void AddMoney(int amount)
    {
        GameManager.Instance.AddCoins(amount);
    }

    public void RemoveMoney(int amount)
    {
        GameManager.Instance.RemoveCoins(amount);
    }

    // Health management methods - delegate to PlayerHealth
    public void ApplyDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(damage);
        }
    }

    public void ApplyHealth(int amount)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyHealth(amount);
        }
    }

    public bool IsDead()
    {
        return playerHealth != null && playerHealth.IsDead();
    }

    public void StartVictoryDance()
    {
        DisableCollider();
        StartCoroutine(VictoryDance());
    }

    private void DisableCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator VictoryDance()
    {
        // Disable movement
        if (movementController != null)
        {
            movementController.enabled = false;
        }

        // Get initial values
        Vector3 originalScale = transform.localScale;

        while (true)
        {
            // Rotate
            transform.Rotate(0f, 0f, 360f * Time.deltaTime);

            // Pulse size
            float pulse = Mathf.PingPong(Time.time * 2f, 0.5f) + 0.5f;
            transform.localScale = originalScale * pulse;

            yield return null;
        }
    }
}
