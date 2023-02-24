using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using _conveyor.Mobile3DFPS.Multiplayer;
using _conveyor.Mobile3DFPS.FileManagement;
using _conveyor.Mobile3DFPS.Customization;

namespace _conveyor.Mobile3DFPS.UI
{
	public class TitleScreenManager : MonoBehaviour
	{
		public GameObject playUI, mainUI, settingUI, customUI;
		public Slider volumeSlider;
		public AudioMixer mixer;
		public Text currencyText;
		public Text statusText;
		public Cosmetic[] cosmetics;
		
		private NetManager nManager;
		private CharacterCustomizer customizer;
		
		private int head, body;

		private void Start()
		{
			nManager = FindObjectOfType<NetManager>();
			customizer = FindObjectOfType<CharacterCustomizer>();
			cosmetics = FindObjectsOfType<Cosmetic>();
			
			//let NetManager know title menu is active
			if(nManager!=null)
			{
				nManager.Setup(false);
			}
		
			PlayerData data = FileManager.LoadData();
			
			if(nManager.money <= 0)
				nManager.money = data.Currency;
				
			customizer.bodyIndex = data.BodyIndex;
			customizer.headIndex = data.HeadIndex;
			customizer.UpdateCharacter(true);
		
			//load the last set volume:
			mixer.SetFloat("volume", PlayerPrefs.GetFloat("master_vol", 0.0f));
			volumeSlider.value = PlayerPrefs.GetFloat("master_vol", 0.0f);
			
			Back();
			UpdateUI();
		}
		
		private void Update()
		{
			//a small infinate money thingy for editor only
			if(Input.GetKeyDown(KeyCode.K) && Application.platform == RuntimePlatform.WindowsEditor)
			{
				nManager.money+=20;
				UpdateUI();
			}
		}

		//UI BUTTONS LOGIC:
	
		public void Play()
		{
			playUI.SetActive(true);
			mainUI.SetActive(false);
		}
	
		public void Settings()
		{
			settingUI.SetActive(true);
			mainUI.SetActive(false);
		}
	
		public void CharacterCustomize()
		{
			UpdateUI();
			
			customUI.SetActive(true);
			mainUI.SetActive(false);
			
			head = customizer.headIndex;
			body = customizer.bodyIndex;
			
			customizer.UpdateUITexts();
		}
	
		public void Back()
		{
			mainUI.SetActive(true);
			playUI.SetActive(false);
			settingUI.SetActive(false);
		}
		
		public void BackCustomization()
		{
			mainUI.SetActive(true);
			customUI.SetActive(false);
			
			//return the previous character outfit
			customizer.headIndex = head;
			customizer.bodyIndex = body;
			customizer.UpdateCharacter();
			
			customizer.UpdateUITexts();
			
			UpdateUI();
			head = -1;
			body = -1;
		}
		
		public void ConfirmCustomization()
		{
			mainUI.SetActive(true);
			customUI.SetActive(false);
			
			//take money
			if(nManager.money >= customizer.totalCost) {
				nManager.money -= customizer.totalCost;
				
				for (int i = 0; i < cosmetics.Length; i++) {
					if(cosmetics[i].gameObject.activeSelf) //if it's active it's brought!
					{
						cosmetics[i].hasBrought = true;
					}
					else
					{
						cosmetics[i].hasBrought = false;
					}
				}
			}
			else
			{
				//return the previous character outfit
				customizer.headIndex = head;
				customizer.bodyIndex = body;
				customizer.UpdateCharacter();
			}
			
			customizer.UpdateUITexts();
			
			UpdateUI();
			head = -1;
			body = -1;
		}
	
		public void SetHostAddress(string text)
		{
			nManager.networkAddress = text;
		}
		
		public void SetPlayerName(string Name)
		{
			nManager.LocalPlayerName = Name;
		}
		
		public void UpdateUI()
		{
			currencyText.text = "Currrency: "+nManager.money;
		}
	
		//SETTINGS MENU LOGIC:
	
		public void SetVoloume(float value)
		{
			mixer.SetFloat("volume", value);
			PlayerPrefs.SetFloat("master_vol", value);
		}
		
		// Sent to all game objects before the application is quit.
		private void OnApplicationQuit() {
			if(customUI.activeSelf) //if player tries to save unbrought stuff
			{
				customizer.headIndex = head;
				customizer.bodyIndex = body;
			}
			
			PlayerData data = new PlayerData()
			{
				HeadIndex = customizer.headIndex,
				BodyIndex = customizer.bodyIndex,
				Currency = nManager.money
			};
			
			FileManager.SaveData(data);
		}
	}
}












