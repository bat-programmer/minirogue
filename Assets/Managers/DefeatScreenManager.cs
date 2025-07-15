using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefeatScreenManager : MonoBehaviour
{
    private GameObject defeatScreenInstance;

    public void ShowDefeatScreen(Dictionary<string, int> stats)
    {
        Debug.Log("Showing defeat screen");
        
        // Create UI elements to display stats
        GameObject defeatScreen = new GameObject("DefeatScreen");
        Canvas canvas = defeatScreen.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        defeatScreen.AddComponent<CanvasScaler>();
        defeatScreen.AddComponent<GraphicRaycaster>();
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(defeatScreen.transform);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(defeatScreen.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "GAME OVER :(";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(400, 100);
        
        // Create stats panel
        GameObject statsPanel = new GameObject("StatsPanel");
        statsPanel.transform.SetParent(defeatScreen.transform);
        RectTransform statsRect = statsPanel.AddComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.5f, 0.5f);
        statsRect.anchorMax = new Vector2(0.5f, 0.5f);
        statsRect.anchoredPosition = Vector2.zero;
        statsRect.sizeDelta = new Vector2(400, 300);
        
        // Create stat entries
        float yPos = 100;
        foreach (var stat in stats)
        {
            GameObject statObj = new GameObject(stat.Key);
            statObj.transform.SetParent(statsPanel.transform);
            TextMeshProUGUI statText = statObj.AddComponent<TextMeshProUGUI>();
            string displayName = StatDisplayNames.DisplayNames.ContainsKey(stat.Key) ? StatDisplayNames.DisplayNames[stat.Key] : stat.Key;
            statText.text = $"{displayName}: {stat.Value}";
            statText.fontSize = 24;
            RectTransform statRect = statObj.GetComponent<RectTransform>();
            statRect.anchorMin = new Vector2(0.5f, 0.5f);
            statRect.anchorMax = new Vector2(0.5f, 0.5f);
            statRect.anchoredPosition = new Vector2(0, yPos);
            statRect.sizeDelta = new Vector2(300, 30);
            yPos -= 30;
        }
        
        // Create "Press Enter" message
        GameObject enterTextObj = new GameObject("EnterText");
        enterTextObj.transform.SetParent(defeatScreen.transform);
        TextMeshProUGUI enterText = enterTextObj.AddComponent<TextMeshProUGUI>();
        enterText.text = "Press Enter to restart";
        enterText.fontSize = 24;
        enterText.alignment = TextAlignmentOptions.Center;
        RectTransform enterRect = enterTextObj.GetComponent<RectTransform>();
        enterRect.anchorMin = new Vector2(0.5f, 0.9f);
        enterRect.anchorMax = new Vector2(0.5f, 0.5f);
        enterRect.anchoredPosition = Vector2.zero;
        enterRect.sizeDelta = new Vector2(400, 50);

        // Create "Press Escape" message
        GameObject escapeTextObj = new GameObject("EscapeText");
        escapeTextObj.transform.SetParent(defeatScreen.transform);
        TextMeshProUGUI escapeText = escapeTextObj.AddComponent<TextMeshProUGUI>();
        escapeText.text = "Presiona Escape para volver al menú principal";
        escapeText.fontSize = 24;
        escapeText.alignment = TextAlignmentOptions.Center;
        RectTransform escapeRect = escapeTextObj.GetComponent<RectTransform>();
        escapeRect.anchorMin = new Vector2(0.5f, 0.2f); 
        escapeRect.anchorMax = new Vector2(0.5f, 0.2f);
        escapeRect.anchoredPosition = Vector2.zero;
        escapeRect.sizeDelta = new Vector2(400, 50);

        defeatScreenInstance = defeatScreen;
    }

    private void Update()
    {
        if (defeatScreenInstance != null && Input.GetKeyDown(KeyCode.Return))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
