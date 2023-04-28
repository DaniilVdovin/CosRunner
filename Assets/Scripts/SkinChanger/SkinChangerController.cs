using Assets.Scripts.SkinChanger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkinChangerController
{
    private static SkinnedMeshRenderer _skin_Data;
    public static SkinnedMeshRenderer SkinData { get { return _skin_Data; } set { _skin_Data = value; } }
    internal static List<SkinModel> curentSkinList = new List<SkinModel>() {new SkinModel()};

    public static void SetSkinData(int id)
    {
        Debug.Log("nowSkin is " + id);

    }
    public static SkinnedMeshRenderer GetSkinData()
    {
        return SkinData;
    }
    
}
