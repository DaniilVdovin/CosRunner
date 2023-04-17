using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private UIDocument UI;
    private Button Setting;
    private GroupBox LittleSettings;
    private VisualElement EngGameFrame;

    private Label Score;
    private Label Coins;

    public PlayerControl PlayerControl;
    private void Start()
    {
        UI = GetComponent<UIDocument>();
        Setting = UI.rootVisualElement.Q<Button>("Settings");
        LittleSettings = UI.rootVisualElement.Q<GroupBox>("LittleSettings");
        EngGameFrame = UI.rootVisualElement.Q<VisualElement>("EngGameFrame");
        Score = UI.rootVisualElement.Q<Label>("Score");
        Coins = UI.rootVisualElement.Q<Label>("Coins");
        Setting.RegisterCallback<ClickEvent>((e) => {
            LittleSettings.visible = !LittleSettings.visible;
        });
    }
    private void FixedUpdate()
    {
        SetScore(PlayerControl.Score);
        SetCoins(PlayerControl.Coins);
    }
    void SetScore(float value) => Score.text = "Score: " + value.ToString("f2");
    void SetCoins(int value) => Coins.text = "Coins: " + value.ToString();
}
