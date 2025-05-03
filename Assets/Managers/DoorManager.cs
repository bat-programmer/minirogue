using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door GameObjects")]
    public GameObject topDoor;
    public GameObject rightDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;

    [Header("Door Controllers")]
    public DoorController topDoorController;
    public DoorController rightDoorController;
    public DoorController bottomDoorController;
    public DoorController leftDoorController;

    private void Awake()
    {
        // If door controllers are not assigned, try to get them from door GameObjects
        if (topDoor != null && topDoorController == null)
            topDoorController = topDoor.GetComponent<DoorController>();

        if (rightDoor != null && rightDoorController == null)
            rightDoorController = rightDoor.GetComponent<DoorController>();

        if (bottomDoor != null && bottomDoorController == null)
            bottomDoorController = bottomDoor.GetComponent<DoorController>();

        if (leftDoor != null && leftDoorController == null)
            leftDoorController = leftDoor.GetComponent<DoorController>();
    }

    public void UpdateDoors(bool top, bool right, bool bottom, bool left)
    {
        // Activate/deactivate door GameObjects based on the door flags
        if (topDoor != null) topDoor.SetActive(top);
        if (rightDoor != null) rightDoor.SetActive(right);
        if (bottomDoor != null) bottomDoor.SetActive(bottom);
        if (leftDoor != null) leftDoor.SetActive(left);
    }
}