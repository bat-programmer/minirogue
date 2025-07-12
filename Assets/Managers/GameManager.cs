using Assets.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    private GameObject defeatScreen;
    public IPotionEffect currentPotion;
    public PotionUISlot potionUISlot; // Assign in the Inspector
    public static GameManager Instance;
    public Transform playerTransform;

    public int playerHealth = 100;

    // Track discovered potion types
    private Dictionary<System.Type, bool> discoveredPotions = new Dictionary<System.Type, bool>();

    // Track first potion assignment
    public bool HasAssignedFirstPotion { get; set; } = false;

    // Reference to the current potion the player is holding
    public GameObject heldPotion;
    public event System.Action OnAllEnemiesDefeated;
    private int deadEnemies = 0;
    private int totalEnemies = 0;
    public TextMeshProUGUI coinText;
    
    // Lifetime game statistics
    private Dictionary<string, int> lifetimeStats = new Dictionary<string, int>()
    {
        {"totalEnemiesKilled", 0},
        {"totalCoinsCollected", 0},
        {"potionsUsed", 0},
        {"heartsSacrificed", 0},
        {"damageTaken", 0}
    };
    public event System.Action<Dictionary<string, int>> OnStatsUpdated;

    [Header("Coin Drop Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinDropEveryXDeaths = 3;
    [SerializeField][Range(0f, 1f)] private float coinDropChance = 0.5f;

    [Header("Player Money")]
    [SerializeField] private int playerCoins = 0;
    public int PlayerCoins => playerCoins;

    private void Start()
    {
        totalEnemies = FindObjectsOfType<NavEnemyBase>().Length;
        deadEnemies = 0;

        NavEnemyBase.OnAnyEnemyDied += HandleEnemyDeath;
    }
    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"[GameManager] Enemy registered. Total: {totalEnemies}");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public bool IsPotionDiscovered(System.Type potionType)
    {
        return discoveredPotions.ContainsKey(potionType);
    }

    public void DiscoverPotion(IPotionEffect effect)
    {
        discoveredPotions[effect.GetType()] = true;
    }

    public void EquipPotion(IPotionEffect effect, string label, Sprite sprite, Color color)
    {
        Vector3 textPosition = playerTransform != null ? playerTransform.position : transform.position;

        FloatingText.Create(textPosition, "Potion Grabbed!", Color.green);
        Debug.Log("Potion equipped: " + effect.GetType().Name);
        currentPotion = effect;
        Debug.Log("Potion color: " + color.ToString());
        potionUISlot.SetPotion(sprite, color, label);
        
        // Mark as discovered when equipped
        DiscoverPotion(effect);
    }

    public void UsePotion(Jugador player)
    {
        if (currentPotion != null)
        {
            string potionName = currentPotion.GetUILabel();
            currentPotion.ApplyEffect(player);
            currentPotion = null;
            potionUISlot.ClearPotion();
            FloatingText.Create(player.transform.position, $"{potionName} used", Color.yellow);
            IncrementStat("potionsUsed");
        }
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        IncrementStat("totalCoinsCollected", amount);
        Debug.Log($"[GameManager] Coins: {playerCoins}");
        UpdateCoinUI();
    }

    public bool RemoveCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            Debug.Log($"[GameManager] Coins: {playerCoins}");
            UpdateCoinUI();
            return true;
        }

        Debug.Log("[GameManager] Not enough coins!");
        return false;
    }

    public void IncrementStat(string statName, int amount = 1)
    {
        if (lifetimeStats.ContainsKey(statName))
        {
            lifetimeStats[statName] += amount;
            OnStatsUpdated?.Invoke(lifetimeStats);
        }
    }

    public Dictionary<string, int> GetStats()
    {
        return new Dictionary<string, int>(lifetimeStats);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"x {playerCoins}";
        }
    }

    private void OnDestroy()
    {
        NavEnemyBase.OnAnyEnemyDied -= HandleEnemyDeath;
    }
    private void HandleEnemyDeath()
    {
        deadEnemies++;
        IncrementStat("totalEnemiesKilled");

        if (deadEnemies >= totalEnemies)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
        // Try to spawn coin every X deaths
        if (deadEnemies % coinDropEveryXDeaths == 0)
        {
            float roll = Random.value; // Random between 0 and 1
            if (roll < coinDropChance)
            {
                SpawnCoin();
            }
        }
    }
    private void SpawnCoin()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("[GameManager] Coin prefab not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("[GameManager] Player transform not assigned.");
            return;
        }

        // Spawn near the player with slight random offset
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle * 1.5f;
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }

    public void ShowDefeatScreen()
    {
        Debug.Log("Showing defeat screen");
        // Get game stats
        Dictionary<string, int> stats = GetStats();
        
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
        titleText.text = "GAME OVER";
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
            statText.text = $"{stat.Key}: {stat.Value}";
            statText.fontSize = 24;
            RectTransform statRect = statObj.GetComponent<RectTransform>();
            statRect.anchorMin = new Vector2(0.5f, 0.5f);
            statRect.anchorMax = new Vector2(0.5f, 0.5f);
            statRect.anchoredPosition = new Vector2(0, yPos);
            statRect.sizeDelta = new Vector2(300, 30);
            yPos -= 30;
        }
        
        // Create restart button
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(defeatScreen.transform);
        Button button = buttonObj.AddComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "RESTART";
        buttonText.fontSize = 36;
        buttonText.alignment = TextAlignmentOptions.Center;
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.2f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        // Add restart functionality
        // Create "Press Enter" message
        GameObject enterTextObj = new GameObject("EnterText");
        enterTextObj.transform.SetParent(defeatScreen.transform);
        TextMeshProUGUI enterText = enterTextObj.AddComponent<TextMeshProUGUI>();
        enterText.text = "Press Enter to restart";
        enterText.fontSize = 24;
        enterText.alignment = TextAlignmentOptions.Center;
        RectTransform enterRect = enterTextObj.GetComponent<RectTransform>();
        enterRect.anchorMin = new Vector2(0.5f, 0.9f);
        enterRect.anchorMax = new Vector2(0.5f, 0.9f);
        enterRect.anchoredPosition = Vector2.zero;
        enterRect.sizeDelta = new Vector2(400, 50);

        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = Color.red;
        colors.highlightedColor = new Color(1f, 0.5f, 0.5f);
        button.colors = colors;

        button.onClick.AddListener(() => {
            Debug.Log("Restarting game...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        });

        this.defeatScreen = defeatScreen;
    }

    public void ShowWinScreen()
    {
        Debug.Log("Showing win screen");
        // Get game stats
        Dictionary<string, int> stats = GetStats();
        
        // Create UI elements to display stats
        GameObject winScreen = new GameObject("WinScreen");
        Canvas canvas = winScreen.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        winScreen.AddComponent<CanvasScaler>();
        winScreen.AddComponent<GraphicRaycaster>();
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(winScreen.transform);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(winScreen.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "YOU WIN!";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(400, 100);
        
        // Create stats panel
        GameObject statsPanel = new GameObject("StatsPanel");
        statsPanel.transform.SetParent(winScreen.transform);
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
            statText.text = $"{stat.Key}: {stat.Value}";
            statText.fontSize = 24;
            RectTransform statRect = statObj.GetComponent<RectTransform>();
            statRect.anchorMin = new Vector2(0.5f, 0.5f);
            statRect.anchorMax = new Vector2(0.5f, 0.5f);
            statRect.anchoredPosition = new Vector2(0, yPos);
            statRect.sizeDelta = new Vector2(300, 30);
            yPos -= 30;
        }
        
        // Create restart button
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(winScreen.transform);
        Button button = buttonObj.AddComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "RESTART";
        buttonText.fontSize = 36;
        buttonText.alignment = TextAlignmentOptions.Center;
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.2f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        // Add restart functionality
        // Create "Press Enter" message
        GameObject enterTextObj = new GameObject("EnterText");
        enterTextObj.transform.SetParent(winScreen.transform);
        TextMeshProUGUI enterText = enterTextObj.AddComponent<TextMeshProUGUI>();
        enterText.text = "Press Enter to restart";
        enterText.fontSize = 24;
        enterText.alignment = TextAlignmentOptions.Center;
        RectTransform enterRect = enterTextObj.GetComponent<RectTransform>();
        enterRect.anchorMin = new Vector2(0.5f, 0.9f);
        enterRect.anchorMax = new Vector2(0.5f, 0.9f);
        enterRect.anchoredPosition = Vector2.zero;
        enterRect.sizeDelta = new Vector2(400, 50);

        // Create "Press Escape" message
        GameObject escapeTextObj = new GameObject("EscapeText");
        escapeTextObj.transform.SetParent(winScreen.transform);
        TextMeshProUGUI escapeText = escapeTextObj.AddComponent<TextMeshProUGUI>();
        escapeText.text = "Press Escape to return to main menu";
        escapeText.fontSize = 24;
        escapeText.alignment = TextAlignmentOptions.Center;
        RectTransform escapeRect = escapeTextObj.GetComponent<RectTransform>();
        escapeRect.anchorMin = new Vector2(0.5f, 0.1f); // Position it below the restart button
        escapeRect.anchorMax = new Vector2(0.5f, 0.1f);
        escapeRect.anchoredPosition = Vector2.zero;
        escapeRect.sizeDelta = new Vector2(400, 50);

        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = Color.red;
        colors.highlightedColor = new Color(1f, 0.5f, 0.5f);
        button.colors = colors;

        button.onClick.AddListener(() => {
            Debug.Log("Restarting game...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        });

        this.defeatScreen = winScreen; // Re-using defeatScreen variable for simplicity, could be a separate winScreen variable
    }

    private void Update()
    {
        if (defeatScreen != null && Input.GetKeyDown(KeyCode.Return))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        if (defeatScreen != null && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Returning to main menu...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Assuming "MainMenu" is the scene name
        }
    }
}
