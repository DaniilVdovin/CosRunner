using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


namespace Assets.Data
{
    internal class PrefencesController
    {
        private List<ShopItemScr> shopItems = new();
        public void add(ShopItemScr item)
        {
            if (!shopItems.Contains(item)) return;
            Debug.Log("Item ADded");
            PlayerPrefs.SetInt("ShopItemID:" + item.id, 1);
            shopItems.Where(x => x.id == item.id).First().Has = true;
            PlayerPrefs.Save();
        }

        
        public List<ShopItemScr> get()
        {
            foreach (var i in shopItems)
            {
                i.Has =  PlayerPrefs.GetInt("ShopItemID:" + i.id, 0)==1;
                i.Selected =  PlayerGeneralData.id_Prefs == i.id;
            }
            return shopItems;
        }
        public PrefencesController(List<ShopItemScr> datas)
        {
            
            shopItems = datas.Select(x => x.clone()).ToList();

        }
    }
}
