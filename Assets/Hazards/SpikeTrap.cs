using UnityEngine;
using System.Collections; // Add this for coroutines

public class SpikeTrap : MonoBehaviour
{
    public enum SpikeState
    {
        Retracted,  // Spike is fully retracted and safe
        Pulsing,    // Spike cycles through animation
        Extended    // Spike is fully extended and dangerous
    }

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spikeFrames; // Array of 4 frames

    [Header("Settings")]
    [SerializeField] private SpikeState currentState = SpikeState.Retracted;
    [SerializeField] private float animationSpeed = 4f; // Frames per second
    [SerializeField] private int damageAmount = 1;

    [Header("Timing")]
    [SerializeField] private float startDelay = 0f; // Add this field

    // Animation frame indices where spike is dangerous when pulsing (0-based)
    [SerializeField] private int[] dangerousFrameIndices = { 2, 3 };

    private float animationTimer = 0f;
    private int currentFrame = 0;
    private bool isPlayerInRange = false;
    private GameObject player;

    private void Start()
    {
        // Initialize the spike's appearance based on its state
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // If the trap should start pulsing, delay it
        if (currentState == SpikeState.Pulsing && startDelay > 0f)
        {
            currentState = SpikeState.Retracted;
            UpdateSpikeState();
            StartCoroutine(StartPulsingAfterDelay());
        }
        else
        {
            UpdateSpikeState();
        }
    }

    private IEnumerator StartPulsingAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        SetSpikeState(SpikeState.Pulsing);
    }

    private void Update()
    {
        if (currentState == SpikeState.Pulsing)
        {
            HandlePulsingAnimation();
        }

        if (isPlayerInRange)
        {
            CheckPlayerDamage();
        }
    }

    public void SetSpikeState(SpikeState newState)
    {
        currentState = newState;
        UpdateSpikeState();
    }

    private void UpdateSpikeState()
    {
        switch (currentState)
        {
            case SpikeState.Retracted:
                // Set to first frame
                spriteRenderer.sprite = spikeFrames[0];
                currentFrame = 0;
                break;

            case SpikeState.Extended:
                // Set to last frame
                spriteRenderer.sprite = spikeFrames[spikeFrames.Length - 1];
                currentFrame = spikeFrames.Length - 1;
                break;

            case SpikeState.Pulsing:
                // Animation will be handled in Update
                animationTimer = 0f;
                currentFrame = 0;
                spriteRenderer.sprite = spikeFrames[currentFrame];
                break;
        }
    }

    private void HandlePulsingAnimation()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= 1f / animationSpeed)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % spikeFrames.Length;
            spriteRenderer.sprite = spikeFrames[currentFrame];
        }
    }

    private void CheckPlayerDamage()
    {
        bool isDangerous = false;

        // Determine if spike is currently dangerous
        if (currentState == SpikeState.Extended)
        {
            isDangerous = true;
        }
        else if (currentState == SpikeState.Pulsing)
        {
            // Check if current frame is in the dangerous frames list
            foreach (int dangerousFrame in dangerousFrameIndices)
            {
                if (currentFrame == dangerousFrame)
                {
                    isDangerous = true;
                    break;
                }
            }
        }

        if (isDangerous && player != null)
        {
            //apply damage to player
            //
            // Assuming the player has a method to apply damage
            player.GetComponent<Jugador>().ApplyDamage(damageAmount);

            Debug.Log($"Player took {damageAmount} damage from spike trap!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }
}