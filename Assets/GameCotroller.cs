using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCotroller : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadSceneAsync("NDY", LoadSceneMode.Additive);
    }
}
