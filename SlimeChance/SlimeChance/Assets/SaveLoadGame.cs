using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadGame : MonoBehaviour
{
    private string myPath;

	// Use this for initialization
	void Start ()
    {
        myPath = Application.persistentDataPath + "/Save_File.gd";
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		
	}

    public void SaveGame(string name_, string block_, int id_, float[] playerFaves_, float[] enemyFaves_)
    {
        GameData.currentFile = new GameData(name_, block_, id_, playerFaves_, enemyFaves_);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(myPath);
        bf.Serialize(file, GameData.currentFile);
        file.Close();
    }

    public bool LoadGame()
    {
        if (File.Exists(myPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(myPath, FileMode.Open);
            GameData.currentFile = ((GameData)bf.Deserialize(file));
            file.Close();

            return true;
        }

        return false;
    }

    public void DeleteGame()
    {
        if (File.Exists(myPath))
        {
            File.Delete(myPath);
        }
    }
}
