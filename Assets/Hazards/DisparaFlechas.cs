using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparaFlechas : MonoBehaviour
{
    public GameObject flechaPrefab; // Assign the Flecha prefab in the Inspector
    public float fireInterval = 2f;

    // Start is called before the first frame update
    void Start()
    {
        flechaPrefab = Resources.Load<GameObject>("Projectiles/Flecha");

        if (flechaPrefab == null)
        {
            Debug.LogError("Projectile prefab not found in Resources/Projectiles/Flecha");
        }
        StartCoroutine(FireArrows());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FireArrows()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            Instantiate(flechaPrefab, transform.position, Quaternion.identity);
           
        }
    }
}
