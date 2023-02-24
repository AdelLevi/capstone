using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using _conveyor.Mobile3DFPS.UI;

public class PlayServicesSetupManager : MonoBehaviour
{
	private TitleScreenManager title;
	
	private void Start()
	{
		title = FindObjectOfType<TitleScreenManager>();
		PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
	}
	
	internal void ProcessAuthentication(SignInStatus status)
	{
		if(status == SignInStatus.Success)
		{
			title.statusText.text = "Signed-In Sucessfully!";
		}
		
		if(status == SignInStatus.Canceled)
		{
			title.statusText.text = "Sign-In Cancelled, Not Signed-In!";
		}
		
		if(status == SignInStatus.InternalError)
		{
			title.statusText.text = "Internal Error During Sign-In, Not Signed-In!";
		}
	}
}
