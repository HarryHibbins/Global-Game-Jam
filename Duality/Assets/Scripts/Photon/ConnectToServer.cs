using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;

    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby");
    }

    /*public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");

    }*/

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
