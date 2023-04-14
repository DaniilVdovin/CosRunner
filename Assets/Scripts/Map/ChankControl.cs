using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChankControl : MonoBehaviour
{
    public enum Ttype
    {
        Floor,
        Pivot
    }
    public bool WeRot = false;
    public Ttype type;
    public Transform[] Lines;
    public GameObject[] Stoping;

    public void Start()
    {
        if(type == Ttype.Floor)
        if (Random.Range(0,5)==2)
        {
            Instantiate(Stoping[Random.Range(0, Stoping.Length - 1)],
                Lines[Random.Range(0, Lines.Length - 1)].transform.position, Quaternion.identity, transform);
        }
    }
}
