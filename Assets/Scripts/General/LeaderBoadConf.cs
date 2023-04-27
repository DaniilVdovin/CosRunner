using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderBoadConf : MonoBehaviour
{

    private const string LeaderboardId = "SpaceRunner";
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static async void AddScoreAsync(float score)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized) return;
        if (!AuthenticationService.Instance.IsSignedIn) return;

        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(LeaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
    public static void AddScore(float score) => AddScoreAsync(score);
    public static async Task<List<LeaderBoardItem>> GetPlayerRangeAsync()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized) return new();
        if (!AuthenticationService.Instance.IsSignedIn) return new();

        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        List<LeaderBoardItem> res  = new();
        foreach (var item in scoresResponse.Results)
        {
            res.Add(new LeaderBoardItem()
            {
                id = item.Rank,
                Name = item.PlayerName,
                Score = (float)item.Score,
            });
        }
        return res;
    }
    public static List<LeaderBoardItem> GetPlayerRange() => GetPlayerRangeAsync().Result;
}
