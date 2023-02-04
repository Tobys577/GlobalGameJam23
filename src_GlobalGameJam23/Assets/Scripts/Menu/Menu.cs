using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputField;

    private void Start()
    {
        usernameInputField.text = PlayerPrefs.GetString("Player");
    }

    public void LoadMainLevel(string levelName)
    {
        PlayerPrefs.SetString("Player", usernameInputField.text);
        SceneManager.LoadScene(levelName);
    }
}
