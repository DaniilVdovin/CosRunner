using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameCotroller : MonoBehaviour
{
    public PlayerControl PlayerControl;
    public Generate Generate;

    public VisualElement Menu;

    public ShopUI Shop;

    private void Awake()
    {
        if(!SceneManager.GetSceneByName("NDY").isLoaded)
            SceneManager.LoadSceneAsync("NDY", LoadSceneMode.Additive);
    }
    private void Start()
    {
        Menu = GetComponent<UIDocument>().rootVisualElement.Q<TemplateContainer>("MainUI");
        Shop = GetComponent<ShopUI>();

        PlayerControl = FindAnyObjectByType<PlayerControl>();
        Generate = FindAnyObjectByType<Generate>();

        Menu.Q<Button>("Start").RegisterCallback<ClickEvent>(StartGame);
        Menu.Q<Button>("Shop").RegisterCallback<ClickEvent>(StartShop);
    }

    private void StartShop(ClickEvent e)
    {
        Menu.visible = false;
        Shop.StartShop();
    }
    private void StartGame(ClickEvent e = null)
    {
        Menu.visible = false;
        PlayerControl.StartGame();
        Generate.StartGenerate();
    }
    private void OnDestroy()
    {
        Menu.Q<Button>("Start").UnregisterCallback<ClickEvent>(StartGame);
        Menu.Q<Button>("Shop").UnregisterCallback<ClickEvent>(StartShop);
    }
}
