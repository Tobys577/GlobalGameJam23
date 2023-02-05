using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public struct CharacterSelectionIcon
{
    public Image icon;
    public TMP_Text name;
    public TMP_Text characterName;
}

public class CharacterSelectionScreen : NetworkBehaviour
{
    public TMP_Text timerCounter;

    public bool isAttackingSide;
    public int iconNumber;

    public int characterID = -1;

    [SerializeField]
    private Character[] characters;

    [SerializeField]
    private Button lockInButton;

    [SerializeField]
    private CharacterSelectionIcon[] attackIcons;

    [SerializeField]
    private CharacterSelectionIcon[] defenceIcons;

    [SerializeField]
    private Button[] characterButtons;

    [HideInInspector]
    public int[] lockedInAttacking = new int[] { -1, -1, -1 };
    [HideInInspector]
    public int[] lockedInDefending = new int[] { -1, -1, -1 };

    private bool lockedIn = false;

    private BasicSpawner basicSpawner;

    public void Start()
    {
        basicSpawner = GameObject.Find("BasicSpawner").GetComponent<BasicSpawner>();
    }

    public void callUpdateTimer(int time)
    {
        RPC_UpdateTimer(time);
    }

    public void callSelectCharacter()
    {
        RPC_SelectCharacter(characterID, isAttackingSide, iconNumber, BasicSpawner.username, lockedIn);
    }

    public void setSelectedCharacter(int selectedCharacter)
    {
        characterID = selectedCharacter;
    }

    public void skip()
    {
        basicSpawner.timer = 0;
    }

    public void lockInCharacter()
    {
        if (characterID != -1)
        {
            lockedIn = true;
        }
    }

    private void Update()
    {
        callSelectCharacter();

        lockInButton.interactable = !(characterID == -1);

        if (isAttackingSide)
        {
            if (!lockedIn)
            {
                for (int j = 0; j < lockedInAttacking.Length; j++)
                {
                    if (lockedInAttacking[j] == characterID)
                    {
                        characterID = -1;
                    }
                }
            }

            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].interactable = !lockedIn;
                for (int j = 0; j < lockedInAttacking.Length; j++)
                {
                    if(i == lockedInAttacking[j])
                    {
                        characterButtons[i].interactable = false;
                        break;
                    }
                }
            }
        } else
        {
            if (!lockedIn)
            {
                for (int j = 0; j < lockedInDefending.Length; j++)
                {
                    if (lockedInDefending[j] == characterID)
                    {
                        characterID = -1;
                    }
                }
            }

            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].interactable = !lockedIn;
                for (int j = 0; j < lockedInDefending.Length; j++)
                {
                    if (i == lockedInDefending[j])
                    {
                        characterButtons[i].interactable = false;
                        break;
                    }
                }
            }
        }
    }

    public void callSpawnPlayers()
    {
        RPC_SpawnPlayer();
    }

    [Rpc]
    public void RPC_SpawnPlayer()
    {
        NetworkObject player = basicSpawner.SpawnPlayer();
        player.GetComponent<PlayerMovement>().bodySprite.sprite = characters[characterID].characterSprite;
        player.GetComponent<Gun>().attacking = isAttackingSide;
        gameObject.SetActive(false);
    }

    [Rpc]
    public void RPC_UpdateTimer(int time, RpcInfo info = default)
    {
        timerCounter.text = time.ToString();
    }

    [Rpc]
    public void RPC_SelectCharacter(int characterId, bool x_attackingSide, int x_iconNum, string userName, bool x_lockedIn = false)
    {
        if (characterId == -1)
        {
            if (x_attackingSide)
            {
                attackIcons[x_iconNum].name.text = userName;
                attackIcons[x_iconNum].characterName.text = "";
                attackIcons[x_iconNum].icon.sprite = null;
            }
            else
            {
                defenceIcons[x_iconNum].name.text = userName;
                defenceIcons[x_iconNum].characterName.text = "";
                defenceIcons[x_iconNum].icon.sprite = null;
            }
        }
        else
        {
            if (x_attackingSide)
            {
                attackIcons[x_iconNum].name.text = userName;
                attackIcons[x_iconNum].characterName.text = characters[characterId].characterName;
                attackIcons[x_iconNum].icon.sprite = characters[characterId].icon;
            }
            else
            {
                defenceIcons[x_iconNum].name.text = userName;
                defenceIcons[x_iconNum].characterName.text = characters[characterId].characterName;
                defenceIcons[x_iconNum].icon.sprite = characters[characterId].icon;
            }

            if (x_lockedIn)
            {
                if (x_attackingSide)
                {
                    lockedInAttacking[x_iconNum] = characterId;
                } else
                {
                    lockedInDefending[x_iconNum] = characterId;
                }
            }
        }
    }
}
