using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.GoogleServices;
using GooglePlayGames;
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
        Application.targetFrameRate = 60;
        GoogleClientConfig.Auth();
        //???????????????
        //PlayerGeneralData.Init();
    }
    private void Start()
    {
        Menu = GetComponent<UIDocument>().rootVisualElement.Q<TemplateContainer>("MainUI");
        // check
        Shop = GetComponent<ShopUI>();
        LeaderBoard = GetComponent<LeaderBoardUI>();

        PlayerControl = FindAnyObjectByType<PlayerControl>();
        
        Generate = FindAnyObjectByType<Generate>();

        Menu.Q<Button>("Start").RegisterCallback<ClickEvent>(StartGame);
        Menu.Q<Button>("Shop").RegisterCallback<ClickEvent>(StartShop);
        Menu.Q<Button>("Rating").RegisterCallback<ClickEvent>(StartLeaderBoard);

    }

    private void StartShop(ClickEvent e)
    {
        Menu.visible = false;
        //Check
        if(Shop != null)
        {
            Debug.Log(Shop);
            Debug.Log(Shop.Coin);
            Debug.Log("Shop started");
            Shop.StartShop();
        }
            
        else
            Debug.Log("Shop not started");
    }
    private void StartLeaderBoard(ClickEvent e)
    {
        Menu.visible = false;
        //LeaderBoard.StartLeaderBoard();
        PlayGamesPlatform.Instance.ShowLeaderboardUI(LeaderBoadConf.LeaderboardId);
    }
    private void StartGame(ClickEvent e = null)
    {
        Menu.visible = false;
        PlayerControl.StartGame();
        Generate.StartGenerate();
    }
}
