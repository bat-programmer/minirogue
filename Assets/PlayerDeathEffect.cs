using UnityEngine;
using System.Collections.Generic;

public class PlayerDeathEffect : MonoBehaviour
{
    public void TriggerDeathEffect(SpriteRenderer playerRenderer)
    {
        if (playerRenderer == null || playerRenderer.sprite == null)
        {
            Debug.LogWarning("PlayerDeathEffect: Player sprite renderer is missing");
            return;
        }

        // Split the sprite into fragments
        Texture2D originalTexture = playerRenderer.sprite.texture;
        int fragmentSize = 8; // Size of each fragment in pixels
        int width = originalTexture.width;
        int height = originalTexture.height;
        float pixelsPerUnit = playerRenderer.sprite.pixelsPerUnit;

        GameObject fragmentsParent = new GameObject("DeathFragments");
        fragmentsParent.transform.position = transform.position;

        for (int y = 0; y < height; y += fragmentSize)
        {
            for (int x = 0; x < width; x += fragmentSize)
            {
                // Calculate fragment dimensions
                int fragWidth = Mathf.Min(fragmentSize, width - x);
                int fragHeight = Mathf.Min(fragmentSize, height - y);
                
                // Create texture fragment
                Rect rect = new Rect(x, y, fragWidth, fragHeight);
                Sprite fragmentSprite = Sprite.Create(originalTexture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
                
                // Create fragment GameObject
                GameObject fragment = new GameObject($"Fragment_{x}_{y}");
                fragment.transform.position = transform.position + new Vector3(
                    (x - width/2f) / pixelsPerUnit, 
                    (y - height/2f) / pixelsPerUnit, 
                    0
                );
                fragment.transform.SetParent(fragmentsParent.transform);
                
                // Add renderer and collider
                SpriteRenderer fragRenderer = fragment.AddComponent<SpriteRenderer>();
                fragRenderer.sprite = fragmentSprite;
                fragRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                fragRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
                
                // Add physics
                Rigidbody2D rb = fragment.AddComponent<Rigidbody2D>();
                BoxCollider2D collider = fragment.AddComponent<BoxCollider2D>();
                
                // Apply explosion force
                Vector2 forceDir = (fragment.transform.position - transform.position).normalized;
                float force = Random.Range(5f, 15f);
                rb.AddForce(forceDir * force, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-20f, 20f));
                
                // Self-destruct after delay
                Destroy(fragment, Random.Range(2f, 4f));
            }
        }
        
        // Destroy parent after all fragments are gone
        Destroy(fragmentsParent, 5f);
    }
}
