using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public static GameData currentFile;

    public string myLevelName;
    public string myBlockName;

    public int myBlockID;

    public int myMonth;
    public int myDay;

    public int myHour;
    public int myMinute;

    //Levels of favor throughout game, in order: Swimming, Literature, Paranormal, Cooking
    public float[] playerFaveLevels = new float[4];
    public float[] enemyFaveLevels = new float[4];

    //Call this with new when making a new game
    public GameData(string levelName_, string blockName_, int blockID_, float[] playerFaves_, float[] enemyFaves_)
    {
        myLevelName = levelName_;
        myBlockName = blockName_;

        myBlockID = blockID_;

        for(int i = 0; i < 4; i++)
        {
            playerFaveLevels[i] = playerFaves_[i];
            enemyFaveLevels[i] = enemyFaves_[i];
        }
        
        myMonth = System.DateTime.Now.Month;
        myDay = System.DateTime.Now.Day;

        myHour = System.DateTime.Now.Hour;
        myMinute = System.DateTime.Now.Minute;
    }
}
