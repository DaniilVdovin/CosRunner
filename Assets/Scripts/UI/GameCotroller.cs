using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class GameCotroller : MonoBehaviour
{
    public PlayerControl PlayerControl;
    public Generate Generate;

    public VisualElement Menu;

    public ShopUI Shop;
    public LeaderBoardUI LeaderBoard;

    private void Awake()
    {
        if (!GameObject.Find("ADS"))
            SceneManager.LoadSceneAsync("NDY", LoadSceneMode.Additive);
    }
    private void Start()
    {
       
        Menu = GetComponent<UIDocument>().rootVisualElement.Q<TemplateContainer>("MainUI");
        Shop = GetComponent<ShopUI>();
        LeaderBoard = GetComponent<LeaderBoardUI>();

        PlayerControl = FindAnyObjectByType<PlayerControl>();
        Generate = FindAnyObjectByType<Generate>();

        PlayerGeneralData.Init();

        Menu.Q<Button>("Start").RegisterCallback<ClickEvent>(StartGame);
        Menu.Q<Button>("Shop").RegisterCallback<ClickEvent>(StartShop);
        Menu.Q<Button>("Rating").RegisterCallback<ClickEvent>(StartLeaderBoard);
    }

    private void StartShop(ClickEvent e)
    {
        Menu.visible = false;
        Shop.StartShop();
    }
    private void StartLeaderBoard(ClickEvent e)
    {
        Menu.visible = false;
        LeaderBoard.StartLeaderBoard();
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
