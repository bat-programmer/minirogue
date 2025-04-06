using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftDoorClosed;
    [SerializeField] private SpriteRenderer rightDoorClosed;
    [SerializeField] private SpriteRenderer leftDoorOpen;
    [SerializeField] private SpriteRenderer rightDoorOpen;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool autoClose = true;
    [SerializeField] private float autoCloseDelay = 1.0f;

    private bool isOpen = false;
    private float closeTimer = 0f;

    private void Start()
    {
        UpdateDoorVisuals();
    }

    private void Update()
    {
        // Auto-close the door after delay if enabled
        if (isOpen && autoClose)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0)
            {
                isOpen = false;
                UpdateDoorVisuals();
            }
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        UpdateDoorVisuals();

        if (isOpen && autoClose)
        {
            closeTimer = autoCloseDelay;
        }
        Debug.Log("Door toggled: " + (isOpen ? "Opened" : "Closed"));
    }

    private void UpdateDoorVisuals()
    {
        leftDoorClosed.gameObject.SetActive(!isOpen);
        rightDoorClosed.gameObject.SetActive(!isOpen);
        leftDoorOpen.gameObject.SetActive(isOpen);
        rightDoorOpen.gameObject.SetActive(isOpen);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isOpen)
        {
            ToggleDoor();
        }
    }
}