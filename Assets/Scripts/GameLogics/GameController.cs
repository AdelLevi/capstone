using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using _conveyor.Mobile3DFPS.Player;
using _conveyor.Mobile3DFPS.Multiplayer;
using _conveyor.Mobile3DFPS.UI;

namespace _conveyor.Mobile3DFPS.GameLogic
{
	public class GameController : NetworkBehaviour
	{
		[SyncVar] public uint hostId;
		[SyncVar] public int seconds, minutes;
		[SyncVar] public bool raceBegun;
		[SyncVar] public int raceTimeLeft = 60;
		
		[Space]
		public Camera endCam;
		public readonly SyncList<string> playerNames = new SyncList<string>();
		
		private UIManager ui;
		public bool hasWon_Lost { get; private set; }
		public GameObject[] players { get; private set; }
		private Camera camMain;
		private NetManager nm;
		private bool hasUpdatedUI = false;
		private bool hasDoneTimeLoop = false;
		
		public NetPlayer localPlayer { get; private set; }
		
		public PlayerMovement movement
		{
			get { return localPlayer.GetComponent<PlayerMovement>(); }
		}
		
		private void Start()
		{
			hasWon_Lost = false;
			
			if(NetworkClient.isHostClient)
				raceBegun = false;
			
			camMain = Camera.main;
			nm = FindObjectOfType<NetManager>();
			ui = FindObjectOfType<UIManager>();
			
			FindObjectOfType<FinishLine>().gController = this;
			
			//let NetManager know game has begun!
			nm.Setup(true);
			
			//let's find the local player for this machine!
			NetworkIdentity[] ids = FindObjectsOfType<NetworkIdentity>();
			
			for (int i = 0; i < ids.Length; i++) {
				if(ids[i].isLocalPlayer)
				{
					localPlayer = ids[i].GetComponent<NetPlayer>();
					break;
				}
			}
		}
		
		private void Update()
		{
			//while race hasn't begun update the player list count!
			if(!raceBegun)
			{
				if(NetworkClient.isHostClient) //check if 20 players are reached
				{
					if(nm.numPlayers>=20)
					{
						if(!hasUpdatedUI)
						{
							//destroy all list objects to lessen load on CPU
							foreach (Transform T in ui.playersList)
							{
								Destroy(T.gameObject);
							}
						
							ui.playerListUI.SetActive(false);
							hasUpdatedUI = true;
							raceBegun = true;
						}
					}
				}
				
				//just update players from list!
				if(ui.playersList.childCount != playerNames.Count)
				{
					foreach (Transform T in ui.playersList)
					{
						Destroy(T.gameObject);
					}
					
					for (int i = 0; i < playerNames.Count; i++) {
						Text text = Instantiate(ui.playerUIObject, ui.playersList).GetComponent<Text>();
						text.text = playerNames[i];
					}
				}
			}
			
			if(NetworkClient.isHostClient)
			{
				if(raceBegun)
				{
					seconds = ui.Seconds;
					minutes = ui.Minutes;
					
					if(!hasUpdatedUI)
					{
						//destroy all list objects to lessen load on CPU
						foreach (Transform T in ui.playersList)
						{
							Destroy(T.gameObject);
						}
						
						ui.playerListUI.SetActive(false);
						hasUpdatedUI = true;
					}
				}
				else
				{
					if(!hasDoneTimeLoop) //update time left before race begins!
					{
						StartCoroutine(RaceCountdown());
					}
				}
			}
			else //update from host only!
			{
				if(raceBegun)
				{
					ui.Seconds = seconds;
					ui.Minutes = minutes;
					
					if(!hasUpdatedUI)
					{
						//destroy all list objects to lessen load on CPU
						foreach (Transform T in ui.playersList)
						{
							Destroy(T.gameObject);
						}
						
						ui.playerListUI.SetActive(false);
						hasUpdatedUI = true;
					}
				}
				else
				{
					ui.raceTimeText.text = "TIME BEFORE RACE BEGINS: " + raceTimeLeft + " sec";
				}
			}
		}
		
		private IEnumerator RaceCountdown()
		{
			hasDoneTimeLoop = true;
			yield return new WaitForSeconds(1.0f);
			raceTimeLeft--;
			ui.raceTimeText.text = "TIME BEFORE RACE BEGINS: " + raceTimeLeft + " sec";
			
			if(raceTimeLeft<=0) raceBegun = true;
			
			hasDoneTimeLoop = false;
		}
		
		public void GameWon(NetworkIdentity id) //won by the local player
		{
			string Name = id.GetComponent<NetPlayer>().sharedName;
			
			players = GameObject.FindGameObjectsWithTag("Player");
				
			endCam.gameObject.SetActive(true);
			camMain.gameObject.SetActive(false);
				
			//show an ending screen!
			ui.endScreenUI.SetActive(true);
			ui.mainUI.SetActive(false);
			ui.inputGroup.alpha = 0.0f;
				
			ui.timeStampText.text = "Race Completed in "+ ui.timeText.text + "s";
				
			if(id.isLocalPlayer) {
				ui.winText.text = "Race Completed by " + nm.LocalPlayerName +"!";
			}
			else {
				ui.winText.text = "Race Completed by " + Name +"!";
			}
				
			//give player points
			ui.nManager.money += 5;
			ui.awardText.text = "You are awarded " + 5+"!"; //e.g. You are awarded 50!
			ui.UpdateUI();
			
			hasWon_Lost = true;
			
			//disable all players
			for (int i = 0; i < players.Length; i++) {
				players[i].SetActive(false);
			}
			
			Debug.Log("Game ended!");
		}
		
		[ClientRpc]
		public void Rpc_LoadTitleScreen()
		{
			FindObjectOfType<SceneAsyncLoader>().LoadLevel("TitleScreen");
		}
	}
}









