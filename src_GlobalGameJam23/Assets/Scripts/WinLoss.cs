using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinLoss : MonoBehaviour
{
    public TMP_Text winLossText;

    void Start()
    {
        winLossText.text = "You " + PlayerPrefs.GetString("WonLost");
    }

    IEnumerator waitTime()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu");
    }
}
