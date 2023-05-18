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
        new()
        {
             id = 0,
             Name = "Black",
             Prefab = null,
             Icon = null,
             Has = false,
             Price = 20
        },
        new()
        {
             id = 1,
             Name = "Yellow",
             Prefab = null,
             Icon = null,
             Has = false,
             Price = 20
         },
        new ()
        {
             id = 2,
             Name = "Green",
             Prefab = null,
             Icon = null,
             Has = false,
             Price = 20
         },
        new ()
        {
             id = 3,
             Name = "RED",
             Prefab = null,
             Icon = null,
             Has = false,
             Price = 20
        },
        new ()
        {

             id = 4,
             Name = "White",
             Prefab = null,
             Icon = null,
             Has = false,
             Price = 20
        }


};
        public void add(ShopItemScr item)
        {
            if (!shopItems.Contains(item)) return;
                
                
            PlayerPrefs.SetInt("ShopItemID:"+item.id,1);
            shopItems.Where(x => x.id == item.id).First().Has=true;

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
    }
}
