using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using _conveyor.Mobile3DFPS.UI;

namespace _conveyor.Mobile3DFPS.Multiplayer
{
	public class NetPlayer : NetworkBehaviour
	{
		public Behaviour[] toDiable;
		public Material otherMat;
		
		[Space]
		public GameObject[] headObjects;
		public GameObject[] bodyObjects;
		
		[SyncVar]
		public string sharedName;
		
		private NetworkIdentity id;
		private CinemachineFreeLook camController;
		private NetManager nm;
		private UIManager ui;
		
		private void Start()
		{
			ui = FindObjectOfType<UIManager>();
			nm = FindObjectOfType<NetManager>();
			id = GetComponent<NetworkIdentity>();
			camController = FindObjectOfType<CinemachineFreeLook>();
			
			if(!id.isLocalPlayer)
			{
				//change the color
				GetComponent<MeshRenderer>().material = otherMat;
				GetComponent<CapsuleCollider>().radius = 1.3f;
				
				for (int i = 0; i < toDiable.Length; i++) {
					toDiable[i].enabled = false;
				}
			}
			else
			{
				camController.m_LookAt = transform;
				camController.m_Follow = transform;
				
				StartCoroutine(DelayedWearCosmetics());
				
				//share the local name!
				if(NetworkClient.isHostClient)
				{
					sharedName = nm.LocalPlayerName;
					transform.name = nm.LocalPlayerName + " (Player)";
				}
				else if (hasAuthority)
					Cmd_ShareName(nm.LocalPlayerName);
			}
		}
		
		[Command]
		public void Cmd_ShareName(string n)
		{
			sharedName = n;
			transform.name = n + " (Player)";
			nm.RefreshList();
		}
		
		private IEnumerator DelayedWearCosmetics()
		{
			yield return new WaitForSeconds(0.2f);
			
			for (int n = 0; n < bodyObjects.Length; n++)
				bodyObjects[n].SetActive(false);
			
			for (int i = 0; i < headObjects.Length; i++)
				headObjects[i].SetActive(false);
			
			//wear the cosmetics!
			if(ui.headIndex > 0) {
				headObjects[ui.headIndex-1].SetActive(true);
			}
			
			if(ui.bodyIndex > 0) {
				bodyObjects[ui.bodyIndex-1].SetActive(true);	
			}
		}
	}
}








