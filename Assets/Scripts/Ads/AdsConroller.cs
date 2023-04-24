using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mycom.Target.Unity.Ads;
using Mycom.Target.Unity.Common;

public class AdsConroller : MonoBehaviour
{
    private RewardedAd _interstitialAd;
    private Action<bool,string> result;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        MyTargetManager.DebugMode = true;
        MyTargetManager.Config = new MyTargetConfig.Builder().WithTestDevices("b51e3a7a-b7c2-4563-999b-10ca1ad1abel",
           "9c6130f4-28dc-4623-bb75-78182d6d508c").Build();

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
        _interstitialAd.AdRewarded += Ad_AdRewarded;
        _interstitialAd.AdClicked += OnAdClicked;
        _interstitialAd.AdLoadFailed += OnAdLoadFailed;
    }

    public void SkipByAD(Action<bool, string> result) {
        // Запускаем загрузку данных
        Debug.Log("SkipByAD Load");
        _interstitialAd?.Load();
        this.result = result;
    }
    private void OnLoadCompleted(object sender, EventArgs e)
    {
        _interstitialAd?.Show();
        Debug.Log("OnLoadCompleted");
    }
    private void OnAdDisplayed(object sender, EventArgs e)
    {
        Debug.Log("OnAdDisplayed");
    }
    private void OnAdDismissed(object sender, EventArgs e)
    {
        result.Invoke(false, "Ad Dismissed");
        Debug.Log("OnAdDismissed");
    }
    private void Ad_AdRewarded(object sender, RewardEventArgs e)
    {
        result.Invoke(true, "Ad_AdRewarded");
        Debug.Log("Ad_AdRewarded");
    }
    private void OnAdClicked(object sender, EventArgs e)
    {
        Debug.Log("OnAdClicked");
    }
    private void OnAdLoadFailed(object sender, ErrorEventArgs e)
    {
        result.Invoke(false, "OnAdLoadFailed: " + e.Message);
        Debug.Log("OnAdLoadFailed");
    }
    private RewardedAd CreateInterstitialAd()
    {
        UInt32 slotId = 0;
#if UNITY_ANDROID
        slotId = 38837;
#elif UNITY_IOS
        slotId = 38838;
#endif
        Debug.Log("CreateInterstitialAd");
        // Создаем экземпляр InterstitialAd
        return new RewardedAd(slotId);
    }
}
