using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameLogic : MonoBehaviourPunCallbacks
{
    public List<Player> players = new List<Player>();

    public string player1name;
    public int player1kills;
    public int player1deaths;
    [Space(5)]
    public string player2name;
    public int player2kills;
    public int player2deaths;

    public GameObject scoreboardItem;
    public GameObject scoreboard;

    public GameObject sb;
    public GameObject pm;

    private void Awake()
    {

    }

    private void Start()
    {
        Debug.Log("SPAWN");
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            players.Add(p);
            SetPlayer(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        players.Add(newPlayer);
        SetPlayer(newPlayer);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        players.Remove(otherPlayer);
        foreach (Transform sb in scoreboard.transform)
        {
            if (sb.GetComponent<ScoreboardItem>()._id == otherPlayer.ActorNumber)
            {
                Destroy(sb.gameObject);
            }
        }
    }


    public void UpdatePlayer(Player player, int kills, int deaths)
    {
        Hashtable hash = new Hashtable();
        int currentKills = (int)player.CustomProperties["Kills"];
        int currentDeaths = (int)player.CustomProperties["Deaths"];
        hash.Add("Kills", currentKills + kills);
        hash.Add("Deaths", currentDeaths + deaths);
        player.SetCustomProperties(hash);
    }

    void SetPlayer(Player player)
    {
        Hashtable hash = new Hashtable();
        int kills, deaths;
        if (player.CustomProperties["Kills"] != null)
        {
            kills = (int)player.CustomProperties["Kills"];
        }
        else
        {
            kills = 0;
        }
        if (player.CustomProperties["Deaths"] != null)
        {
            deaths = (int)player.CustomProperties["Deaths"];
        }
        else
        {
            deaths = 0;
        }
        hash.Add("Kills", kills);
        hash.Add("Deaths", deaths);
        player.SetCustomProperties(hash);

        GameObject scoreBoardItem = Instantiate(scoreboardItem, scoreboard.transform);
        scoreBoardItem.GetComponent<ScoreboardItem>().player = player;
    }
}

