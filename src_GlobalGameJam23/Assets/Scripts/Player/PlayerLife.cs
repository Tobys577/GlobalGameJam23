
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : NetworkBehaviour
{
    public int health = 100;

    NetworkObject netObj;
    [HideInInspector] public GameObject diedCanvas;
    private void Start()
    {
        netObj = GetComponent<NetworkObject>();
        if (netObj.HasStateAuthority)
        {
            diedCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (netObj.HasStateAuthority)
        {
            if (health < 0)
            {
                Die();
            }
        }
    }

    public void UpdateHealth(int damage)
    {
        Debug.Log("Dealth Damage " + damage);
        health -= damage;
        RPC_UpdateHealth(health);
    }

    [Rpc]
    public void RPC_UpdateHealth(int newHealth)
    {
        health = newHealth;
    }


    public void callDieNoCanvas()
    {
        RPC_DieNoCanvas();
    }

    [Rpc]
    public void RPC_DieNoCanvas()
    {
        Runner.Despawn(GetComponent<NetworkObject>());
    }

    private void Die()
    {
        if (diedCanvas == null)
            return;

        diedCanvas.SetActive(true);
        Runner.Despawn(GetComponent<NetworkObject>());
    }
}