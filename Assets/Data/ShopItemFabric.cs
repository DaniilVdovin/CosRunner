using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine.Playables;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace Assets.Data
{
    internal class ShopItemFabric
    {
        private  ShopItem item;
        private GameData Controller;

        public ReadOnlyList<ShopItem> TakeAllShopItems()
        {
            var fileStream = File.OpenRead("Assets/Data/gamedata.xml");

        var gameData = new GameData(fileStream, GameData.Format.Xml);
            return gameData.GetShopItems();
        }
    }
}
