using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GooglePlayGames;
using UnityEngine;

public class LeaderBoadConf : MonoBehaviour
{

    public const string LeaderboardId = "CgkIqcKH3KUWEAIQAA";
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static void AddScoreAsync(float score)
    {
        PlayGamesPlatform.Instance.ReportScore((long)score, LeaderboardId, (bool success) => {
             // handle success or failure
         });
    }
    public static async Task<List<LeaderBoardItem>> GetPlayerRangeAsync()
    {
        List<LeaderBoardItem> res = new();
        
        //foreach (var item in )
        //{
        //    res.Add(new LeaderBoardItem()
        //    {
        //        id = item.Rank,
        //        Name = item.PlayerName,
        //        Score = (float)item.Score,
        //        itsMe = item.PlayerId == PlayerGeneralData.ID
        //    });
        //}
        
        return res;
    }
}
