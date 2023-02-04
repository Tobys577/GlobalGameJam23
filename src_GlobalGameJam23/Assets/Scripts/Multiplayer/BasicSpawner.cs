using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static string username = "";

    [SerializeField]
    private int maxSessionSize = 6;

    [SerializeField]
    private NetworkPrefabRef playerPrefabRef;

    [SerializeField]
    private NetworkPrefabRef characterSelectionScreenPref;

    [SerializeField]
    private GameObject connectingObj;

    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private NetworkRunner runner;

    private CharacterSelectionScreen characterSelectionScreen;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        StartGame(GameMode.Shared);
    }

    private async void StartGame(GameMode mode)
    {
        connectingObj.SetActive(true);

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // No name given = Create / Random join session.
        // Each session will play to max
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = maxSessionSize
        });

        // Use to make session not joinable when in game:
        // runner.SessionInfo.IsOpen = false;
    }

    private IEnumerator timerCountDown()
    {
        int timer = 120;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            characterSelectionScreen.callUpdateTimer(timer);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        connectingObj.SetActive(false);

        if (runner.SessionInfo.PlayerCount == 1)
        {
            print("count down started");
            NetworkObject networkedCharacterSelection = runner.Spawn(characterSelectionScreenPref, Vector2.zero, Quaternion.identity, runner.LocalPlayer);
            characterSelectionScreen = networkedCharacterSelection.GetComponent<CharacterSelectionScreen>();
            StartCoroutine(timerCountDown());
        } else
        {
            print(runner.SessionInfo.PlayerCount);
        }

        if (runner.LocalPlayer == player)
        {
            characterSelectionScreen = FindObjectOfType<CharacterSelectionScreen>();
            characterSelectionScreen.isAttackingSide = !(runner.SessionInfo.PlayerCount % 2 == 0);
            characterSelectionScreen.iconNumber = (int)(runner.SessionInfo.PlayerCount / 2);
            if (!characterSelectionScreen)
                characterSelectionScreen.iconNumber--;

            characterSelectionScreen.characterID = -1;
        }
    }

    [ContextMenu("Spawn Player")]
    public void SpawnPlayer()
    {
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefabRef, Vector2.zero, Quaternion.identity, runner.LocalPlayer);
        spawnedCharacters.Add(runner.LocalPlayer, networkPlayerObject);

        characterSelectionScreen.gameObject.SetActive(false);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(spawnedCharacters.TryGetValue(player, out NetworkObject outedNetworkObject))
        {
            runner.Despawn(outedNetworkObject);
            spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }
}
