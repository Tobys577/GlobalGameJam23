using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private NetworkPrefabRef playerPrefabRef;

    public override void Spawned()
    {
        Debug.Log("Spawned called");

        NetworkObject playerObject = Runner.Spawn(playerPrefabRef, transform.position, Quaternion.identity, Runner.LocalPlayer);
    }
}
