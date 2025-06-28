using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private Transform teleporterOut;

    [SerializeField] private List<MonoBehaviour> postTeleportActions; // Objects that implement IPostTeleportAction

    void Start()
    {
        // Find the child named "TeleporterOut"
        teleporterOut = transform.Find("TeleporterOut");
        if (teleporterOut == null)
        {
            Debug.LogError("TeleporterOut child not found!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && teleporterOut != null)
        {
            other.transform.position = teleporterOut.position;

            foreach (var action in postTeleportActions)
            {
                if (action is IPostTeleportAction teleportAction)
                {
                    teleportAction.OnPostTeleport();
                }
            }
        }
    }

}
