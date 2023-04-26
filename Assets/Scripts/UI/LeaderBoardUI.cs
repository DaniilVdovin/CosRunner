using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
/// <summary>
/// leader board dataclass save id,score,name,result,type
/// </summary>
public class LeaderBoardItem
{
    public int id;
    public string Name;
    public float Score;
    public string Result { get=>Name+": "+Score.ToString("f2");}
    public int type { get => id < 1 ? 0 : id < 3 ? 1 : id < 7 ? 2 : id < 11 ? 3 : 4; }
}
/// <summary>
/// controller class
/// </summary>
public class LeaderBoardUI : MonoBehaviour
{
    private VisualElement UI, Holder;
    public List<LeaderBoardItem> items;
    public VisualTreeAsset Def_Item;
    public Label Score;
    public Button Close;
    public GameCotroller Menu;
    public Sprite[] Spites;
    /*
     * Diamond - 1 
     * Gold - 2
     * Silver - 4
     * Brons - 4
     * Def - 89
     */

    // Start is called before the first frame update

    void Start()
    {
        items = new();
        Menu = GetComponent<GameCotroller>();
        UI = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("LeaderBoardUI");
        Score = UI.Q<Label>("Score");
        Holder = UI.Q<VisualElement>("LeaderBoardContainer");
        Close = UI.Q<Button>("LeaderBoardClose");
        Close.RegisterCallback<ClickEvent>(LBUIClose);
        for (int i = 0; i < 100; i++)
        {
            items.Add(new LeaderBoardItem()
            {
                id = i,
                Name = "Name " + i,
                Score = 1234 * i
            });
        }
        PlayerGeneralData.StatsUpdate += UPD;
    }
    /// <summary>
    /// sets data to Score object its event
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    private void UPD(object s,EventArgs e) {
        Score.text = "Score: " + PlayerGeneralData.Score.ToString("f2");
    }
    public void StartLeaderBoard() {
        UI.visible = true;
        UPD(null, null);
        StartCoroutine(Generate());
    }
    /// <summary>
    /// generate leaderboard with yield return WaitForseconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator Generate()
    {
        Holder.Clear();
        foreach (var item in items)
        {
            TemplateContainer temp = Def_Item.Instantiate();
            temp.style.opacity = 0;
            temp.Q<Label>("TextH").text = item.Result;
            temp.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(Spites[item.type]);
            Holder.Add(temp);
            DOTween.To(() => 0f, x => temp.style.opacity = x
                    , 1f, .5f)
            .SetEase(Ease.Linear);
            yield return new WaitForSeconds(.1f);
        }
    }
    private void LBUIClose(ClickEvent evt)
    {
        UI.visible = false;
        Menu.Menu.visible = true;
    }
    private void OnDestroy()
    {
        Close.UnregisterCallback<ClickEvent>(LBUIClose);
    }
}
