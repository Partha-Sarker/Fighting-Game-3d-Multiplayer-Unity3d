using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetManager : NetworkManager
{

	public NetworkDiscovery discovery;

	public override void OnStartHost()
	{
		discovery.Initialize();
		discovery.StartAsServer();

	}

	public override void OnStartClient(NetworkClient client)
	{
		discovery.showGUI = false;
	}

	public override void OnStopClient()
	{
		discovery.StopBroadcast();
		discovery.showGUI = true;
	}
}
