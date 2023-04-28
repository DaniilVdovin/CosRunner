using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.VisualScripting;

public class ExtraItem
{
    public int id;
    private Sprite _Icon;
    public Sprite Icon
    {
        get => Icon; set
        {
            Icon = value;
            if(_template != null && Icon != null)
            {
                _template.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(Icon);
            }
        }
    }
    private float _Duration;
    public float Duration
    {
        get => _Duration; set
        {
            _Duration = value;
        }
    }
    private TemplateContainer _template;
    public TemplateContainer template
    {
        get => _template; set
        {
            _template = value;
            _template.RegisterCallback<ClickEvent>(Click);
        }
    }
    private TweenerCore<float, float, FloatOptions> _tweener { get; set; }
    public EventHandler<ExtraItem> EventClose;
    private void Click(ClickEvent e)
    {
        Close();
    }
    public void Close()
    {
        if ( EventClose != null) EventClose.Invoke(this,this);
        if (_tweener.IsActive()) _tweener.Kill();
        _template.parent.Remove(_template);
    }
    public void Start(TemplateContainer t = null, float? d = null)
    {
        if (_template != null)
        {
            if (_tweener.IsActive()) _tweener.Kill();
            _tweener = DOTween.To(() => (t??_template).Q<VisualElement>("Holder").localBound.width,
                x => (t ?? _template).Q<VisualElement>("Bar").style.width = x, 0f, d??Duration);
            _tweener.SetEase(Ease.Linear);
            _tweener.OnComplete(() => Close());
            _tweener.Play();
            
            DOTween.To(() => d??Duration,
                x => (t ?? _template).Q<Label>("Timer").text = x.ToString("f2"), 0f, d ?? Duration);
            _tweener.SetEase(Ease.Linear);
        }
    }
}

public class GameUI : MonoBehaviour
{
    private VisualElement UI, DieFrame, _LoaderUI, StatsFrame, _OxygenUI, ExtraItemsHolder;
    private GroupBox LittleSettings;
    private Button Setting, Replay, Resurect, Rating;
    private Label Score, Coins;

    private PlayerControl PC;
    private AdsConroller AdsConroller;

    public VisualTreeAsset ExtraTemplate;

    public List<ExtraItem> extraItems;

    private float temp_Score;
    private int temp_Coins;

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
        ExtraItemsHolder = UI.Q<VisualElement>("ExtraItemsHolder");
        extraItems = new();
        _LoaderUI = UI.Q<VisualElement>("_LoaderUI");
        _OxygenUI = UI.Q<VisualElement>("_OxygenBarUI");

        Replay.RegisterCallback<ClickEvent>(CallbackReplay);
        Resurect.RegisterCallback<ClickEvent>(CallbackResurect);
        Rating.RegisterCallback<ClickEvent>(CallbackReplay);

        Setting.RegisterCallback<ClickEvent>((e) => {
            LittleSettings.visible = !LittleSettings.visible;
        });
    }
    public ExtraItem AddExtraItem(int id, Sprite sprite, float Duration, EventHandler<ExtraItem> ActionClose)
    {
        ExtraItem item;
        if (extraItems.Exists((i) => i.id == id))
        {
            item = extraItems.Find((i) => i.id == id);
            item.Start();
        }
        else
        {
            item = new();
            item.id = id;
            TemplateContainer temp = ExtraTemplate.Instantiate();
            ExtraItemsHolder.Add(temp);
            item.template = temp;
            item.Duration = Duration;
            item.EventClose += ActionClose;
            item.EventClose += (s, i) => extraItems.Remove(i);
            extraItems.Add(item);
            item.Start(temp,Duration);
        }
        return item;
    }
    public void StartGame()
    {
        UI.visible = true;
        AnimateLoading(new[] { "Готовим печеньки", "Генерируем карту", "Артем когда релиз ?", "Э! A когда играть ?", "Погнали !" },
             42 * .1f);
    }
    /*---------------------------DIE FRAME----------------------------*/
    public void Die()
    {
        StartCoroutine(ShowDieFrame());
    }
    private void CallbackReplay(ClickEvent e) {
        SaveStats();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
    private void CallbackResurect(ClickEvent e)
    {
        AnimateLoading(new[] {"Начали грузить","Только для вас"},
             5f);
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
        PC.PreRessurect();
        for (int i = 4; i >= 1; i--)
        {
            Debug.Log("RESURECT: " + i);
            yield return new WaitForSeconds(1f);
        }
        PC.isRun = true;
    }
    private IEnumerator ShowDieFrame()
    {
        yield return new WaitForSeconds(3f);
        DieFrame.visible = true;
        Time.timeScale = 0;
    }
    /*-------------------------END DIE FRAME-------------------------*/
    public void SetCoinsAndScore(PlayerControl pc,int Coin, float Score)
    {
        if (this.enabled)
        {
            PC = pc;
            temp_Score = Score;
            temp_Coins = Coin;
            SetCoins(Coin);
            SetScore(Score);
        }
    }
    public void SetScore(float value) => Score.text = "Score: " + value.ToString("f2");
    public void SetCoins(int value) => Coins.text = "Coins: " + value.ToString();
    public void SetOxygen(float value)
        => _OxygenUI.Q<VisualElement>("Ox_Bar").style.height = (float)(((float)_OxygenUI.localBound.height/100f)*value);
    private void SaveStats() { SaveBestScore(); SaveCoins(); }
    private void SaveBestScore()
    {
        PlayerGeneralData.Score = temp_Score;
    }
    private void SaveCoins()
    {
        PlayerGeneralData.Coins = temp_Coins;
    }
    public void AnimateLoading(String[] txt,float Duration)
    {
        VisualElement p = _LoaderUI[1];
        Label t = _LoaderUI[2] as Label;
        DOTween.To(() => 0, x =>
        t.text = txt[x], txt.Length-1,Duration)
            .SetEase(Ease.Linear);
        DOTween.To(() => 0, x =>
        p.style.width = x, _LoaderUI.worldBound.width, Duration)
            .SetEase(Ease.Linear)
            .OnComplete(()=>_LoaderUI.visible = false);
    }
}
