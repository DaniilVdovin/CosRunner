using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<GameObject> Coins;
    public List<GameObject> Stopings;

    public GameObject[] HardChankItems;

    public void Start()
    {
        Generate();
    }
    private void Generate()
    {
        if (Lines.Length > 0)
            if (type == Ttype.Floor) {
                if (Random.Range(0, 5) == 2)
                {
                    Stopings.Add(Instantiate(Stoping[Random.Range(0, Stoping.Length)],
                        Lines[Random.Range(0, Lines.Length)].transform.position, Quaternion.identity, transform));
                }
                if (Random.Range(0, 5) == 3) {
                    Stopings.Add(Instantiate(HardChankItems[Random.Range(0, HardChankItems.Length)],
                        transform.position,Quaternion.identity,transform));
                    Stopings.Last().transform.localRotation = Quaternion.Euler(0, (Random.Range(0, 2) == 1 ? 0 : 180), 0);
                }
             }
    }
    public void Regenerate()
    {
        Clear();
        Generate();
    }
    public void Clear() {
        foreach (var item in Coins)    Destroy(item);
        foreach (var item in Stopings) Destroy(item);
        Coins.Clear(); Stopings.Clear();
    }
}
