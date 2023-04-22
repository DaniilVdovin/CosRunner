using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameCotroller : MonoBehaviour
{
    public static PlayerControl PlayerControl;
    public static Generate Generate;
    private static UIDocument Menu;


    private void Awake()
    {
        if(!SceneManager.GetSceneByName("NDY").isLoaded)
            SceneManager.LoadSceneAsync("NDY", LoadSceneMode.Additive);
    }
    private void Start()
    {
        Menu = GetComponent<UIDocument>();
        PlayerControl = FindAnyObjectByType<PlayerControl>();
        Generate = FindAnyObjectByType<Generate>();

        Menu.rootVisualElement.Q<Button>("Start").RegisterCallback<ClickEvent>(StartGame);
    }
    public static void StartGame(ClickEvent e = null)
    {
        Menu.enabled = false;
        PlayerControl.StartGame();
        Generate.StartGenerate();
    }
    private void OnDestroy()
    {
        Menu.rootVisualElement.Q<Button>("Start").UnregisterCallback<ClickEvent>(StartGame);
    }
}
