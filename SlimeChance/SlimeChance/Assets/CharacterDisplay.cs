using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour
{
    //Position, speaker, and active status
    private Vector2 myPos;
    private Vector2 oldPos;
    private bool currentSpeaker = false;
    private bool currentActive = false;

    //Image and screensize base
    private Image myImage;
    private Image oldImage;
    private Color myColor;

	// Use this for initialization
	void Awake ()
    {
        myImage = transform.GetChild(0).GetComponent<Image>();
        oldImage = GetComponent<Image>();

        oldPos = transform.localPosition;
        myPos = oldPos;

        myColor = Color.gray;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CharacterChanges();
	}

    private void CharacterChanges()
    {
        //change character image color and position to match any changes
        Vector2 newPos = myPos;
        
        if (currentSpeaker)
        {
            newPos += new Vector2(0, 15);
        }

        float newX = Mathf.MoveTowards(oldPos.x, newPos.x, 0.1f);
        float newY = Mathf.MoveTowards(oldPos.y, newPos.y, 0.1f);

        //change position and color if any
        transform.localPosition = new Vector2(newX, newY);
        myImage.color = Color.LerpUnclamped(myImage.color, myColor, 0.15f);
    }

    public void ChangeSprite(Sprite newSprite_)
    {
        oldImage.sprite = myImage.sprite;
        myImage.sprite = newSprite_;

        myImage.color = Color.clear;
        oldImage.color = myColor;
    }

    public void ChangeSpeaker(bool newSpeaker_, bool newActive_ = false)
    {
        //change who speaker and who active char is
        currentSpeaker = newSpeaker_;
        currentActive = newActive_;

        if(currentSpeaker)
        {
            myColor = Color.white;
        }
        else if(currentActive)
        {
            myColor = Color.gray;
        }
        else
        {
            myColor = Color.clear;
            oldImage.color = myColor;
        }
    }

    public void ChangePosition(Vector2 myNewPos_)
    {
        //meant to be used on all of them at once
        oldPos = myPos;
        myPos = myNewPos_;
    }
}
