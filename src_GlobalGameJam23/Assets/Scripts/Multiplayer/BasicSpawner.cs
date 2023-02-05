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
    private NetworkPrefabRef roundManagementScreenPref;

    [SerializeField]
    private GameObject connectingObj;

    private NetworkRunner runner;

    [HideInInspector] public CharacterSelectionScreen characterSelectionScreen;
    [HideInInspector] public RoundManagement roundManagementScreen;

    [HideInInspector] public int timer = 120;

    public GameObject diedCanvas;

    private Dictionary<int, KeyValuePair<bool, int>> playerCountDict = new Dictionary<int, KeyValuePair<bool, int>>()
    {
        { 1, new KeyValuePair<bool, int>(true, 0) },
        { 2, new KeyValuePair<bool, int>(false, 0) },
        { 3, new KeyValuePair<bool, int>(true, 1) },
        { 4, new KeyValuePair<bool, int>(false, 1) },
        { 5, new KeyValuePair<bool, int>(true, 2) },
        { 6, new KeyValuePair<bool, int>(false, 2) }
    };

    private void Start()
    {
        diedCanvas.SetActive(false);

        StartGame();
    }

    public void StartGame()
    {
        StartGame(GameMode.Shared);
    }

    public void ExitToMenu()
    {
        runner.Shutdown();
        SceneManager.LoadScene("Menu");
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
        timer = 120;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
            characterSelectionScreen.callUpdateTimer(timer);

            bool allLocked = true;
            for(int i = 0; i < 6 && allLocked != false; i++)
            {
                if(i % 2 == 0)
                {
                    if (characterSelectionScreen.lockedInAttacking[i] == -1)
                        allLocked = false;
                } else
                {
                    if (characterSelectionScreen.lockedInDefending[i] == -1)
                        allLocked = false;
                }
            }

            if (allLocked)
                timer = 0;
        }

        if (runner.SessionInfo.PlayerCount <= 1)
        {
            ExitToMenu();
        } else
        {
            characterSelectionScreen.callSpawnPlayers();
            runner.SessionInfo.IsOpen = false;
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

            NetworkObject networkedRoundManagement = runner.Spawn(roundManagementScreenPref, Vector2.zero, Quaternion.identity, runner.LocalPlayer);
            roundManagementScreen = networkedRoundManagement.GetComponent<RoundManagement>();
            roundManagementScreen.isHost = true;
            StartCoroutine(timerCountDown());
        }
        else
        {
            print(runner.SessionInfo.PlayerCount);
        }

        if (runner.LocalPlayer == player)
        {
            characterSelectionScreen = FindObjectOfType<CharacterSelectionScreen>();
            print(runner.SessionInfo.PlayerCount);
            characterSelectionScreen.isAttackingSide = playerCountDict[runner.SessionInfo.PlayerCount].Key;
            characterSelectionScreen.iconNumber = playerCountDict[runner.SessionInfo.PlayerCount].Value;
            print(playerCountDict[runner.SessionInfo.PlayerCount].Key.ToString() + " ~ " + playerCountDict[runner.SessionInfo.PlayerCount].Value.ToString());
            characterSelectionScreen.characterID = -1;
        }
    }

    [ContextMenu("Spawn Player")]
    public NetworkObject SpawnPlayer(Vector2 location)
    {
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefabRef, location, Quaternion.identity, runner.LocalPlayer);

        return networkPlayerObject;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

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
