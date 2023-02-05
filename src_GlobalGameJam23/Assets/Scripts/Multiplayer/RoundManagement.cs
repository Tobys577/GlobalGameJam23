using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class RoundManagement : NetworkBehaviour
{
    public GameObject uiParent;
    public TMP_Text roundText;

    public GameObject countDownPanel;
    public TMP_Text countDownText;

    [SerializeField]
    private int roundCountDownTime;

    [HideInInspector]
    [Networked] public int Countdown { set; get; }

    private char[] rounds;

    public Dictionary<KeyValuePair<bool, int>, NetworkObject> playerObjects = new Dictionary<KeyValuePair<bool, int>, NetworkObject>();

    [HideInInspector] public bool isHost = false;

    private Transform[] attackSideSpawnPoints = new Transform[3];
    private Transform[] defenceSideSpawnPoints = new Transform[3];
    private GameObject[] barriers;
    private BasicSpawner basicSpawner;

    public int currentRound = 0;

    bool playing = false;

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

        GameObject barriersObj = GameObject.Find("Barriers");
        barriers = new GameObject[barriersObj.transform.childCount];
        for(int i = 0; i < barriers.Length; i++)
        {
            barriers[i] = barriersObj.transform.GetChild(i).gameObject;
        }

        rounds = new char[] { 'x', 'x', 'x', 'x', 'x' };

        if(isHost)
            RPC_ResetRounds(rounds);
    }

    public void StartRound()
    {
        RPC_StartRound();
        callSpawnPlayers();

        if (isHost)
        {
            StartCoroutine(roundCountDown());
        }
    }

    IEnumerator roundCountDown()
    {
        Countdown = roundCountDownTime;

        playing = false;
        RPC_SetBarriers(true);

        while(Countdown > 0)
        {
            yield return new WaitForSeconds(1);
            Countdown--;
        }

        playing = true;
        RPC_SetBarriers(false);
    }

    bool winStateCheck(bool attacking)
    {
        foreach(KeyValuePair<KeyValuePair<bool, int>, NetworkObject> playerDictKeyValuePair in playerObjects)
        {
            if (playerDictKeyValuePair.Value != null) { 
                if(playerDictKeyValuePair.Key.Key != attacking)
                {
                    return false;
                } else
                {
                    print(playerDictKeyValuePair.Value);
                }
            } else
            {
                print("is null");
            }
        }

        return true;
    }

    private void Update()
    {
        if (countDownPanel.activeSelf)
        {
            countDownText.text = Countdown.ToString();
        }

        string roundTextMsg = "";

        for(int i = 0; i < rounds.Length; i++)
        {
            switch(rounds[i])
            {
                case 'x':
                    roundTextMsg += "X";
                    break;
                case 'a':
                    roundTextMsg += basicSpawner.characterSelectionScreen.isAttackingSide ? "W" : "L";
                    break;
                case 'd':
                    roundTextMsg += basicSpawner.characterSelectionScreen.isAttackingSide ? "L" : "W";
                    break;
            }

            if (rounds.Length - 1 != i)
                roundTextMsg += " ~ ";
        }

        if (isHost && playing)
        {
            if (winStateCheck(true))
            {
                EndRound('a');
            }


            if (winStateCheck(false))
            {
                EndRound('d');
            }
        }

        roundText.text = roundTextMsg;
    }

    public void callSpawnPlayers()
    {
        RPC_SpawnPlayer();
    }

    [Rpc]
    public void RPC_SetBarriers(bool value)
    {
        countDownPanel.SetActive(value);

        foreach(GameObject barrier in barriers)
        {
            barrier.SetActive(value);
        }
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
        player.GetComponent<PlayerMovement>().callChangeSprite(basicSpawner.characterSelectionScreen.characterID);
        player.GetComponent<Gun>().attacking = basicSpawner.characterSelectionScreen.isAttackingSide;
        player.GetComponent<PlayerLife>().diedCanvas = basicSpawner.diedCanvas;
        RPC_AddPlayerDict(isAttacking, sideId, player.Id);
    }

    [Rpc]
    public void RPC_AddPlayerDict(bool attacking, int id, NetworkId netId) 
    {
        KeyValuePair<bool, int> key = new KeyValuePair<bool, int>(attacking, id);
        if (playerObjects.ContainsKey(key)) {
            playerObjects[key] = Runner.FindObject(netId);
        } else {
            playerObjects.Add(key, Runner.FindObject(netId));
        }
        print("Played added: " + playerObjects.Count.ToString());
    }

    [Rpc]
    public void RPC_ResetRounds(char[] newRounds)
    {
        rounds = newRounds;
    }

    IEnumerator startNextRound()
    {
        yield return new WaitForSeconds(3);
        StartRound();
    }

    void EndRound(char thisRoundWin)
    {
        if (!playing)
            return;

        playing = false;
        print("Round ended!");
        rounds[currentRound] = thisRoundWin;
        RPC_ResetRounds(rounds);
        currentRound++;

        foreach (KeyValuePair<KeyValuePair<bool, int>, NetworkObject> playerDictKeyValuePair in playerObjects)
        {
            if (playerDictKeyValuePair.Value != null)
            {
                playerDictKeyValuePair.Value.GetComponent<PlayerLife>().callDieNoCanvas();
                print("Killed 1");
            }
        }

        StartCoroutine(startNextRound());

        if (currentRound == 5)
        {
            Runner.Shutdown();
            bool attackWon = false;

            int attackI = 0;
            int defenceI = 0;
            foreach (char character in rounds)
            {
                switch (character)
                {
                    case 'a':
                        attackI++;
                        break;
                    case 'd':
                        defenceI++;
                        break;
                }
            }
            attackWon = attackI > defenceI;

            if (attackWon && basicSpawner.characterSelectionScreen.isAttackingSide)
                PlayerPrefs.SetString("WonLost", "won");
            else
                PlayerPrefs.SetString("WonLost", "lost");

            PlayerPrefs.GetString("WonLost");
            SceneManager.LoadScene("WinLoss");
        }
    }
}
