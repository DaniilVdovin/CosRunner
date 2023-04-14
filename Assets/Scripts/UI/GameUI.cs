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
    private void Start()
    {
        UI = GetComponent<UIDocument>();
        Setting = UI.rootVisualElement.Q<Button>("Settings");
        LittleSettings = UI.rootVisualElement.Q<GroupBox>("LittleSettings");
        EngGameFrame = UI.rootVisualElement.Q<VisualElement>("EngGameFrame");
        Setting.RegisterCallback<ClickEvent>((e) => {
            LittleSettings.visible = !LittleSettings.visible;
        });
    }
}
