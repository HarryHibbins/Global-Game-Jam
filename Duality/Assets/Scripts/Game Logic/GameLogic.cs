using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject killfeed;
    public GameObject killfeedItem;

    public GameObject sb;
    public GameObject pm;

    public PhotonView pv;

    private void Awake()
    {

    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();

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
    public void AddKillfeed(Player killer, Player victim, string weaponname)
    {
        pv.RPC("RPC_AddKillfeed", RpcTarget.All, killer, victim, weaponname);
    }

    [PunRPC]
    void RPC_AddKillfeed(Player killer, Player victim, string weaponname)
    {
        Debug.Log(weaponname);
        GameObject killFeedItem = Instantiate(killfeedItem, killfeed.transform);
        killFeedItem.GetComponent<KillfeedItem>().killerText.text = killer.NickName;
        killFeedItem.GetComponent<KillfeedItem>().victimText.text = victim.NickName;
        killFeedItem.GetComponent<KillfeedItem>().GetSprite(weaponname);

        Destroy(killFeedItem, 5);
    }
}

