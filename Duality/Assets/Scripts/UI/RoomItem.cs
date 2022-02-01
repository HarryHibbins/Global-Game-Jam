using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    public TMP_Text playersText;
    private CreateAndJoinRooms manager;

    private void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void SetRoomCount(int current, int max)
    {
        playersText.text = current.ToString() + "/" + max.ToString();
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
