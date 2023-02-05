using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : NetworkBehaviour
{
    public int health = 100;

    NetworkObject netObj;
    GameObject diedCanvas;
    private void Start()
    {
        netObj = GetComponent<NetworkObject>();
        if (netObj.HasStateAuthority)
        {
            diedCanvas = GameObject.Find("Died");
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
                Destroy(gameObject);
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

    private void OnDestroy()
    {
        if (diedCanvas == null)
            return;

        diedCanvas.SetActive(true);
        Runner.Despawn(GetComponent<NetworkObject>());
    }
}
