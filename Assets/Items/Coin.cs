using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assuming you have a method to add coins to the player's inventory
            //PlayerInventory.AddCoins(coinValue);
            collision.gameObject.GetComponent<Jugador>().AddMoney(1); // Example: Add 1 coin to the player's inventory
            Debug.Log("Coin collected!");
            Destroy(gameObject); // Destroy the coin after collection
        }
    }
}
