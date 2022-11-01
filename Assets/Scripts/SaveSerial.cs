using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSerial : MonoBehaviour
{


public void SaveLocalData(LocalData localData)
{
    BinaryFormatter bf = new BinaryFormatter(); 
    FileStream file = File.Create(Application.persistentDataPath 
        + "/MySaveData.dat"); 
    SaveData data = new SaveData();
    data.PathURL = localData.PathURL;
    bf.Serialize(file, data);
    file.Close();
    Debug.Log("Game data saved!");
}

public bool LoadLocalData(out LocalData localData)
{

  if (File.Exists(Application.persistentDataPath 
    + "/MySaveData.dat"))
  {
    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = 
      File.Open(Application.persistentDataPath 
      + "/MySaveData.dat", FileMode.Open);
    SaveData data = (SaveData)bf.Deserialize(file);
    file.Close();
    localData = new LocalData(data.PathURL);
    Debug.Log("Game data loaded!");
    
    return true;    
  }
  else{
    Debug.Log("There is no save data!");
    localData = new LocalData();
    return false;
  }
}

}



[Serializable]
public class SaveData{
    public string PathURL{get; set;}
}

public class LocalData{

    public string PathURL{get; set;}

    public LocalData(){}

    public LocalData(string pathURL){
        PathURL = pathURL;
    }
}
