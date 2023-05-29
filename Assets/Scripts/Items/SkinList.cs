using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public  class SkinList:List<ShopItemScr>
    {
        public SkinList() {
            this.Add(new()
            {
                id = 0,
                _has = true,
                Price = 0,
                Prefab = Resources.Load<Mesh>("Player_meshes/StandUp2"),
                Icon = Resources.Load<Sprite>("Player_icons/default_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/default_mask")
            }
            );
            this.Add(new()
            {
                id = 1,
                _has = false,
                Price = 150,
                Prefab = Resources.Load<Mesh>("Player_meshes/StandUp3"),
                Icon = Resources.Load<Sprite>("Player_icons/batman_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/batman_mask")
            }
                );
            this.Add(new()
            {
                id = 2,
                _has = false,
                Price = 600,
                Prefab = Resources.Load<Mesh>("Player_meshes/SKIN_DEVIL"),
                Icon = Resources.Load<Sprite>("Player_icons/devil_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/devil_mask")
            });
            this.Add(new()
            {
                id = 3,
                _has = false,
                Price = 1200,
                Prefab = Resources.Load<Mesh>("Player_meshes/Mexican"),
                Icon = Resources.Load<Sprite>("Player_icons/mexican_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/mexican_mask")
            });
            this.Add(new()
            {
                id = 4,
                _has = false,
                Price = 1900,
                Prefab = Resources.Load<Mesh>("Player_meshes/Rock"),
                Icon = Resources.Load<Sprite>("Player_icons/rock_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/rock_mask")
            });
            this.Add(new()
            {
                id = 5,
                _has = false,
                Price = 2100,
                Prefab = Resources.Load<Mesh>("Player_meshes/Rocket"),
                Icon = Resources.Load<Sprite>("Player_icons/rocket_icon"),
                Icon_mask = Resources.Load<Sprite>("Player_icons/rocket_mask")
            });
        }
    }
}
