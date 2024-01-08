using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HnSGameLobby : MonoBehaviourPunCallbacks
{
    public static HnSGameLobby lobby;

    public Button playButton;
    public Button cancelButton;
    public GameObject exitButton;
    public GameObject offlineButton;

    public GameObject gameManager;
    public GameObject lobbyUI;
    public InputField inputName;

    public string playerName; 
    public Text statusText;
    
    private void Awake() {
        lobby = this;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        //connects to photon server
        PhotonNetwork.ConnectUsingSettings();

        //enable if photon connection is not yet established
        offlineButton.gameObject.SetActive(true);

        //config photon
        PhotonNetwork.AutomaticallySyncScene = true;

        //restore the player's previous name input
        inputName.text = PlayerPrefs.GetString("playername"); 
    }

    // Update is called once per frame
    void Update()
    {
        //checks input field for Name
        playerName = inputName.text; 
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
        //Make sure the player entered a name first
        if(playerName == "")
        {
            Debug.Log("Input a name first...");
            SetStatus("Please enter a name");
        }

        // Set the player's nickname and create room
        else
        {
            PhotonNetwork.NickName = inputName.text;

            //save player's name for future games   
            PlayerPrefs.SetString("playername", inputName.text);

            //Disable UI and Create Room
            cancelButton.interactable = true;
            playButton.interactable = false;
            CreateRoom(); 
        }
    }

    //creates a network room
    void CreateRoom()
    {
        Debug.Log("Creating a new room...");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        PhotonNetwork.CreateRoom("CGD3102TAG", roomOps);
    }

    void JoinRoom()
    {
        PhotonNetwork.JoinRoom("CGD3102TAG");
    }

    //if successfully joined room
    public override void OnJoinedRoom()
    {
        //if this player is the host
        Debug.Log("CGD3102TAG Created.");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }

        Debug.Log("Successfully entered a room.");
        SetStatus("");
        lobbyUI.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected.");
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed, there must be an existing room of the same name.");
        JoinRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Canceled.");
        cancelButton.interactable = false;
        playButton.interactable = true;
        PhotonNetwork.LeaveRoom();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void SetStatus(string message)
    {
        statusText.text = message;
    }
}
