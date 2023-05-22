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
        private List<ShopItemScr> shopItems = new() {
      
};

        public void add(ShopItemScr item)
        {
            if (!shopItems.Contains(item)) return;


            PlayerPrefs.SetInt("ShopItemID:" + item.id, 1);
            shopItems.Where(x => x.id == item.id).First().Has = true;
            PlayerPrefs.Save();


        }
        public List<ShopItemScr> get()
        {
            foreach (var i in shopItems)
            {
               i.Has =  PlayerPrefs.GetInt("ShopItemID:" + i.id, 0)==1?true:false;
            }
            return shopItems;
            
        }
        public PrefencesController(List<Mesh> prefs,Sprite icon)
        {
            int i = 0;
            foreach (var item1 in prefs)
            {
                ShopItemScr item = new ShopItemScr();
                item.id = i;
                i++;
                item.Name = item1.name;
                item.Prefab = item1;
                if(item.Has == false)
                    item.Icon = icon;
                item.Has = false;
                item.Price = (i + 1) * 10;
                shopItems.Add(item);

            }
            
        }
    }
}
