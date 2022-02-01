using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    public Player player;
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public string username;
    public int _id;
    public int kills;
    public int deaths;

    private void Update()
    {
        if (player.CustomProperties["Kills"] != null)
        {
            kills = (int)player.CustomProperties["Kills"];
        }
        if (player.CustomProperties["Deaths"] != null)
        {
            deaths = (int)player.CustomProperties["Deaths"];
        }
        username = player.NickName;
        _id = player.ActorNumber;
        usernameText.text = username;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
    }
}
