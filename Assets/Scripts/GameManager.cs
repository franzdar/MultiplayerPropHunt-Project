using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private TMP_Text[] playerList;
    Player[] allPlayers;
    public int playerCount;
    public TMP_Text playerStatus;
    public TMP_Text playerTagTimer;
    public TMP_Text activeProp;
    public TMP_Text propInfo;
    public GameObject leaveRoom;

    // Start is called before the first frame update
    void Start()
    {
        // check how many players in the room
        allPlayers = PhotonNetwork.PlayerList;
        playerCount = PhotonNetwork.PlayerList.Count();
        UpdatePlayerList();

        // spawn the player
        var newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint[playerCount-1].position, Quaternion.identity);
        Debug.Log("Players in room: " + playerCount);

        // if we're the host
        if (PhotonNetwork.IsMasterClient)
        {
            // Starts as tagged
            newPlayer.GetComponent<PlayerController>().photonView.RPC("OnTagged", RpcTarget.AllBuffered);
        }
        else
        {
            activeProp.gameObject.SetActive(true);
            propInfo.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        allPlayers = PhotonNetwork.PlayerList;
        playerCount = PhotonNetwork.PlayerList.Count();
        UpdatePlayerList();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(photonView);
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void BackToGame()
    {
        leaveRoom.SetActive(false);
        Cursor.visible = false;
    }

    // update the player list 
    public void UpdatePlayerList()
    {
        int listCount = 0;

        foreach (Player p in allPlayers)
        {
            Debug.Log(p + "inserted");
            playerList[listCount].text = p.NickName.ToString();
            listCount += 1;
        }
    }

    public void ClearPlayerList()
    {
        playerList[0].text = "<No Signal>";
        playerList[1].text = "<No Signal>";
        playerList[2].text = "<No Signal>";
        playerList[3].text = "<No Signal>";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Players in room: " + playerCount);
        Debug.Log("Player Joined Room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ClearPlayerList();
        Debug.Log("Players in room: " + playerCount);
        Debug.Log("Player Left Room");
    }
}
