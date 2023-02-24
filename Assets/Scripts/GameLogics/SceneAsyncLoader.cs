using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using _conveyor.Mobile3DFPS.Multiplayer;

namespace _conveyor.Mobile3DFPS.GameLogic
{
	public class SceneAsyncLoader : MonoBehaviour
	{
		public GameObject loadingUI;
		public Slider slider;
		
		public void LoadLevel(string sceneName)
		{
			loadingUI.SetActive(true);
			StartCoroutine(SceneLoadAsync(sceneName));
		}
		
		public IEnumerator SceneLoadAsync(string sceneName)
		{
			AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
			
			while(!ao.isDone)
			{
				float progress = Mathf.Clamp01(ao.progress/0.9f);
				slider.value = progress;
				yield return null;
			}
		}
	}
}








