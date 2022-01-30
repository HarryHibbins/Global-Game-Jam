using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    public TMP_Text playerName;
    private Image backgroundImage;
    public Color highlightColor;

    public GameObject leftArrow;
    public GameObject rightArrow;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
    }

    public void ApplyLocalChanges()
    {
        //backgroundImage.color = highlightColor;
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }
}
