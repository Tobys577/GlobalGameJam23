using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RoundManagement : NetworkBehaviour
{
    public GameObject uiParent;

    public Dictionary<KeyValuePair<bool, int>, NetworkObject> playerObjects = new Dictionary<KeyValuePair<bool, int>, NetworkObject>();

    [HideInInspector] public bool isHost = false;

    private Transform[] attackSideSpawnPoints = new Transform[3];
    private Transform[] defenceSideSpawnPoints = new Transform[3];
    private BasicSpawner basicSpawner;

    public int currentRound = 0;

    public void Start()
    {
        uiParent.SetActive(false);
        basicSpawner = GameObject.Find("BasicSpawner").GetComponent<BasicSpawner>();

        GameObject attackSPs = GameObject.Find("SpawnPointsAttacking");
        for(int i = 0; i < 3; i++)
        {
            attackSideSpawnPoints[i] = attackSPs.transform.GetChild(i);
        }

        GameObject defenceSPs = GameObject.Find("SpawnPointsDefending");
        for (int i = 0; i < 3; i++)
        {
            defenceSideSpawnPoints[i] = defenceSPs.transform.GetChild(i);
        }
    }

    public void StartRound()
    {
        RPC_StartRound();
        callSpawnPlayers();
    }

    public void callSpawnPlayers()
    {
        RPC_SpawnPlayer();
    }

    [Rpc]
    public void RPC_StartRound()
    {
        uiParent.SetActive(true);
        basicSpawner.characterSelectionScreen.gameObject.SetActive(false);
    }

    [Rpc]
    public void RPC_SpawnPlayer()
    {
        bool isAttacking = basicSpawner.characterSelectionScreen.isAttackingSide;
        int sideId = basicSpawner.characterSelectionScreen.iconNumber;
        NetworkObject player = basicSpawner.SpawnPlayer(isAttacking ? attackSideSpawnPoints[sideId].position : defenceSideSpawnPoints[sideId].position);
        player.GetComponent<PlayerMovement>().bodySprite.sprite = basicSpawner.characterSelectionScreen.characters[
            basicSpawner.characterSelectionScreen.characterID].characterSprite;
        player.GetComponent<Gun>().attacking = basicSpawner.characterSelectionScreen.isAttackingSide;
        gameObject.SetActive(false);
        RPC_AddPlayerDict(isAttacking, sideId, player.Id);
    }

    [Rpc]
    public void RPC_AddPlayerDict(bool attacking, int id, NetworkId netId)
    {
        playerObjects.Add(new KeyValuePair<bool, int>(attacking, id), Runner.FindObject(netId));
        print("Played added: " + playerObjects.Count.ToString());
    }

    void EndRound()
    {
        uiParent.SetActive(false);
        currentRound++;
    }
}
