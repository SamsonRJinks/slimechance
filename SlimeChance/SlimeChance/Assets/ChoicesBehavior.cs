using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicesBehavior : MonoBehaviour
{
    //All choice objects in hierarchy
    private List<IndividualChoice> myChoices = new List<IndividualChoice>();

    //Image that blocks characters from view
    private Image myImage;

    //Image that allows player to advance story
    private Image clickImage;

    //Screen change behavior
    private PlayerBehavior screenPlayer;

    // Use this for initialization
    void Start ()
    {
        //Set up all the components the choices will need
        myImage = GetComponent<Image>();
        clickImage = GameObject.Find("GeneralClick").GetComponent<Image>();
        screenPlayer = transform.parent.GetComponent<PlayerBehavior>();

        //add all individualchoice components to private list
        for(int i = 0; i < transform.childCount; i++)
        {
            myChoices.Add(transform.GetChild(i).GetComponent<IndividualChoice>());
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {

	}

    public void ActivateChoices(int totalChoices_, string currentID_, MyBlock currentBlock_, float[] favorTotals_)
    {
        //turn on screen image to block characters
        myImage.enabled = true;
        clickImage.enabled = false;

        //turn on indidual choices (enum + 1), apply text based on individual modifiers 
        for (int i = 0; i <= totalChoices_; i++)
        {
            bool willActivate = true;
            FavorCheck.FavorType choiceType = FavorCheck.FavorType.None;

            //check each favor possible to see if any matches this choice
            for (int j = 0; j < currentBlock_.favorChecks.Count; j++)
            {
                //Get the choice number that the current favor check is referring to (First, Second, Third, Fourth)
                int choiceNum_ = (int)currentBlock_.favorChecks[j].myChoiceNum;
                //Get the favor level of the current favor check
                int favorLev_ = (int)currentBlock_.favorChecks[j].myFavorLevel;
                //Set the type you'll be checking against
                choiceType = currentBlock_.favorChecks[j].myType;

                print("Choice Check: " + choiceNum_ + ", " + i);
                print(choiceType + " Favor Check: " + favorLev_ + ", " + favorTotals_[(int)choiceType]);

                //Check if the current favorCheck being observed has the same choiceNumber as the current block choices
                if (choiceNum_ == i && favorLev_ > favorTotals_[(int)choiceType])
                {
                    //Check if the choice outreaches the amount given
                    willActivate = false;
                    break;
                }
            }

            myChoices[i].ActivateChoice(willActivate, currentID_ + "_Choice_" + (i + 1), choiceType);
        }
    }

    public void RecordChoices(int ChoiceNum_)
    {
        if(ChoiceNum_ <= myChoices.Count)
        {
            string choiceMade = myChoices[ChoiceNum_ - 1].GetChoiceName();
            string choiceFinal = "";

            //remove the last char in choicemade to avoid problems
            for (int i = 0; i < choiceMade.Length - 1; i++)
            {
                choiceFinal += choiceMade[i];
                //print(choiceFinal[i]);
            }

            //Adjust favor based on the choice made
            screenPlayer.ChangeFavor(ChoiceNum_);
            screenPlayer.GoToNewBlockSet(choiceFinal);
        }
    }

    public void DeActivateChoice()
    {
        myImage.enabled = false;
        clickImage.enabled = true;

        //turn off all choices
        for (int i = 0; i < myChoices.Count; i++)
        {
            myChoices[i].EndChoice();
        }
    }

    public void GetChoicesBlock(int choiceNum_)
    {

    }
}
