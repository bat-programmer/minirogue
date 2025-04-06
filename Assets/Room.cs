using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Exit
}

public class Room : MonoBehaviour
{
    public RoomType roomType;
    public Vector2Int roomSize = new Vector2Int(16, 16); // Tile dimensions

    public bool doorTop;
    public bool doorBottom;
    public bool doorLeft;
    public bool doorRight;
}
