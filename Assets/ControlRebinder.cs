using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlRebinder : MonoBehaviour
{
    [Header("UI Elements")]
    public Button moveUpButton;
    public Button moveDownButton;
    public Button moveLeftButton;
    public Button moveRightButton;
    public Button attackUpButton;
    public Button attackDownButton;
    public Button attackLeftButton;
    public Button attackRightButton;
    public Button usePotionButton;
    public Button saveButton;
    public Button defaultsButton;
    public Button backButton;

    [Header("Rebinding UI")]
    public GameObject rebindPanel;
    public TMP_Text rebindPrompt;

    private string currentRebindKey;
    private bool isRebinding = false;

    void Start()
    {
        // Set up button click handlers
        moveUpButton.onClick.AddListener(() => StartRebinding("MoveUp", KeyCode.W));
        moveDownButton.onClick.AddListener(() => StartRebinding("MoveDown", KeyCode.S));
        moveLeftButton.onClick.AddListener(() => StartRebinding("MoveLeft", KeyCode.A));
        moveRightButton.onClick.AddListener(() => StartRebinding("MoveRight", KeyCode.D));
        attackUpButton.onClick.AddListener(() => StartRebinding("AttackUp", KeyCode.UpArrow));
        attackDownButton.onClick.AddListener(() => StartRebinding("AttackDown", KeyCode.DownArrow));
        attackLeftButton.onClick.AddListener(() => StartRebinding("AttackLeft", KeyCode.LeftArrow));
        attackRightButton.onClick.AddListener(() => StartRebinding("AttackRight", KeyCode.RightArrow));
        usePotionButton.onClick.AddListener(() => StartRebinding("UsePotion", KeyCode.E));

        saveButton.onClick.AddListener(SaveBindings);
        defaultsButton.onClick.AddListener(ResetToDefaults);
        backButton.onClick.AddListener(BackToMenu);

        // Initialize button labels
        UpdateButtonLabels();
    }

    void Update()
    {
        if (isRebinding)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // Don't allow mouse buttons or special keys
                        if (!IsForbiddenKey(keyCode))
                        {
                            PlayerPrefs.SetInt(currentRebindKey, (int)keyCode);
                            isRebinding = false;
                            rebindPanel.SetActive(false);
                            UpdateButtonLabels();
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool IsForbiddenKey(KeyCode keyCode)
    {
        // Prevent binding mouse buttons or special keys
        return keyCode == KeyCode.Mouse0 || 
               keyCode == KeyCode.Mouse1 ||
               keyCode == KeyCode.Mouse2 ||
               keyCode == KeyCode.Escape;
    }

    private void StartRebinding(string keyName, KeyCode defaultKey)
    {
        currentRebindKey = keyName;
        isRebinding = true;
        rebindPanel.SetActive(true);
        rebindPrompt.text = $"Press a key for {keyName}...\n(Current: {GetKeyName(keyName, defaultKey)})";
    }

    private string GetKeyName(string prefKey, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(prefKey))
            return ((KeyCode)PlayerPrefs.GetInt(prefKey)).ToString();
        return defaultKey.ToString();
    }

    private void UpdateButtonLabels()
    {
        UpdateButtonLabel(moveUpButton, "MoveUp", KeyCode.W);
        UpdateButtonLabel(moveDownButton, "MoveDown", KeyCode.S);
        UpdateButtonLabel(moveLeftButton, "MoveLeft", KeyCode.A);
        UpdateButtonLabel(moveRightButton, "MoveRight", KeyCode.D);
        UpdateButtonLabel(attackUpButton, "AttackUp", KeyCode.UpArrow);
        UpdateButtonLabel(attackDownButton, "AttackDown", KeyCode.DownArrow);
        UpdateButtonLabel(attackLeftButton, "AttackLeft", KeyCode.LeftArrow);
        UpdateButtonLabel(attackRightButton, "AttackRight", KeyCode.RightArrow);
        UpdateButtonLabel(usePotionButton, "UsePotion", KeyCode.E);
    }

    private void UpdateButtonLabel(Button button, string keyName, KeyCode defaultKey)
    {
        if (button == null) return;
        
        var tmpText = button.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = GetKeyName(keyName, defaultKey);
        }
        else
        {
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = GetKeyName(keyName, defaultKey);
            }
        }
    }

    private void SaveBindings()
    {
        PlayerPrefs.Save();
        // Optional: Show save confirmation
    }

    private void ResetToDefaults()
    {
        // Clear all custom bindings
        PlayerPrefs.DeleteKey("MoveUp");
        PlayerPrefs.DeleteKey("MoveDown");
        PlayerPrefs.DeleteKey("MoveLeft");
        PlayerPrefs.DeleteKey("MoveRight");
        PlayerPrefs.DeleteKey("AttackUp");
        PlayerPrefs.DeleteKey("AttackDown");
        PlayerPrefs.DeleteKey("AttackLeft");
        PlayerPrefs.DeleteKey("AttackRight");
        PlayerPrefs.DeleteKey("UsePotion");
        
        UpdateButtonLabels();
    }

    private void BackToMenu()
    {
        // Implement scene transition back to main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
