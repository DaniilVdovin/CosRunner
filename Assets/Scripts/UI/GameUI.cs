using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private UIDocument UI;
    private Button Setting;
    private GroupBox LittleSettings;

    private VisualElement DieFrame;
    private Button Replay,Resurect,Rating;


    private Label Score;
    private Label Coins;

    private PlayerControl PlayerControl;
    private void Start()
    {
        UI = GetComponent<UIDocument>();
        Setting = UI.rootVisualElement.Q<Button>("Settings");
        LittleSettings = UI.rootVisualElement.Q<GroupBox>("LittleSettings");
        DieFrame = UI.rootVisualElement.Q<VisualElement>("DieFrame");
        Replay = UI.rootVisualElement.Q<Button>("Replay");
        Resurect = UI.rootVisualElement.Q<Button>("Resurect");
        Rating = UI.rootVisualElement.Q<Button>("Rating");

        Score = UI.rootVisualElement.Q<Label>("Score");
        Coins = UI.rootVisualElement.Q<Label>("Coins");


        Replay.RegisterCallback<ClickEvent>(CallbackReplay);
        Resurect.RegisterCallback<ClickEvent>(CallbackResurect);
        Rating.RegisterCallback<ClickEvent>(CallbackReplay);

        Setting.RegisterCallback<ClickEvent>((e) => {
            LittleSettings.visible = !LittleSettings.visible;
        });


    }
    public void ConnectPlayer(PlayerControl playerControl) => PlayerControl = playerControl;
    /*---------------------------DIE FRAME----------------------------*/
    public void Die()
    {
        StartCoroutine(ShowDieFrame());
    }
    private void CallbackReplay(ClickEvent e) {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void CallbackResurect(ClickEvent e)
    {
        Time.timeScale = 1;
        DieFrame.visible = false;
        StartCoroutine(ResurestAction());
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
        SetCoins(Coin);
        SetScore(Score);
    }
    public void SetScore(float value) => Score.text = "Score: " + value.ToString("f2");
    public void SetCoins(int value) => Coins.text = "Coins: " + value.ToString();
}
