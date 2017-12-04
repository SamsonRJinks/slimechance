using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Dialogue
{
    //ID, speaker name, and dialogue line for a given block
    public string myID;
    public string myName;
    public string myDialogue;

    //constructor for Dialogue class
    public Dialogue(string myID_, string myName_, string myDialogue_)
    {
        myID = myID_;
        myName = myName_;
        myDialogue = myDialogue_;
    }
}

public class StreamReader : MonoBehaviour
{
    public string textFileName;

    //Basic textfile the reader will be using
    private TextAsset myTextFile;

    //Dictionary to be used by text blocks to access specific lines in game
    private Dictionary<string, Dialogue> dialogueDictionary = new Dictionary<string, Dialogue>();

    void Awake()
    {
        myTextFile = Resources.Load(textFileName) as TextAsset;

        //On awake, add all to dictionary
        FillDictionary();
    }

    public void NewTextFile(string newFileName_)
    {
        //Add new text file as the base for the dictionary
        textFileName = newFileName_;
        myTextFile = Resources.Load(textFileName) as TextAsset;

        //Clear and refill dictionary with new text asset
        dialogueDictionary.Clear();
        FillDictionary();
    }

    private void FillDictionary()
    {
        //temporary ID, speaker name, and dialogue line to be passed into dialogue constructor
        string lineID = "";
        string currentSpeaker = "";
        string currentLine = "";

        for(int i = 0; i < myTextFile.text.Length; i++)
        {
            //check for each line of text, extrapolate data from each
            if(lineID == "" && myTextFile.text[i] == '#')
            {
                //Collect textID to be used by dictionary to find data later
                i += 1;

                //Move through textID, adding each char to temporary ID
                while(myTextFile.text[i] != '#')
                {
                    lineID += myTextFile.text[i];
                    i++;
                }

                //Jump to next line (two chars are '' and '\n' respectively
                i += 2;

                print("ID: " + lineID);
            }
            else if(currentSpeaker == "")
            {
                //Move through name line, collect name of character speaking
                while(myTextFile.text[i] != '\n')
                {
                    currentSpeaker += myTextFile.text[i];
                    i++;
                }

                print("Name: " + currentSpeaker);
            }
            else if(currentLine == "")
            {
                //Move through dialogue line, collect dialogue to be spoken
                while (myTextFile.text[i] != '\n')
                {
                    currentLine += myTextFile.text[i];
                    i++;
                }

                print("Dialogue: " + currentLine);
            }
            else
            {
                //apply all dialogue to new Dialogue, then add it to Dictionary
                Dialogue newDia = new Dialogue(lineID, currentSpeaker, currentLine);

                dialogueDictionary.Add(newDia.myID, newDia);

                //Reset all temporary variables
                lineID = "";
                currentSpeaker = "";
                currentLine = "";

                //Jump ahead to next line or end of statement
                i += 8;

                if(i < myTextFile.text.Length)
                {
                    //if this test doesn't return as a hashtag, check that there are no errant characters
                    print("TEST - This char should be a hashtag : '" + myTextFile.text[i + 1] + "'");
                }
                else
                {
                    print("End of dictionary.");
                }
            }
        }
    }

    public Dialogue GetDialogue(string myKey_)
    {
        return dialogueDictionary[myKey_];
    }
}
