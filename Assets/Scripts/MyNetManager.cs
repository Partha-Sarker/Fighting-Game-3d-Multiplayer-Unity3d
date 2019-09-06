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
    public GameObject WaitingText;
    public string state;

    public override void OnStartHost()
	{
		discovery.Initialize();
		discovery.StartAsServer();

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
        state = "Hosting";
        StartHost();
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
        HostButton.SetActive(false);
        JoinButton.SetActive(false);
        EnterButton.SetActive(false);
        WaitingText.SetActive(false);
    }

    public void ResetUI()
    {
        HostButton.SetActive(true);
        JoinButton.SetActive(true);
        CancelButton.SetActive(false);
        EnterButton.SetActive(false);
        WaitingText.SetActive(false);
    }

    public void Cancel()
    {
        if(state == "Joining")
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
