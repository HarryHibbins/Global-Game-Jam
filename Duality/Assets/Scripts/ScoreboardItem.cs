using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public string username;
    public int _id;
    public int kills;
    public int deaths;

    public Player player;

    private void Update()
    {
        username = player.NickName;
        _id = player.ActorNumber;
        kills = (int)player.CustomProperties["Kills"];
        deaths = (int)player.CustomProperties["Deaths"];

        usernameText.text = username;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
    }
}
