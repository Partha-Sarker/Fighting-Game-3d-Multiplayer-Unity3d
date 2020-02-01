using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class MyNetManager : NetworkManager
{
	public NetworkDiscovery discovery;
    public GameObject HostButton;
    public GameObject JoinButton;
    public GameObject CancelButton;
    public GameObject EnterButton;
    public GameObject rematchButton;
    public GameObject exitButton;
    public GameObject WaitingText;
    public GameObject winText;
    public GameObject loseText;
    public Animator fadeAnimator;
    public string state;

    public override void OnStartHost()
	{
		discovery.Initialize();
		discovery.StartAsServer();

	}

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        print("Client is disconnected");
    }

    public override void OnStartClient(NetworkClient client)
    {
        discovery.showGUI = false;
        HideUI();
	}

    public override void OnStopClient()
    {
        discovery.StopBroadcast();
		discovery.showGUI = true;
        ResetUI();
	}

    public void StartGameHost()
    {
        fadeAnimator.SetTrigger("Fade In");
        StartCoroutine(StartingGameHost());
    }

    IEnumerator StartingGameHost()
    {
        yield return new WaitForSeconds(.75f);
        state = "Hosting";
        try
        {
            StartHost();
        } catch(Exception e)
        {
            print("The port is occupied");
        }
    }

    public void StartJoinRequest()
    {
        state = "Joining";
        discovery.Initialize();
        discovery.StartAsClient();
    }

    public void HideUI()
    {
        state = "Playing";
        CancelButton.SetActive(true);
        exitButton.SetActive(false);
        HostButton.SetActive(false);
        JoinButton.SetActive(false);
        EnterButton.SetActive(false);
        WaitingText.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
    }

    public void ResetUI()
    {
        HostButton.SetActive(true);
        JoinButton.SetActive(true);
        winText.SetActive(true);
        loseText.SetActive(true);
        exitButton.SetActive(true);
        Manager.isRefresed = false;
        CancelButton.SetActive(false);
        EnterButton.SetActive(false);
        WaitingText.SetActive(false);
        rematchButton.SetActive(false);
    }

    public void Cancel()
    {
        fadeAnimator.SetTrigger("Fade In");
        StartCoroutine(Cancelling());
    }

    IEnumerator Cancelling()
    {
        yield return new WaitForSeconds(.75f);
        if (state == "Joining")
        {
            discovery.StopBroadcast();
        }
        else
        {
            StopHost();
        }
        ResetUI();
    }

}
