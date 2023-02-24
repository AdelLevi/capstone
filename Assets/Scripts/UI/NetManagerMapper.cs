using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _conveyor.Mobile3DFPS.Multiplayer;

namespace _conveyor.Mobile3DFPS.UI
{
	public class NetManagerMapper : MonoBehaviour
	{
		public string[] GameLevels;
		public InputField field;
		
		private NetManager nManager;
		
		private void Start()
		{
			nManager = FindObjectOfType<NetManager>();
		}
		
		public void Host()
		{
			//randomize the levels!
			nManager.onlineScene = GameLevels[Random.Range(0, GameLevels.Length)];
			
			if(nManager!=null)
			{
				nManager.RandomizeName();
				nManager.StartHost();
			}
		}
		
		public void Join()
		{
			if(nManager!=null)
			{
				nManager.RandomizeName();
				nManager.StartClient();
			}
		}
		
		public void SetAddress(string newAddess)
		{
			if(newAddess == "") { 
				field.text = "localhost";
				nManager.networkAddress = "localhost";
				return;
			}
			
			nManager.networkAddress = newAddess;
		}
	}
}







