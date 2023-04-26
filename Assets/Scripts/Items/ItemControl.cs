using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    public ItemModel model;
    public bool CanTake = true;
    public void Get(PlayerControl pc)
    {
        if (CanTake)
        {
            switch (model.Type)
            {
                case ItemModel.TType.Coin: pc.Coins += model.Value; break;
                case ItemModel.TType.Oxygen:{
                        pc.Oxygen += model.Value;
                        if (pc.Oxygen > 100)
                            pc.Oxygen = 100;}; break;
                case ItemModel.TType.Shield:
                    {
                        pc.ShieldCounddown = 10f; pc.isShield = true;
                        pc.ShieldMenu = pc.GameUI.AddExtraItem((int)ItemModel.TType.Shield, null, 10f, (s, i) =>
                        {
                            Debug.Log("Done " + i.id);
                            pc.isShield = false;
                        });
                    }
                    break;
                case ItemModel.TType.Magnit:
                    {
                        pc.GameUI.AddExtraItem((int)ItemModel.TType.Magnit, null, 10f, (s, i) =>
                        {
                            Debug.Log("Done " + i.id);
                        });
                    }
                    break;
            }

            StartCoroutine(GetAnination());
        }
    }

    private IEnumerator GetAnination()
    {
        GameObject er_temp = Instantiate(model.Effect, transform.position+Vector3.up*2, Quaternion.identity);
        for (float i = 1; i >= 0; i-=0.05f)
        {
            this.gameObject.transform.localScale = new Vector3(i,i,i);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
}
