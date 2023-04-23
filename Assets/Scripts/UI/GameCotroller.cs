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
    private static VisualElement Menu;


    private void Awake()
    {
        if(!SceneManager.GetSceneByName("NDY").isLoaded)
            SceneManager.LoadSceneAsync("NDY", LoadSceneMode.Additive);
    }
    private void Start()
    {
        Menu = GetComponent<UIDocument>().rootVisualElement.Q<TemplateContainer>("MainUI");
        PlayerControl = FindAnyObjectByType<PlayerControl>();
        Generate = FindAnyObjectByType<Generate>();

        Menu.Q<Button>("Start").RegisterCallback<ClickEvent>(StartGame);
    }
    public static void StartGame(ClickEvent e = null)
    {
        Menu.visible = false;
        PlayerControl.StartGame();
        Generate.StartGenerate();
    }
    private void OnDestroy()
    {
        Menu.Q<Button>("Start").UnregisterCallback<ClickEvent>(StartGame);
    }
}
