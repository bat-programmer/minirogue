using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    
    public static void Create(Vector3 position, string text, Color color, int fontSize = 4, bool pulseEffect = false)
    {
        GameObject textGO = new GameObject("FloatingText");
        textGO.transform.position = position;

        TextMeshPro textMesh = textGO.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.font = Resources.Load<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/PressStart2P-Regular.ttf"); // Ensure you have the Arial SDF font in your Resources folder
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 1000;
        //textMesh.sortingLayerID = SortingLayer.NameToID("UI"); // Ensure you have a sorting layer named "UI"

        // Add outline effect
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = Color.black;
        textMesh.fontMaterial.EnableKeyword("OUTLINE_ON");

        FloatingText floatingText = textGO.AddComponent<FloatingText>();
        if (pulseEffect)
        {
            floatingText.StartCoroutine(floatingText.PulseEffect());
        }
        floatingText.StartCoroutine(floatingText.AnimateAndDestroy());
    }

    private IEnumerator PulseEffect()
    {
        float minScale = 1f;
        float maxScale = 1.2f;
        float pulseDuration = 0.5f;

        while (true)
        {
            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(minScale, maxScale, elapsed / pulseDuration);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(maxScale, minScale, elapsed / pulseDuration);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }
        }
    }

    private IEnumerator AnimateAndDestroy()
    {
        Vector3 startPos = transform.position; 
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Move up and fade out
            transform.position = startPos + Vector3.up * progress * 2f;
            GetComponent<TextMeshPro>().alpha = 1f - progress;

            yield return null;
        }

        Destroy(gameObject);
    }
}
