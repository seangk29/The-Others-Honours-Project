using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    public int health;
    public bool takeDamage;

    public int removeHealth;

    public float invul;
    public float timer;
    
    
    // Update is called once per frame
    void Update()
    {
        
        invul -= Time.deltaTime;
        
        if (takeDamage && invul <= 0)
        {
            health -= removeHealth;
            takeDamage = false;

            invul = 2f;
        }
    }
}
