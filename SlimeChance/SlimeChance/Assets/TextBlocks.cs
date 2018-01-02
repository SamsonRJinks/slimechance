using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class MyBlock
{
    //Enums for blocks and speakers
    public enum Speakers { Left, Middle, Right };
    public enum ChoiceNum { Zero, Two, Three, Four };

    //Dialogue ID for this block
    public string blockID;

    //Speaker position and whether this block has a choice
    public Speakers mySpeaker = Speakers.Middle;
    public ChoiceNum myChoices = ChoiceNum.Zero;

    //All character sprites to be used in block
    public List<Sprite> characterSprites = new List<Sprite>();

    //All additional data to be added to choices that relates to their favor
    public List<FavorCheck> favorChecks = new List<FavorCheck>();
}

[System.Serializable]
public class FavorCheck
{
    public enum ChoiceNum { First, Second, Third, Fourth };
    public enum FavorType { Swimming, Literature, Paranormal, Cooking, None };
    public enum FavorLevel { Zero = 0, TwentyFive = 25, Fifty = 50, SeventyFive = 75, OneHundred = 100 };

    //The favor that will be applied to an individual choice
    public ChoiceNum myChoiceNum = ChoiceNum.First;

    //The type of favor necessary for this choice
    public FavorType myType = FavorType.None;

    //The favor amount necessary to cross for this choice to be available
    public FavorLevel myFavorLevel = FavorLevel.Zero;

    //Whether favor change (if any) applies to Slimeantha or Roozelyn
    public bool changeForPlayer = true;

    //Amount by which this choice changes the favor type presented
    public int favorChangeAmount = 0;
}


public class TextBlocks : MonoBehaviour
{
    //Name of the block that is referenced in text assets and playerbehavior
    public string myBlockName;

    //Base text asset that this set of blocks will use
    public string myTextAssetName;

    //all blocks in this single blockset
    public List<MyBlock> myBlocks = new List<MyBlock>();
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {

	}

    public MyBlock GetBlock(int ID_)
    {
        //Return block from ID
        return myBlocks[ID_];
    }

    public int GetSetLength()
    {
        //Return length of blockset
        return myBlocks.Count;
    }
}
