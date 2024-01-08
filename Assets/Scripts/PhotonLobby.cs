using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject createButton;
    public GameObject joinButton;
    public GameObject cancelButton;
    public GameObject exitButton;
    public GameObject offlineButton;

    public bool isBtnCreate = false;

    //navigation
    public GameObject gameManager;
    public GameObject lobbyUI;
    public InputField inputCreate;
    public InputField inputJoin;

    public string roomToJoin; //reference for the photon room to join
    
    private void Awake() {
        lobby = this;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //connects to photon server
        offlineButton.gameObject.SetActive(true); //enable if photon connection is not yet established
    }

    // Update is called once per frame
    void Update()
    {
        roomToJoin = inputJoin.text; //checks input field for Join
    }

    //if the connection is established
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has successfully connected to the Photon Master server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        lobbyUI.SetActive(true);
        offlineButton.SetActive(false);
    }

    public void OnPlayButtonClicked()
    {
        if(roomToJoin == "")
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Joining room...");
        }
        else
        {
            PhotonNetwork.JoinRoom(roomToJoin);
            Debug.Log("Joining room...");
        }

        createButton.SetActive(false);
        joinButton.SetActive(false);
        cancelButton.SetActive(true);
    }

    //creates a network room
    void CreateRoom()
    {
        if (isBtnCreate)
        {
            Debug.Log("Creating a new room...");
            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 5};
            PhotonNetwork.CreateRoom(inputCreate.text, roomOps);
            Debug.Log(inputCreate.text + " Created.");
        }
    }

    //if successfully joined room
    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully entered a room.");
        lobbyUI.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        PhotonNetwork.LoadLevel("Main");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Joined random room but failed. There must be no open room.");
        CreateRoom();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed, there must be an existing room of the same name.");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Canceled.");
        cancelButton.SetActive(false);
        createButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
