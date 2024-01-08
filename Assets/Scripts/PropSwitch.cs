using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PropSwitch : MonoBehaviourPunCallbacks
{
    public GameManager gameManager;

    public GameObject player;
    public GameObject prop_One;
    public GameObject prop_Two;
    public GameObject prop_Three;

    void Awake()
    {
        // finds the local game manager
        gameManager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.RightShift) && !PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("NextProp", RpcTarget.AllBuffered);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("PrevProp", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void NextProp()
    {
        if (player.activeSelf == true || prop_Three.activeSelf == true)
        {
            player.SetActive(false);
            prop_Three.SetActive(false);
            prop_One.SetActive(true);

            gameManager.activeProp.text = "Barrel";
            Debug.Log("One");
        }
        else if (prop_One.activeSelf == true)
        {
            prop_One.SetActive(false);
            prop_Two.SetActive(true);

            gameManager.activeProp.text = "Cart";
            Debug.Log("Two");
        }
        else if (prop_Two.activeSelf == true)
        {
            prop_Two.SetActive(false);
            prop_Three.SetActive(true);

            gameManager.activeProp.text = "Garbage Bin";
            Debug.Log("Three");
        }
    }

    [PunRPC]
    void PrevProp()
    {
        if (player.activeSelf == true || prop_One.activeSelf == true)
        {
            player.SetActive(false);
            prop_Three.SetActive(true);
            prop_One.SetActive(false);

            gameManager.activeProp.text = "Garbage Bin";
            Debug.Log("Three");
        }
        else if (prop_Two.activeSelf == true)
        {
            prop_One.SetActive(true);
            prop_Two.SetActive(false);

            gameManager.activeProp.text = "Barrel";
            Debug.Log("Two");
        }
        else if (prop_Three.activeSelf == true)
        {
            prop_Two.SetActive(true);
            prop_Three.SetActive(false);

            gameManager.activeProp.text = "Cart";
            Debug.Log("One");
        }
    }
}
