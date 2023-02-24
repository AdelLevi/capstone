using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace _conveyor.Mobile3DFPS.GameLogic
{
	public class FinishLine : MonoBehaviour
	{
		public GameController gController { get; set; }
		
		private void OnTriggerEnter(Collider other) 
		{
			if(other.tag == "Player")
			{
				gController.GameWon(other.GetComponent<NetworkIdentity>());
				Debug.Log(other.name + " Triggered!");
			}
		}
	}

}