using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionUISlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI potionLabel;
    public Image potionIcon;

    public void SetPotion(Sprite sprite, Color color, string label = "Health Potion")
    {
        if (sprite == null)
        {
            Debug.LogWarning("Sprite is null. Cannot set potion icon.");
            return;
        }
        potionIcon.sprite = sprite;
        potionIcon.color = color;
        potionIcon.enabled = true;

        if (potionLabel != null)
        {
            potionLabel.text = label;
            potionLabel.enabled = true;
        }
    }

    public void ClearPotion()
    {
        potionIcon.sprite = null;
        potionIcon.enabled = false;

        if (potionLabel != null)
        {
            potionLabel.text = "";
            potionLabel.enabled = false;
        }
    }
}