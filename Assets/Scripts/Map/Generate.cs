using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Generate : MonoBehaviour
{
    public enum Side
    {
        Rigth,
        Left,
        Forward,
    }
    public struct TTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject Chank;
    }
    public GameObject[] Chanks;
    public List<GameObject> Map;
    public GameObject MapParent;
    public Side LastSide = Side.Forward;
    private int SideIter = 4;
    private int offset = 20;
    private int ChankCount = 20;
    private int MemCount = 40;
    public PlayerControl PlayerControl;
    private bool isGenerate = false;
    private WaitForSeconds wait = new WaitForSeconds(.2f);
    public GameObject PrefCoin;
    private int CoinnPosOlds = 1;
    private bool firstGeneration = true;
    public void StartGenerate()
    {
        StartCoroutine(GenerateRoad(ChankCount * 2));
    }
    private void FixedUpdate()
    {
        if (!isGenerate && PlayerControl.ChankNow != null)
            if (Map.LastIndexOf(PlayerControl.ChankNow.gameObject) == Map.Count - ChankCount)
                StartCoroutine(GenerateRoad(ChankCount));
    }
    IEnumerator GenerateRoad(int count)
    {
        isGenerate = true;
        int addition = Map.Count;
        for (int i = addition; i < addition + count; i++)
        {
            GameObject last = MapParent;
            if (Map.Count != 0) last = Map.Last();
            if (Map.Count > MemCount) {
                if (!Map[i - MemCount].CompareTag("Map_rot"))
                {
                    Map.Add(Map[i - MemCount]);
                    TTransform temp = GetNextPosotion(last.transform.position,true);
                    if (temp.Chank != null)
                    {
                        Destroy(Map[i]);
                        ChankReplace(i,temp);
                    }
                    else ChankMove(temp);
                }
                else
                {
                    Destroy(Map[i - MemCount]);
                    AddChank(GetNextPosotion(last.transform.position));
                }
            }
            else AddChank(GetNextPosotion(last.transform.position));
            yield return wait;
        }
        StartCoroutine(GenerateCoins(addition, count));
        isGenerate = false;
        if (firstGeneration)
        {
            PlayerControl.isRun = true;
            firstGeneration = false;
        }
    }
    private void AddChank(TTransform temp)
        => Map.Add(ChankInstantiate(temp));
    private GameObject ChankInstantiate(TTransform temp)
        => Instantiate(temp.Chank, temp.position, temp.rotation, MapParent.transform);
    private void ChankReplace(int i,TTransform temp)
        => Map[i] = ChankInstantiate(temp);
    private void ChankMove(TTransform temp)
    {
        Map.Last().transform.SetPositionAndRotation(temp.position, temp.rotation);
        Map.Last().GetComponent<ChankControl>().Regenerate();
    }
    private IEnumerator GenerateCoins(int Start, int End)
    {
        End += Start;
        int pos = CoinnPosOlds;
        int ccount = 0;
        for (int i = Start; i < End; i++)
        {
            if (Map[i] == null) continue;
            if (!Map[i].CompareTag("Map_rot"))
            {
                if (ccount == 15)
                {
                    int pos_old = pos;
                    pos = Random.Range(0, 3);
                    int dif = Mathf.Abs(pos_old - pos);
                    ccount = dif;
                    for (int l = 0; l < dif; l++)
                    {
                        CreateCoin(Map[i], new Vector3((pos_old > pos
                            ? -1.1f : 1.1f), 0f, -10f + (1.5f *l)));
                    }
                    
                }
                Vector3 vector = Vector3.zero;
                vector.x = -5f + (5f * pos);
                vector.z = -7f;
                for (int c = 0; c < 5; c++)
                {
                    CreateCoin(Map[i], vector);
                    vector.z += 3.5f;
                    ccount++;
                    yield return new WaitForSeconds(0.05f);
                }
            }
            else
            {
                pos = Random.Range(0, 3);
                ccount = 0;
            }
        }
        CoinnPosOlds = pos;
    }
    void CreateCoin(GameObject Parent, Vector3 Position)
    {
        GameObject coin = Instantiate(PrefCoin, Parent.transform);
        coin.transform.localPosition = transform.position + Position;
        Parent.GetComponent<ChankControl>().Coins.Add(coin);
    }
    TTransform GetNextPosotion(Vector3 LastPosition,bool move = false) {
        int side = 0;
        side = (int)LastSide;
        TTransform result = new TTransform();
        
        switch (side) {
            case ((int)Side.Forward):
                result.position = Vector3.forward; break;
            case ((int)Side.Left):
                result.position = Vector3.left; break;
            case ((int)Side.Rigth):
                result.position = Vector3.right; break;
        }
        SideIter--;
        if (SideIter == 0)
        {
            LastSide = (Side)MixRand((int)LastSide, 0, 3);
            SideIter = 4;
        }
        if (LastSide != (Side)side)
        {
            result.Chank = Chanks[1];
            if (side == (int)Side.Forward)
                if (LastSide == Side.Left)
                    result.rotation = Quaternion.Euler(0, 90, 0);
                else
                    result.rotation = Quaternion.Euler(0, 0, 0);
            if (side == (int)Side.Rigth)
                if (LastSide == Side.Forward)
                    result.rotation = Quaternion.Euler(0, 180, 0);
            if (side == (int)Side.Left)
                if (LastSide == Side.Forward)
                    result.rotation = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            result.Chank = Chanks[0];
            switch (LastSide)
            {
                case Side.Forward:
                    result.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Side.Left:
                    result.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case Side.Rigth:
                    result.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }
            if (move) result.Chank = null;
        }
        result.position *= offset;
        result.position += LastPosition;
        return result;
    }
    int MixRand(int x,int min,int max)
    {
        switch ((Side)x)
        {
            case Side.Forward: return Random.Range(min, max-1);
            case Side.Left:    return URand((int)Side.Rigth, min, max);
            case Side.Rigth:   return URand((int)Side.Left, min, max);
            default: return (int)Side.Forward;
        }
    }
    int URand(int x,int min,int max) {
        int temp = Random.Range(min, max);
        if(temp != x) return temp;
        else return URand(x, min, max);
    }
}
