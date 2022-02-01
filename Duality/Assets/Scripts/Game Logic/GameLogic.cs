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
        players = new List<Player>();
    }

    private void Start()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            players.Add(p);
            SetPlayer(p);
        }
    }

    private void Update()
    {
        /*if (players.Count > 0)
        {
            player1name = players[0].NickName;
            player1kills = (int)players[0].CustomProperties["Kills"];
            player1deaths = (int)players[0].CustomProperties["Deaths"];
        }
        if (players.Count > 1)
        {
            player2name = players[1].NickName;
            player2kills = (int)players[1].CustomProperties["Kills"];
            player2deaths = (int)players[1].CustomProperties["Deaths"];
        }*/
        //Debug.Log(sblist[0].GetComponent<ScoreboardItem>()._id);
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
        hash.Add("Kills", 0);
        hash.Add("Deaths", 0);
        player.SetCustomProperties(hash);

        GameObject scoreBoardItem = Instantiate(scoreboardItem, scoreboard.transform);
        scoreBoardItem.GetComponent<ScoreboardItem>().player = player;
    }
}

