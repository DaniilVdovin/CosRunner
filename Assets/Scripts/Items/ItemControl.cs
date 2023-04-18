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
        for (float i = 1; i >= 0; i-=0.05f)
        {
            this.gameObject.transform.localScale = new Vector3(i,i,i);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
}