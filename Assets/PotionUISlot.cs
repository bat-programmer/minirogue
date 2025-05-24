using UnityEngine;
using UnityEngine.UI;

public class PotionUISlot : MonoBehaviour
{
    public Image potionIcon;

    public void SetPotion(Sprite sprite, Color color)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Sprite is null. Cannot set potion icon.");
            return;
        }
        potionIcon.sprite = sprite;
        potionIcon.color = color;
        potionIcon.enabled = true;
    }

    public void ClearPotion()
    {
        potionIcon.sprite = null;
        potionIcon.enabled = false;
    }
}
