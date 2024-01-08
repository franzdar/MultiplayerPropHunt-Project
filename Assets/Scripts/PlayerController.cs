using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private new Camera camera;
    [SerializeField] private TMP_Text nameText;

    private bool isFound;
    [SerializeField] private Color taggedColor;
    private Color unfoundColor;
    private float timeSpentHiding;
    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioClip taggedSFX;

    void Awake()
    {
        // if this is not the local player
        if (!photonView.IsMine)
        {
            Destroy(camera.gameObject);
        }

        // update the player's name display
        UpdatePlayerDisplay();

        // store the default color
        unfoundColor = GetComponentInChildren<MeshRenderer>().material.color;

        // finds the local game manager
        gameManager = FindObjectOfType<GameManager>();
    }

    [PunRPC]
    public void OnTagged()
    {
        Debug.Log("Tagged");

        // flag the player as tagged
        isFound = true;

        // change the color of the player to the IT color
        GetComponentInChildren<MeshRenderer>().material.color = taggedColor;

        // plays audio indicator
        if (photonView.IsMine)
        {
            GetComponent<AudioSource>().PlayOneShot(taggedSFX);
        }
    }

    [PunRPC]
    public void OnUntagged()
    {
        Debug.Log("Untagged.");

        // flag the player as untagged
        isFound = false;

        // Restore the color of the player to the default color
        GetComponentInChildren<MeshRenderer>().material.color = unfoundColor;
    }

    private void Update()
    {
        // update player display
        UpdatePlayerDisplay();

        // only run this for the local player that is tagged
        if (photonView.IsMine)
        {
            // updates player status
            // if player is Found
            if (isFound)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    gameManager.playerStatus.text = "<color=red>You are IT. Find Them!";
                }
                else
                {
                    gameManager.playerStatus.text = "<color=red>You've been FOUNDED";
                    gameManager.playerTagTimer.text = $"<color=red>FOUND in {timeSpentHiding:F1} sec";
                }
            }
            else
            {
                timeSpentHiding += Time.deltaTime;
                gameManager.playerStatus.text = "<color=green>Hide from IT";
            }

            gameManager.playerTagTimer.text = $"{timeSpentHiding:F1} sec";

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.leaveRoom.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if we hit a player
        var otherPlayer = other.GetComponent<Collider>().GetComponent<PlayerController>();
        var otherPlayerController = other.GetComponent<PlayerMovement>();

        if (otherPlayer != null)
        {
            // if we're IT 
            if (isFound)
            {
                // find other player
                otherPlayer.photonView.RPC("OnTagged", RpcTarget.AllBuffered);
                if (!PhotonNetwork.IsMasterClient)
                {
                    otherPlayerController.enabled = false;
                }
            }
        }
    }
    private void UpdatePlayerDisplay()
    {
        // Displays player info if the player is not IT
        if (PhotonNetwork.IsMasterClient)
        {
            nameText.text = "";
        }
        else
        {
            nameText.text = $"{photonView.Owner.NickName}\n<size=50%>{timeSpentHiding:F1} sec</size>";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // write info to the network
        if (stream.IsWriting)
        {
            stream.SendNext(isFound);
            stream.SendNext(timeSpentHiding);
        }

        // read info from the network
        else
        {
            isFound = (bool)stream.ReceiveNext();
            timeSpentHiding = (float)stream.ReceiveNext();

            UpdatePlayerDisplay();
        }
    }
}
