using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Model", menuName = "Items/Item Model", order = 1)]
public class ItemModel : ScriptableObject
{
    public enum TType
    {
        Coin,
        Oxygen,
        Shield,
        Magnit
    }
    public TType Type;
    public int Value;
    public float Duration;
    public GameObject Effect;
}
