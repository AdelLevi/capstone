using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using _conveyor.Mobile3DFPS.GameLogic;
using _conveyor.Mobile3DFPS.UI;

namespace _conveyor.Mobile3DFPS.Multiplayer
{
	public class NetManager : NetworkManager
	{
		public string LocalPlayerName;
		public int money;
		
		private bool isInGame = false;
		UIManager ui;
		GameController gController;
		
		public void RandomizeName()
		{
			LocalPlayerName = "Guest" + Random.Range(1, 9999).ToString("0000");
		}
		
		public void ChangePlayerName(string value)
		{
			LocalPlayerName = value;
		}
		
		public void Setup(bool isGame)
		{
			//fill vars
			ui = FindObjectOfType<UIManager>();
			gController = FindObjectOfType<GameController>();
			
			isInGame = isGame;
			
			if(NetworkClient.isHostClient && isInGame) {
				gController.playerNames.Add(LocalPlayerName + " (HOST)"); //always add the host name first!
			}
		}
		
		public void RefreshList()
		{
			StartCoroutine(UpdatePlayersList());
		}
		
		private IEnumerator UpdatePlayersList()
		{
			yield return new WaitForSeconds(0.5f);
			
			//remove all previous refs first
			gController.playerNames.Clear();
			foreach (Transform T in ui.playersList)
			{
				Destroy(T.gameObject);
			}
			
			//add the host client on top always!
			gController.playerNames.Add(LocalPlayerName + " (HOST)");
			
			NetPlayer[] nplayers = FindObjectsOfType<NetPlayer>();
				
			for (int i = 0; i < nplayers.Length; i++) {
				if(!nplayers[i].isLocalPlayer) //never add host client later
				{
					if(gController.playerNames.Contains(nplayers[i].sharedName) == false) //this player name is not in list
						gController.playerNames.Add(nplayers[i].sharedName);
				}
			}
		}
		
		public override void OnServerDisconnect(NetworkConnectionToClient conn) {
			base.OnServerDisconnect(conn);
			RefreshList();
		}
	}
}







