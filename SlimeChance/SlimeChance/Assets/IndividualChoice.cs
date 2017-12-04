using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndividualChoice : MonoBehaviour
{
    private StreamReader myDialogue;
    private bool choiceActive = false;

    private string myID = "";

    private Text myText;
    private Image myImage;

    public string myChoiceName = "";

	// Use this for initialization
	void Start ()
    {
        myDialogue = transform.parent.parent.GetComponent<StreamReader>();

        myText = transform.GetChild(0).GetComponent<Text>();
        myImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if(choiceActive)
        {
            ColorShift();
        }
	}

    public void ColorShift()
    {
        myImage.color = Color.Lerp(myImage.color, Color.white, 0.1f);
        myText.color = Color.Lerp(myText.color, Color.black, 0.05f);
    }

    public void ActivateChoice(bool choiceAvailable, string myID_, FavorCheck.FavorType myType_)
    {
        myImage.enabled = true;
        choiceActive = true;

        if (choiceAvailable)
        {
            myID = myID_;

            myImage.raycastTarget = true;

            myChoiceName = myDialogue.GetDialogue(myID).myName;
            myText.text = myChoiceName;
        }
        else
        {
            myImage.raycastTarget = false;

            myText.text = "??? - Requires Higher " + myType_ + " Favor";
        }
    }

    public void EndChoice()
    {
        myImage.color = Color.clear;
        myText.color = Color.clear;

        myImage.enabled = false;

        choiceActive = false;
    }

    public string GetChoiceName()
    {
        return myChoiceName;
    }
}
