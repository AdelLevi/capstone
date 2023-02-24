using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _conveyor.Mobile3DFPS.Multiplayer;
using _conveyor.Mobile3DFPS.FileManagement;

namespace _conveyor.Mobile3DFPS.Customization
{
	//make the first part of array a null, as this is used nowhere!
	public class CharacterCustomizer : MonoBehaviour
	{
		public GameObject[] headOutfits;
		public GameObject[] bodyOutfits;
		public Text costText;
	
		public int bodyIndex { get; set; }
		public int headIndex { get; set; }
		private NetManager nManager;
		private Color originalColor;
		public List<Cosmetic> enabledCosmetics = new List<Cosmetic>();
		
		public int totalCost
		{
			get
			{
				if(enabledCosmetics.Count > 0)
				{
					int total = 0;
					for (int i = 0; i < enabledCosmetics.Count; i++) 
					{
						if(enabledCosmetics[i].hasBrought == false) 
						{
							total += enabledCosmetics[i].Cost;
						}
					}
					return total;
				}
				else
				{
					return 0;
				}
			}
		}
		
		private void Start()
		{
			nManager = FindObjectOfType<NetManager>();
			originalColor = costText.color;
		}
		
		public void UpdateUITexts()
		{
			costText.text = "Cost: " + totalCost;
			
			//cost higher, change color to red, elsewise let it be white
			if(nManager!=null)
				costText.color = totalCost > nManager.money ? Color.red : originalColor;	
		}
	
		public void UpdateCharacter(bool isStart = false)
		{
			//BODY
			if(bodyIndex == 0) //wear no outfits
			{
				for (int i = 0; i < bodyOutfits.Length; i++) {
					if(bodyOutfits[i]!=null) { 
						bodyOutfits[i].SetActive(false);
						bodyOutfits[i].GetComponent<Cosmetic>().Remove();
					}
				}
			}
			else
			{
				//disable all outfits first
				for (int i = 0; i < bodyOutfits.Length; i++) {
					if(bodyOutfits[i]!=null) {
						bodyOutfits[i].SetActive(false);
						bodyOutfits[i].GetComponent<Cosmetic>().Remove();
					}
				}
				
				//enable the new outfit now!
				bodyOutfits[bodyIndex-1].SetActive(true);
				bodyOutfits[bodyIndex-1].GetComponent<Cosmetic>().Add();
				
				if(isStart)
					bodyOutfits[bodyIndex-1].GetComponent<Cosmetic>().hasBrought = true;
			}
		
			//HEAD
			if(headIndex == 0) //wear no outfits
			{
				for (int i = 0; i < headOutfits.Length; i++) {
					if(headOutfits[i]!=null) {
						headOutfits[i].SetActive(false);
						headOutfits[i].GetComponent<Cosmetic>().Remove();
					}
				}
			}
			else
			{
				//disable all outfits first
				for (int i = 0; i < headOutfits.Length; i++) {
					if(headOutfits[i]!=null) {
						headOutfits[i].SetActive(false);
						headOutfits[i].GetComponent<Cosmetic>().Remove();
					}
				}
				
				//enable the new outfit now!
				headOutfits[headIndex-1].SetActive(true);
				headOutfits[headIndex-1].GetComponent<Cosmetic>().Add();
				
				if(isStart) {
					//Debug.Log(headOutfits[headIndex-1].name);
					headOutfits[headIndex-1].GetComponent<Cosmetic>().hasBrought = true;
				}
			}
			
			UpdateUITexts();
		}
	
		public void Increase(bool isBody)
		{
			if(isBody)
			{
				bodyIndex++;
			
				if(bodyIndex>bodyOutfits.Length)
				{
					bodyIndex = 0;
				}
			
				UpdateCharacter();
			}
			else
			{
				headIndex++;
			
				if(headIndex>headOutfits.Length)
				{
					headIndex = 0;
				}
			
				UpdateCharacter();
			}
		}
	
		public void Decrease(bool isBody)
		{
			if(isBody)
			{
				bodyIndex--;
			
				if(bodyIndex<0)
				{
					bodyIndex = bodyOutfits.Length;
				}
			
				UpdateCharacter();
			}
			else
			{
				headIndex--;
			
				if(headIndex<0)
				{
					headIndex = headOutfits.Length;
				}
			
				UpdateCharacter();
			}
		}
		
		public void SaveSettings()
		{
			PlayerData data = new PlayerData()
			{
				HeadIndex = headIndex, BodyIndex = bodyIndex, Currency = nManager.money
			};
			
			FileManager.SaveData(data);
		}
	}
}










