using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerBehavior : MonoBehaviour
{
    //use current blockset and id to move through each text block
    private TextBlocks myBlockSet;
    private MyBlock currentBlock;
    private int currentBlockID = -1;

    //Total length of a given blockset
    private int setLength = 0;

    private bool firstLoad = true;

    //Current level the game will go to on the last block
    private string nextLevel = "Start";

    //the text for display and name
    public Text displayText;
    public Text nameText;

    //Text blocks parent, all my blocksets derived from this
    public Transform textBlocksParent;

    //Opening block name
    public string startingBlocksName;

    //lists of all textblocksets and characterdisplays available to the scene
    private List<TextBlocks> allMyBlocksets = new List<TextBlocks>();
    private List<CharacterDisplay> allMyCharacters = new List<CharacterDisplay>();

    //dictionary and reader to display dialogue
    private StreamReader allMyDialogue;

    //the base choice selection parent
    private ChoicesBehavior allMyChoices;

    //the base class for saving and loading a player's save file
    private SaveLoadGame mySaveLoad;

    //the favor managers containing both slimeantha and roozelyn's data respectively
    //Each amount of favor, set as, in order: 0 = Swimming, 1 = Literature, 2 = Paranormal, 3 = Cooking
    private FavorManager playerFavMan;
    private FavorManager enemyFavMan;

    //stored coroutine of letter display
    private Coroutine currentCor = null;

    //Begin before all the others
    void Awake()
    {
        //Get the transform for all the characters to display on screen
        Transform tempChar = transform.Find("AllCharacters");

        //Add all textblocks to array
        for (int i = 0; i < textBlocksParent.childCount; i++)
        {
            allMyBlocksets.Add(textBlocksParent.GetChild(i).GetComponent<TextBlocks>());
        }

        //Add all characterdisplays to array
        for (int i = 0; i < tempChar.childCount; i++)
        {
            allMyCharacters.Add(tempChar.GetChild(i).GetComponent<CharacterDisplay>());
        }

        //Find and make use of the choices screen
        allMyChoices = transform.Find("ChoiceScreen").GetComponent<ChoicesBehavior>();

        //Find and load in the dialogue reader and save/load game
        allMyDialogue = GetComponent<StreamReader>();
        mySaveLoad = GetComponent<SaveLoadGame>();

        //Find and load in the favor managers for slimeantha and roozelyn
        playerFavMan = transform.Find("Slimeantha_Circle").GetComponent<FavorManager>();
        enemyFavMan = transform.Find("Roozelyn_Circle").GetComponent<FavorManager>();

        //Load in the current game if possible
        if (mySaveLoad.LoadGame())
        {
            startingBlocksName = GameData.currentFile.myBlockName;
        }

        //Apply appropriate favor percentages to each
        if (File.Exists(Application.persistentDataPath + "/Save_File.gd"))
        {
            //Applies previously saved favor if available
            playerFavMan.WedgePercentages = GameData.currentFile.playerFaveLevels;
            enemyFavMan.WedgePercentages = GameData.currentFile.enemyFaveLevels;
        }
        else
        {
            //Otherwise, applies new values to each (roozelyn starting better)
            for (int i = 0; i < 4; i++)
            {
                playerFavMan.WedgePercentages[i] = 0.0f;
                enemyFavMan.WedgePercentages[i] = Random.Range(25.0f, 50.0f);
            }
        }
    }

	//After Awake and everyone else getting situated, actually start the game
	void Start ()
    {
        //Start up the game with the first block you plan on going to
        GoToNewBlockSet(startingBlocksName);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {

	}

    public void GoToNewBlockSet(string newSet_)
    {
        //get blocks from set, reset set length and block ID to next
        for(int i = 0; i < allMyBlocksets.Count; i++)
        {
            if(allMyBlocksets[i].myBlockName == newSet_)
            {
                myBlockSet = allMyBlocksets[i];
                print("Found Block Set: " + newSet_);
                break;
            }
            else if(i == allMyBlocksets.Count - 1)
            {
                print("!!!NO NEW BLOCK SET FOUND WITH '" + newSet_ + "', PLEASE CHECK NAME OF BLOCK SET!!!");
            }
        }

        //Check if you've loaded earlier in this session
        if (firstLoad && File.Exists(Application.persistentDataPath + "/Save_File.gd"))
        {
            firstLoad = false;
            currentBlockID = GameData.currentFile.myBlockID - 1;
        }
        else
        {
            currentBlockID = -1;
        }

        setLength = myBlockSet.GetSetLength();

        NextBlock();
    }

    private FavorContainer PlayerFavMan_ReturnFavorValue(int TargetWedge, FavorContainer FavorValueHolder, bool SliOrRoo)
    {
        throw new System.NotImplementedException();
    }

    public void NextBlock()
    {
        //go to next textblock in set
        currentBlockID++;

        //check if blockset is finished
        if (currentBlockID < setLength)
        {
            //set current block for easy access later
            currentBlock = myBlockSet.GetBlock(currentBlockID);

            //Change the text on screen with current block
            ChangeScreenText(currentBlock.blockID);

            //Change the currently speaking and active characters
            ChangeCharacters();

            //Change the screen by block type
            ChangeByChoiceNum();
        }
        else
        {
            //Got to next set of blocks, or end the scene and go to the next scene
            if(myBlockSet.myBlockName != "EndBlocks")
            {
                GoToNewBlockSet(currentBlock.blockID);
                UpdateGameSave();
            }
            else
            {
                SceneManager.LoadScene(nextLevel);
            }
        }
    }

    public void ChangeNextLevel(string next_)
    {
        //change the level you're going to next (nextLevel set at start as default of 1, MUST BE CHANGED)
        nextLevel = next_;
    }

    public void ChangeScreenText(string textID_)
    {
        //Dialogue and dictionary
        Dialogue tempDia = allMyDialogue.GetDialogue(textID_);

        //stop coroutine that was just sent to skip to the next one
        if (currentCor != null)
        {
            StopCoroutine(currentCor);
        }

        //Start up coroutine to display letters, nameplate to display name
        currentCor = StartCoroutine(LetterByLetter(tempDia.myDialogue));
        nameText.text = tempDia.myName;
    }

    public void ChangeCharacters()
    {
        //If no changes desired, leave CharacterSprites blank, and this all will be skipped
        //For any changes to occur, there needs to be at least as many sprites as on screen (3 for now)
        if (allMyCharacters.Count == currentBlock.characterSprites.Count)
        {
            //Change sprites and speaker/active status for all chars based on sprites given
            for (int i = 0; i < allMyCharacters.Count; i++)
            {
                if (i == (int)currentBlock.mySpeaker)
                {
                    //Change to speaker, set image color to white
                    allMyCharacters[i].ChangeSpeaker(true, true);
                    allMyCharacters[i].ChangeSprite(currentBlock.characterSprites[i]);
                }
                else if (currentBlock.characterSprites[i] != null)
                {
                    //Observer characters, set image color to gray
                    allMyCharacters[i].ChangeSpeaker(false, true);
                    allMyCharacters[i].ChangeSprite(currentBlock.characterSprites[i]);
                }
                else
                {
                    //Turn off characters, set speaker and active status to false
                    allMyCharacters[i].ChangeSpeaker(false, false);
                }
            }
        }
    }

    public void ChangeByChoiceNum()
    {
        if(currentBlock.myChoices != MyBlock.ChoiceNum.Zero)
        {
            //Turn on choice overlay, send A, B, C, or D response via clicked upon triggers
            allMyChoices.ActivateChoices((int)currentBlock.myChoices, currentBlock.blockID, currentBlock, playerFavMan.WedgePercentages);
        }
    }

    public void SendChoiceDescriptionText(int myChoiceNum_)
    {
        ChangeScreenText(currentBlock.blockID + "_Choice_" + myChoiceNum_);
    }

    public void RewriteCurrentBlockText()
    {
        //Rewrite the text on screen, often used when unselecting choices
        ChangeScreenText(currentBlock.blockID);
    }

    public void UpdateGameSave()
    {
        //Send scene name, block name, block ID, and current favor levels to be saved as game data
        mySaveLoad.SaveGame(SceneManager.GetActiveScene().name, myBlockSet.myBlockName, currentBlockID, playerFavMan.WedgePercentages, enemyFavMan.WedgePercentages);
    }

    public void ChangeFavor(int choiceNum_)
    {
        //Check if there are any favor checks to check
        if (currentBlock.favorChecks.Count != 0)
        {
            //then check if that the choice given matches any of them
            for (int i = 0; i < currentBlock.favorChecks.Count; i++)
            {
                //make sure this is the right choice in the favors selected (they can be variable in size) and if it's not a none based choice
                if((int)currentBlock.favorChecks[i].myChoiceNum + 1 == choiceNum_ && currentBlock.favorChecks[i].myType != FavorCheck.FavorType.None)
                {
                    //if so, continue by creating an easy reference to the type that we can use to access the desired favor
                    int typeNum_ = (int)currentBlock.favorChecks[i].myType;
                    float[] favorChanging;

                    if(currentBlock.favorChecks[i].changeForPlayer)
                    {
                        favorChanging = playerFavMan.WedgePercentages;
                    }
                    else
                    {
                        favorChanging = enemyFavMan.WedgePercentages;
                    }

                    //apply change as necessary
                    favorChanging[typeNum_] += currentBlock.favorChecks[i].favorChangeAmount;

                    //adjust changes back to expected levels, so they don't go over or under the expected limits
                    if (favorChanging[typeNum_] > 100)
                    {
                        favorChanging[typeNum_] = 100;
                    }
                    else if (favorChanging[typeNum_] < 0)
                    {
                        favorChanging[typeNum_] = 0;
                    }

                    print("Favor Changes in - Swimming: " + favorChanging[0] + ", Literature: " + favorChanging[1] + ", Paranormal: " + favorChanging[2] + ", Cooking: " + favorChanging[3]);

                    break;
                }
            }
        }
    }

    private IEnumerator LetterByLetter(string myText_)
    {
        //Go through each letter in text, display on screen (looping every frame until FOR loop ends)
        for(int i = 0; i < myText_.Length; i++)
        {
            if(i == 0)
            {
                displayText.text = "";
            }

            displayText.text += myText_[i];

            yield return new WaitForEndOfFrame();
        }

        //Upon completing the coroutine, mark that the current Coroutine reference as null
        currentCor = null;
    }
}
