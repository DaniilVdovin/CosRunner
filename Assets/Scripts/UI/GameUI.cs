using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    private VisualElement UI, DieFrame, _LoaderUI,StatsFrame;
    private GroupBox LittleSettings;
    private Button Setting, Replay,Resurect,Rating;
    private Label Score, Coins;

    private PlayerControl PlayerControl;
    private AdsConroller AdsConroller;

    private void Start()
    {
        AdsConroller = GameObject.Find("ADS").GetComponent<AdsConroller>();

        UI = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("GameUI");
        Setting = UI.Q<Button>("Settings");
        LittleSettings = UI.Q<GroupBox>("LittleSettings");
        DieFrame = UI.Q<VisualElement>("DieFrame");
        Replay = UI.Q<Button>("Replay");
        Resurect = UI.Q<Button>("Resurect");
        Rating = UI.Q<Button>("Rating");
        StatsFrame = UI.Q<VisualElement>("Stats");
        Score = UI.Q<Label>("Score");
        Coins = UI.Q<Label>("Coins");

        _LoaderUI = UI.Q<VisualElement>("_LoaderUI");

        Replay.RegisterCallback<ClickEvent>(CallbackReplay);
        Resurect.RegisterCallback<ClickEvent>(CallbackResurect);
        Rating.RegisterCallback<ClickEvent>(CallbackReplay);

        Setting.RegisterCallback<ClickEvent>((e) => {
            LittleSettings.visible = !LittleSettings.visible;
        });
    }
    public void ConnectPlayer(PlayerControl playerControl)
    {
        PlayerControl = playerControl;
    }
    public void StartGame()
    {
        UI.visible = true;
        AnimateLoading();
    }
    /*---------------------------DIE FRAME----------------------------*/
    public void Die()
    {
        StartCoroutine(ShowDieFrame());
    }
    private void CallbackReplay(ClickEvent e) {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
        GameCotroller.StartGame();
    }
    private void CallbackResurect(ClickEvent e)
    {
        AdsConroller.SkipByAD((T, M) =>
        {
            if (T)
            {
                Time.timeScale = 1;
                DieFrame.visible = false;
                StartCoroutine(ResurestAction());
                Debug.Log(M);
            }
            else
            {
                CallbackReplay(new ClickEvent());
                Debug.LogError(M);
            }
        });
    }
    private IEnumerator ResurestAction() {
        PlayerControl.PreRessurect();
        for (int i = 4; i >= 1; i--)
        {
            Debug.Log("RESURECT: " + i);
            yield return new WaitForSeconds(1f);
        }
        PlayerControl.isRun = true;
    }
    private IEnumerator ShowDieFrame()
    {
        yield return new WaitForSeconds(3f);
        DieFrame.visible = true;
        Time.timeScale = 0;
    }
    /*-------------------------END DIE FRAME-------------------------*/
    public void SetCoinsAndScore(int Coin, float Score)
    {
        if (this.enabled)
        {
            SetCoins(Coin);
            SetScore(Score);
        }
    }
    public void SetScore(float value) => Score.text = "Score: " + value.ToString("f2");
    public void SetCoins(int value) => Coins.text = "Coins: " + value.ToString();

    public void AnimateLoading()
    {
        VisualElement p = _LoaderUI[1];
        Label t = _LoaderUI[2] as Label;
        var txt = new[] {"Готовим печеньки","Генерируем карту","Э А когда играть ?","Погнали !"};
        DOTween.To(() => 0, x =>
        t.text = txt[x], txt.Length-1, 42 * .2f)
            .SetEase(Ease.Linear);
        DOTween.To(() => 0, x =>
        p.style.width = x, _LoaderUI.worldBound.width, 42 * .2f)
            .SetEase(Ease.Linear)
            .OnComplete(()=>_LoaderUI.visible = false);
    }
}
