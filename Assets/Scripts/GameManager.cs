using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = false;
    public Button startButton;

    private void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    public void StartGame()
    {
        isGameActive = true;
        startButton.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
    }
}
