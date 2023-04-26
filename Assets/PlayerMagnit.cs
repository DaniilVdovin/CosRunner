using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagnit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "item")
        {
            Gets(other.gameObject);
        }
    }
    IEnumerator Gets(GameObject gameObject)
    {
        while (gameObject.activeInHierarchy)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, transform.position, Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
