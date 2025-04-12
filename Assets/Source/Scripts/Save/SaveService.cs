using UnityEngine;

public class SaveService: MonoBehaviour
{
     private const string Key = "Save";

     public SaveData SaveData = new();
     
     public void Save()
     {
          PlayerPrefs.SetString(Key, JsonUtility.ToJson(SaveData));
          PlayerPrefs.Save();
     }

     public void Load()
     {
          if (PlayerPrefs.HasKey(Key)) 
               SaveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key));
     }
}