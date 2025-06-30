using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    
    public static void Create(Vector3 position, string text, Color color)
    {
        GameObject textGO = new GameObject("FloatingText");
        textGO.transform.position = position;

        TextMeshPro textMesh = textGO.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.font = Resources.Load<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/PressStart2P-Regular.ttf"); // Ensure you have the Arial SDF font in your Resources folder
        textMesh.fontSize = 4;
        textMesh.color = color;        
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 1000;
        //textMesh.sortingLayerID = SortingLayer.NameToID("UI"); // Ensure you have a sorting layer named "UI"

        FloatingText floatingText = textGO.AddComponent<FloatingText>();
        floatingText.StartCoroutine(floatingText.AnimateAndDestroy());
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