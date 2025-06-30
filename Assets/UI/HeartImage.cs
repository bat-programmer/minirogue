using UnityEngine;
using UnityEngine.UI;

public class HeartImage : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetState(int state)
    {
        switch (state)
        {
            case 2:
                image.sprite = fullHeart;
                break;
            case 1:
                image.sprite = halfHeart;
                break;
            case 0:
                image.sprite = emptyHeart;
                break;
        }
    }
}
