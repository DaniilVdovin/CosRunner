using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;

public static class PlayerGeneralData
{
    private static int _Coins = 0 ;
    private static float _Score = 0;
    private static int _id_Prefs = 0;

    public static int Coins
    {
        get => _Coins; set
        {
            _Coins += value;

            PlayerPrefs.SetInt("Coins", _Coins);
            Debug.Log("New Coins: " + _Coins);
            StatsUpdate.Invoke(null, EventArgs.Empty);
        }
    }
    public static float Score
    {
        get => _Score; set
        {
            if (_Score < value)
            {
                _Score = value;
                PlayerPrefs.SetFloat("BestScore", _Score);
                Debug.Log("New bestScore: " + _Score);
                LeaderBoadConf.AddScore(_Score);
                StatsUpdate.Invoke(null, EventArgs.Empty);
            }
        }
    }
    public static int id_Prefs
    {
        get => _id_Prefs; set
        {
            if (_id_Prefs != value)
            {
                _id_Prefs = value;
                PlayerPrefs.SetInt("id_Prefs", _id_Prefs);
                Debug.Log("New id_Prefs: " + _id_Prefs);
                StatsUpdate.Invoke(null, EventArgs.Empty);
            }
        }
    }

    public static EventHandler StatsUpdate;
    public static void Init()
    {
        UnityServices.InitializeAsync();
        Debug.LogError(UnityServices.ExternalUserId);
        StatsUpdate += (s,e) => PlayerPrefs.Save();
        Clear();
        LoadData();
        Login();
    }
    public static void Login()
    {
        var id = PlayerPrefs.GetString("ID", "_");
        AuthenticationService.Instance.SignedIn += () =>
        {
            PlayerPrefs.SetString("ID", AuthenticationService.Instance.PlayerId);
            PlayerPrefs.Save();
            Debug.LogError(AuthenticationService.Instance.PlayerId);
        };
        SignInOptions options = new();
        options.CreateAccount = id == "_";
        Debug.LogError("ID:" + id);
        AuthenticationService.Instance.SignInAnonymouslyAsync(options);

    }
    public static void SaveData()
    {
        var data = new Dictionary<string, object> {
            { "Coins", _Coins }
        };
        CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }
    public static void LoadData() {
        _Coins = PlayerPrefs.GetInt("Coins", 0);
        _id_Prefs = PlayerPrefs.GetInt("id_Prefs", 0);
        _Score = PlayerPrefs.GetFloat("BestScore", 0);

        Debug.Log($"Stats:\nCoins:\t{_Coins}\nBestScore:\t{_Score}\nid_Prefs:\t{_id_Prefs}");
    }
    //DANGER
    public static void Clear()
    {
        _Coins = 0;
        _id_Prefs = 0;
        _Score = 0;
        PlayerPrefs.DeleteAll();
        StatsUpdate.Invoke(null, EventArgs.Empty);
    }
}
