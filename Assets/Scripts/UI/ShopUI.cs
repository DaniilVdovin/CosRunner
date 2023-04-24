using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Item Model", menuName = "Items/Shop Item Model", order = 2)]
public class ShopItem : ScriptableObject
{
    public int id;
    public string Name;
    public bool Has;
    public GameObject Prefab;
    public int Price;
    public Sprite Icon;
    private bool _selected = false;
    public bool Selected
    {
        get => _selected; set
        {
            _selected = value;
            if (_template != null)
                template.Q<VisualElement>("ItemSelected").visible = _selected;
        }
    }
    private TemplateContainer _template;
    public TemplateContainer template
    {
        get => _template; set
        {
            _template = value;
            _template.RegisterCallback<ClickEvent>(ClickEvent);
        }
    }
    public Action<ShopItem, ClickEvent> Click;
    private void ClickEvent(ClickEvent e) => Click.Invoke(this, e);
}
public class ShopUI : MonoBehaviour
{
    public List<ShopItem> items;

    private VisualElement UI;
    private VisualElement Holder;
    public VisualTreeAsset Def_Item;
    public Sprite SpriteLock;
    public Button Close;

    public GameCotroller Menu;

    void Start()
    {
        Menu = GetComponent<GameCotroller>();
        UI = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ShopUI");
        Holder = UI.Q<VisualElement>("ShopContainer");
        Close = UI.Q<Button>("ShopClose");
        Close.RegisterCallback<ClickEvent>(ShopClose);
        UI.visible = false;
    }
    private void LoadItems()//TEST
    {
        items = new();
        for (int i = 0; i < 30; i++)
        {
            ShopItem temp = new()
            {
                Name = "Pers: " + i,
                Prefab = null,
                Icon = null,
                Has = false,
                Price = 100 * i
            };
            items.Add(temp);
        }
    }
    private IEnumerator Generate()
    {
        Holder.Clear();
        foreach (var item in items)
        {
            TemplateContainer temp = Def_Item.Instantiate();
            temp.style.opacity = 0;
            temp.name = item.Name;
            if (item.Has)
                temp.Q<Label>("Price").text = "Have";
            else temp.Q<Label>("Price").text = item.Price == 0 ? "FREE" : item.Price.ToString();
            temp.Q<VisualElement>("Icon").style.backgroundImage = new(item.Icon != null ? item.Icon : SpriteLock);
            item.template = temp;
            item.Selected = false;
            item.Click += ClickEvent;
            Holder.Add(temp);
            DOTween.To(() => 0f, x => temp.style.opacity = x
                    , 1f, .5f)
            .SetEase(Ease.Linear);
            yield return new WaitForSeconds(.1f);
        }
    }
    private void ClickEvent(ShopItem item,ClickEvent e)
    {
        //Instantiate(item.Prefab);
        Debug.Log(item.Name);

        if (item.Has) {
            item.Selected = !item.Selected;
        }
        else {
            item.Has = true;
        }
    }
    public void StartShop()
    {
        UI.visible = true;
        LoadItems();//TEST
        StartCoroutine(Generate());
    }
    private void ShopClose(ClickEvent evt)
    {
        UI.visible = false;
        Menu.Menu.visible = true;
    }
    private void OnDestroy()
    {
        Close.UnregisterCallback<ClickEvent>(ShopClose);
    }
}
