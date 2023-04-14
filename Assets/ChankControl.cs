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
}
