using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

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
    private Button lockInButton;

    [SerializeField]
    private Character[] characters;

    [SerializeField]
    private CharacterSelectionIcon[] attackIcons;

    [SerializeField]
    private CharacterSelectionIcon[] defenceIcons;

    private BasicSpawner basicSpawner;

    public void Start()
    {
        basicSpawner = GameObject.Find("BasicSpawner").GetComponent<BasicSpawner>();
        lockInButton.onClick.AddListener(basicSpawner.SpawnPlayer);   
    }

    public void callUpdateTimer(int time)
    {
        RPC_UpdateTimer(time);
    }

    public void callSelectCharacter()
    {
        RPC_SelectCharacter(characterID, isAttackingSide, iconNumber, BasicSpawner.username);
    }

    private void Update()
    {
        callSelectCharacter();
    }

    [Rpc]
    public void RPC_UpdateTimer(int time, RpcInfo info = default)
    {
        print("Update timer called! " + time.ToString());
        timerCounter.text = time.ToString();
    }

    [Rpc]
    public void RPC_SelectCharacter(int characterId, bool x_attackingSide, int x_iconNum, string userName)
    {
        if(characterId == -1)
        {
            if (x_attackingSide)
            {
                attackIcons[x_iconNum].name.text = userName;
                attackIcons[x_iconNum].characterName.text = "";
                attackIcons[x_iconNum].icon.sprite = null;
            } else
            {
                defenceIcons[x_iconNum].name.text = userName;
                defenceIcons[x_iconNum].characterName.text = "";
                defenceIcons[x_iconNum].icon.sprite = null;
            }
        } else
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
        }
    }
}
