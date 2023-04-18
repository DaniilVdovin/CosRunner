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
                case ItemModel.TType.Oxygen: break;
            }
            StartCoroutine(GetAnination());
        }
    }
    private IEnumerator GetAnination()
    {
        GameObject er_temp = Instantiate(model.Effect, transform.position+Vector3.up*2, Quaternion.identity);
        for (int i = 0; i < max; i++)
        {

        }
        this.gameObject.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
        Destroy(er_temp);
    }
}
