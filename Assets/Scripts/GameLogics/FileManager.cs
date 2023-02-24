using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _conveyor.Mobile3DFPS.FileManagement
{
	public static class FileManager
	{
		public static void SaveData(PlayerData pData)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			string path = Application.persistentDataPath + "/PlayerData.sav";
			FileStream stream = new FileStream(path, FileMode.Create);
			formatter.Serialize(stream, pData);
			stream.Close();
		}
	
		public static PlayerData LoadData()
		{
			string path = Application.persistentDataPath + "/PlayerData.sav";
			PlayerData data = new PlayerData();
		
			if(File.Exists(path))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream stream = new FileStream(path, FileMode.Open);
				data = formatter.Deserialize(stream) as PlayerData;
				stream.Close();
			}
			else
			{
				Debug.Log("No save file exsists at path " + path);
			}
		
			return data;
		}
	}
}
