using UnityEngine;

public class HealthTrader : MonoBehaviour
{
    [Header("Trade Settings")]
    [SerializeField] private int healthCost = 2; // Default: 1 full heart
    [SerializeField] private int coinReward = 2;
    [SerializeField] private bool destroyAfterUse = true;

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem tradeEffect;
    [SerializeField] private string tradeMessage = "Coins +{0}";
    [SerializeField] private Color messageColor = Color.yellow;

    private bool canTrade = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTrade) return;

        Jugador player = other.GetComponent<Jugador>();
        if (player != null)
        {
            // Apply health cost
            player.ApplyDamage(healthCost);
            
            // Give coin reward
            GameManager.Instance.AddCoins(coinReward);
            
            // Visual feedback
            if (tradeEffect != null) tradeEffect.Play();
            FloatingText.Create(transform.position, string.Format(tradeMessage, coinReward), messageColor);
            
            // Disable or destroy
            if (destroyAfterUse)
            {
                Destroy(gameObject);
            }
            else
            {
                canTrade = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
