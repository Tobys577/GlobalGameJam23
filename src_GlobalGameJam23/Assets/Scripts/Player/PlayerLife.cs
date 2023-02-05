using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    int health = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(int damage)
    {
        health -= damage;
    }
}
