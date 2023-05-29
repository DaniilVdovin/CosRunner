using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Data;
using Assets.Scripts.Items;
using DG.Tweening;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ShopItemScr
{
    public int id;
    private string _name;

    public ShopItemScr(ShopItemScr arg)
    {
        this.id = arg.id;
        this._name = arg.Name;
        this.Icon = arg.Icon;
        this._has = arg.Has;
        this.Icon_mask = arg.Icon_mask;
        this.Price = arg.Price;
        this.Prefab = arg.Prefab;
        
    }

    public ShopItemScr()
    {
    }

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
    public Mesh Prefab;
    public int Price;
    public Sprite Icon;
    public Sprite Icon_mask;
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
    public Action<ShopItemScr, ClickEvent> EventClick;
    public Action<ShopItemScr> EventUpdate;
    private void ClickEvent(ClickEvent e) => EventClick.Invoke(this, e);
    

    public void Update() {
        if (_template != null)
        {
            template.Q<VisualElement>("ItemSelected").visible = _selected;
            if (_has)
            {
                template.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(Icon);
                template.Q<Label>("Price").text = "Have";
            }
            else
            {
                template.Q<Label>("Price").text = Price == 0 ? "FREE" : Price.ToString();
                template.Q("Icon").style.backgroundImage = new StyleBackground(Icon_mask);
            }
        }
        if (EventUpdate != null)
            EventUpdate.Invoke(this);
    }
    public ShopItemScr clone()
    {
        return new ShopItemScr(this);
    }
}
public class ShopUI : MonoBehaviour
{
    private List<ShopItemScr> items = new();
    public static List<ShopItemScr> staItem;
    public List<Sprite> ShopItemIcons;
    private VisualElement UI;
    private VisualElement Holder;
    public VisualTreeAsset Def_Item;
    public Label Coin;
    public Sprite SpriteLock;
    public Button Close;
    public GameCotroller Menu;
    private PrefencesController fabris;

    private void Awake()
    {
        items = new SkinList();
    }
    void Start()
    {
        fabris = new PrefencesController(items);
        LoadItems();
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
    private void LoadItems()
    {
       
        foreach(var Item in items)
        {
            Debug.Log(Item);
        }
       staItem = fabris.get();
    }
    private IEnumerator Generate()
    {
        Holder.Clear();

        foreach (var item in staItem)
        {
            TemplateContainer temp = Def_Item.Instantiate();
            temp.style.opacity = 0;
            item.template = temp;
            temp.name = item.Name;
            item.EventClick += ClickEvent;
            item.EventUpdate += ItemUpdate;
            Holder.Add(temp);
            DOTween.To(() => 0f, x => temp.style.opacity = x
                    , 1f, .5f)
            .SetEase(Ease.Linear);
            yield return new WaitForSeconds(.1f);
            Debug.Log(item._has); 
        }
    }
    private void ItemUpdate(ShopItemScr item)
    {
        bool allunsel = false;
        staItem.ForEach((i) => allunsel = i.Selected || allunsel);
        if (!allunsel) { staItem[0].Selected = true; }
    }

    //click
    private void ClickEvent(ShopItemScr item,ClickEvent e)
    {
        //Instantiate(item.Prefab);
        if (item._has) {
            Selected(item);
        }
        else {
            Debug.Log("buy");
            Buy(item);
        }
        //UPDATE
        item.Update();
    }
    private void Selected(ShopItemScr item)
    {
        item.Selected = !item.Selected;
        staItem.Where((i) => i != item).ToList().ForEach((i) => i.Selected = false);
        PlayerGeneralData.id_Prefs = item.id;
    }

    private void Buy(ShopItemScr item)
    {
        if (PlayerGeneralData.Coins >= item.Price)
        {
            PlayerGeneralData.Coins -= item.Price;
            item._has = true;
            Selected(item);
            fabris.add(item);
            Debug.Log("You buy item id:"+item.id);
        }
        else
        {
            Debug.Log("Dont Have Coins");
        }
    }
    public void StartShop()
    {
        UI.visible = true;
        UPD(null, null);
        StartCoroutine(Generate());
    }
    private void ShopClose(ClickEvent evt)
    {
        UI.visible = false;
        staItem.ForEach((i) => {
            i.template.Q<VisualElement>("ItemSelected").visible = false;
            i.EventClick -= ClickEvent;
            i.EventUpdate -= ItemUpdate;
        });
        Menu.Menu.visible = true;
    }
    private void OnDestroy()
    {
        PlayerGeneralData.StatsUpdate -= UPD;
    }
}
