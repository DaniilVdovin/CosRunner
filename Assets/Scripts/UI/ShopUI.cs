using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopItem
{
    public string Name;
    public bool Has;
    public GameObject Prefab;
    public int Price;
    public Sprite Icon;
    private TemplateContainer _template;
    public TemplateContainer template { get=> _template; set {
            _template = value;
            _template.RegisterCallback<ClickEvent>(ClickEvent);
        }}
    public Action<ShopItem, ClickEvent> Click;
    private void ClickEvent(ClickEvent e) => Click.Invoke(this,e);
}
public class ShopUI : MonoBehaviour
{
    public List<ShopItem> items;

    private UIDocument UI;
    private VisualElement Holder;
    public VisualTreeAsset Def_Item;
    public Sprite SpriteLock;

    void Start()
    {
        UI = GetComponent<UIDocument>();
        Holder = UI.rootVisualElement.Q<VisualElement>("ShopContainer");

        LoadItems();
        Generate();
    }
    private void LoadItems()
    {
        //TEST
        items = new();
        for (int i = 0; i < 30; i++)
        {
            ShopItem temp = new()
            {
                Name = "Pers: " + i,
                Prefab = Resources.Load("Player/Stickman1", typeof(GameObject)) as GameObject,
                Icon = Resources.Load("UI/LOCK", typeof(Sprite)) as Sprite,
                Has = false,
                Price = 100 * i
            };
            items.Add(temp);
        }
    }
    private void Generate()
    {
        foreach (var item in items)
        {
            TemplateContainer temp = Def_Item.Instantiate();
            temp.name = item.Name;
            temp.Q<Label>("Price").text = item.Price==0?"FREE":item.Price.ToString();
            temp.Q<VisualElement>("Icon").style.backgroundImage = new(item.Icon != null ? item.Icon : SpriteLock);
            item.template = temp;
            item.Click += (i, e) =>
            {
                Debug.Log("CLICK: " + i.Name);
            };
            Holder.Add(temp);
        }
    }
}
