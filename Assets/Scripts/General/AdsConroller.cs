using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mycom.Target.Unity.Ads;
using Mycom.Target.Unity.Common;

public class AdsConroller : MonoBehaviour
{
    private InterstitialAd _interstitialAd;
    private Action<bool,string> result;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
       
    }
    private void Start()
    {
        MyTargetManager.InitSdk();
        MyTargetManager.DebugMode = true;
        MyTargetManager.Config = new MyTargetConfig.Builder().WithTestDevices("b51e3a7a-b7c2-4563-999b-10ca1ad1abe1",
          "9c6130f4-28dc-4623-bb75-78182d6d508c").Build();
        PlayerGeneralData.Init();
        InitAd();
    }

    private void InitAd()
    {
        // Создаем экземпляр InterstitialAd
        _interstitialAd = CreateInterstitialAd();
        // Устанавливаем обработчики событий
        
        _interstitialAd.AdLoadCompleted += OnLoadCompleted;
        _interstitialAd.AdDisplayed += OnAdDisplayed;
        _interstitialAd.AdDismissed += OnAdDismissed;
        _interstitialAd.AdVideoCompleted += Ad_AdRewarded;
        _interstitialAd.AdClicked += OnAdClicked;
        _interstitialAd.AdLoadFailed += OnAdLoadFailed;


        _interstitialAd.Load();

    }

    public void SkipByAD(Action<bool, string> result) {
        // Запускаем загрузку данных
        InitAd();


        Debug.Log("SkipByAD Load");
        this.result = result;
        if (_interstitialAd != null)
        {
            Debug.Log("Try Show");
            _interstitialAd.Show();
        }
           
        else
        {
            Debug.Log("null");
        }
    }
    private void OnLoadCompleted(object sender, EventArgs e)
    {
        Debug.Log("OnLoadCompleted");
    }
    private void OnAdDisplayed(object sender, EventArgs e)
    {
        Debug.Log("OnAdDisplayed");
    }
    private void OnAdDismissed(object sender, EventArgs e)
    {

        Debug.Log("OnAdDismissed");
    }
    private void Ad_AdRewarded(object sender, EventArgs e)
    {

        Debug.Log("Ad_AdRewarded");
    }
    private void OnAdClicked(object sender, EventArgs e)
    {
        Debug.Log("OnAdClicked");
    }
    private void OnAdLoadFailed(object sender, ErrorEventArgs e)
    {

        Debug.Log("OnAdLoadFailed");
    }
    private InterstitialAd CreateInterstitialAd()
    {
        UInt32 slotId = 38837;
#if UNITY_ANDROID
        slotId = 38837;
#elif UNITY_IOS
        slotId = 38838;
#endif

        // Создаем экземпляр InterstitialAd
        return new InterstitialAd(slotId);
    }
    private void OnDestroy()
    {
        PlayerGeneralData.Clear();
    }
}
