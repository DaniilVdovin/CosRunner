using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Item Model", menuName = "Items/Shop Item Model", order = 2)]
public class ShopItem : ScriptableObject
{
    public int id;
    private string _name;
    
    public string Name
    {
        get => _name; set
        {
            _name = value;
            Update();
        }
    }
    public bool _has;
    public bool Has
    {
        get => _has; set
        {
            _has = value;
            Update();
        }
    }
    public GameObject Prefab;
    public int Price;
    public Sprite Icon;
    private bool _selected;
    public bool Selected
    {
        get => _selected; set
        {
            _selected = value;
            Update();
        }
    }
    private TemplateContainer _template;
    public TemplateContainer template
    {
        get => _template; set
        {
            _template = value;
            _template.RegisterCallback<ClickEvent>(ClickEvent);
            Update();
        }
    }
    public Action<ShopItem, ClickEvent> EventClick;
    public Action<ShopItem> EventUpdate;
    private void ClickEvent(ClickEvent e) => EventClick.Invoke(this, e);

    public void Update() {
        if (_template != null)
        {
            template.Q<VisualElement>("ItemSelected").visible = _selected;
            if (_has) template.Q<Label>("Price").text = "Have";
            else template.Q<Label>("Price").text = Price == 0 ? "FREE" : Price.ToString();
        }
        if (EventUpdate != null)
            EventUpdate.Invoke(this);
    }
}
public class ShopUI : MonoBehaviour
{
    public List<ShopItem> items;

    private VisualElement UI;
    private VisualElement Holder;
    public VisualTreeAsset Def_Item;
    public Label Coin;
    public Sprite SpriteLock;
    public Button Close;

    public GameCotroller Menu;

    void Start()
    {
        Menu = GetComponent<GameCotroller>();
        UI = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ShopUI");
        Holder = UI.Q<VisualElement>("ShopContainer");
        Coin = UI.Q<Label>("Coins");
        Close = UI.Q<Button>("ShopClose");
        Close.RegisterCallback<ClickEvent>(ShopClose);
        UI.visible = false;
        PlayerGeneralData.StatsUpdate += UPD;
    }
    private void UPD(object s, EventArgs e)
    {
        Coin.text = "Coins: " + PlayerGeneralData.Coins;
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
            item.template = temp;
            temp.name = item.Name;
            item.EventUpdate += ItemUpdate;
            item.EventClick += ClickEvent;
            Holder.Add(temp);
            DOTween.To(() => 0f, x => temp.style.opacity = x
                    , 1f, .5f)
            .SetEase(Ease.Linear);
            yield return new WaitForSeconds(.1f);
        }
    }
    private void ItemUpdate(ShopItem item)
    {
        //Debug.Log(item.Name);
        bool allunsel = false;
        items.ForEach((i) => allunsel = i.Selected?true:allunsel);
        if (!allunsel) { items[0].Selected = true; }
    }
    private void ClickEvent(ShopItem item,ClickEvent e)
    {
        //Instantiate(item.Prefab);
        Debug.Log(item.Name);

        if (item.Has) {
            item.Selected = !item.Selected;
            items.Where((i) => i != item).ToList().ForEach((i) => i.Selected = false);
        }
        else {
            Buy(item);
        }

        //UPDATE
        item.Update();
    }
    private void Buy(ShopItem item)
    {
        if (PlayerGeneralData.Coins >= item.Price)
        {
            PlayerGeneralData.Coins -= item.Price;
            PlayerGeneralData.id_Prefs = item.id;
            item.Has = true;
            Debug.LogWarning("You buy item id:"+item.id);
        }
        else
        {
            //Dont Have Coins
            Debug.LogWarning("Dont Have Coins");
        }
    }
    public void StartShop()
    {
        UI.visible = true;
        //LoadItems();//TEST
        UPD(null, null);
        StartCoroutine(Generate());
    }
    private void ShopClose(ClickEvent evt)
    {
        UI.visible = false;
        items.ForEach((i) => {
            i.template.Q<VisualElement>("ItemSelected").visible = false;
            i.EventClick -= ClickEvent;
            i.EventUpdate -= ItemUpdate;
            });
        Menu.Menu.visible = true;
    }
}
