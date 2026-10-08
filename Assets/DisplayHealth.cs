using TMPro;
using UnityEngine;

public class DisplayHealth : MonoBehaviour
{

    public TextMeshProUGUI health;
    public PlayerHealth player;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health.text = "Health: " + player.health;
    }
}
