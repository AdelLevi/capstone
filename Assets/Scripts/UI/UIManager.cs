using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using _conveyor.Mobile3DFPS.Multiplayer;
using _conveyor.Mobile3DFPS.Player;
using _conveyor.Mobile3DFPS.GameLogic;
using _conveyor.Mobile3DFPS.FileManagement;
using Cinemachine;

namespace _conveyor.Mobile3DFPS.UI
{
	public class UIManager : MonoBehaviour
	{
		public GameController gameController;
		public Text playerNameText;
		public Text timeText;
		public Text winText;
		public Text awardText;
		public Text timeStampText;
		public Text moneyText;
		public Text raceTimeText;
		
		public CanvasGroup inputGroup;
		
		public FixedJoystick cameraJoystick;
		public FixedJoystick movementJoystick;
		
		[Space]
		public GameObject endScreenUI;
		public GameObject mainUI, pauseMenu;
		public GameObject playerListUI;
		
		[Space]
		public GameObject playerUIObject;
		public Transform playersList;

		public int Seconds, Minutes;
		
		private bool ret;
		public NetManager nManager { get; private set; }
		public PlayerMovement playerMovement { get; set; }
		private CinemachineFreeLook cm;
		private bool setupName = false;
		
		public int headIndex { get; private set; }
		public int bodyIndex { get; private set; }
		
		private void Start()
		{
			nManager = FindObjectOfType<NetManager>();
			cm = FindObjectOfType<CinemachineFreeLook>();
			timeText.text = "0:00";
			UpdateUI();
			
			//load
			PlayerData data = FileManager.LoadData();
			
			headIndex = data.HeadIndex;
			bodyIndex = data.BodyIndex;
			
			//don't change currency since it has auto loaded from title screen by 'DontDestroyOnLoad()' method
		}
		
		private void Update()
		{
			if(NetworkClient.ready && !setupName)
			{
				//setup the name of the player onto UI
				if(NetworkClient.isHostClient)
					playerNameText.text = nManager.LocalPlayerName + " (HOST)";
				else
					playerNameText.text = nManager.LocalPlayerName;
					
				setupName = true;
			}
			
			if(gameController.hasWon_Lost)
			{
				if(ret)
					StopAllCoroutines();
					
				return;
			}
			
			if(Input.GetKeyDown(KeyCode.Escape)) {
				pauseMenu.SetActive(!pauseMenu.activeSelf);
			}
			
			if(!ret && gameController.raceBegun) {
				StartCoroutine(TimeLoop());
			}
			
			//move the camera according to joystick movement!
			if(cm!=null)
			{
				cm.m_XAxis.Value = cameraJoystick.Horizontal;
				cm.m_YAxis.Value = cameraJoystick.Vertical;
			}
		}
		
		private IEnumerator TimeLoop()
		{
			ret = true;
			yield return new WaitForSeconds(1.0f);
			Seconds++;
			
			if(Seconds >= 10)
			{
				Minutes++;
				Seconds = 0;
			}
			
			timeText.text = Minutes.ToString("00")+":"+Seconds.ToString("00");
			
			ret = false;
		}
		
		public void Sprint(bool way)
		{
			playerMovement.isSprinting = way;
		}
		
		public void Jump()
		{
			playerMovement.Jump();
		}
		
		public void UpdateUI()
		{
			moneyText.text = "Cash: "+nManager.money;
		}
		
		// Sent to all game objects before the application is quit.
		private void OnApplicationQuit() {
			PlayerData data = new PlayerData()
			{
				HeadIndex = headIndex, BodyIndex = bodyIndex, Currency = nManager.money
			};
			
			FileManager.SaveData(data);
		}
		
		public void Disconnect()
		{
			//save player data
			PlayerData data = new PlayerData()
			{
				BodyIndex = bodyIndex, HeadIndex = headIndex, Currency = nManager.money
			};
			
			
			if(NetworkClient.isHostClient) {
				nManager.StopServer();
			}
			
			if(NetworkClient.isConnected)
				NetworkClient.Disconnect();
		}
		
		public void Resume()
		{
			pauseMenu.SetActive(false);
		}
	}
}








