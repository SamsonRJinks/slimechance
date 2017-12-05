using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private SaveLoadGame mySaveLoader;
    private Button[] myButtons = new Button[3];

    public Image frontBacking;

    // Use this for initialization
    void Start()
    {
        mySaveLoader = GetComponent<SaveLoadGame>();

        for(int i = 0; i < transform.childCount; i++)
        {
            myButtons[i] = transform.GetChild(i).GetComponent<Button>();
        }

        if(mySaveLoader.LoadGame())
        {
            myButtons[1].interactable = true;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(Input.GetKey(KeyCode.Escape))
        {
            QuitGame();
        }	
	}

    public void NewGame()
    {
        mySaveLoader.DeleteGame();
        StartCoroutine(NewLevel("Start"));
    }

    public void LoadGame()
    {
        StartCoroutine(NewLevel(GameData.currentFile.myLevelName));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator NewLevel(string myLevel)
    {
        frontBacking.enabled = true;
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(myLevel);
    }
}
