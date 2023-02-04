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

    [SerializeField]
    private Character[] characters;

    [SerializeField]
    private CharacterSelectionIcon[] attackIcons;

    [SerializeField]
    private CharacterSelectionIcon[] defenceIcons;

    public void callUpdateTimer(int time)
    {
        RPC_UpdateTimer(time);
    }

    [Rpc]
    public void RPC_UpdateTimer(int time, RpcInfo info = default)
    {
        print("Update timer called! " + time.ToString());
        timerCounter.text = time.ToString();
    }

    [Rpc]
    public void RPC_SelectCharacter(int characterId)
    {

    }
}
