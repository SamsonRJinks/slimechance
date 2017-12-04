using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class FavorContainer
{
    public int FavorValue;
}

public class FavorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool SlimeanthaOrRoozelyn;
    
    //Array of references to the Favor % wedges in this order: Swimming, Literature, Paranormal, Cooking
    public GameObject[] WedgeArray;

    //Scorekeeping array for all current wedge percentages from 0 to 100. Matches target wedge's Width & Height in the Rect Transform.
    public float[] WedgePercentages;

    //References to all text percentages
    public Text[] TextPercentages;

    //Reference to the CanvasGroup on this circle's TextMommy object, which can control the alpha level of every percentage text in the circle
    public CanvasGroup TextAlpha;

    //Lerping variables for when we need to to increment the wedge percentage
    float TotalAnimationTime = 1.0f;
    float CurrentTimePassed;
    float CurrentTimePassedWedge;

    //Boolean determining if the circle is zoomed in or out (True or False)
    bool ZoomStatus;

    //Event that tells the circle to increment a wedge
    public delegate void FavorChangeEvent(int TargetWedge, int PercentChange, bool PlusOrMinus, bool SliOrRoo);
    public static event FavorChangeEvent OnFavorChange;
    public delegate FavorContainer ReturnFavorEvent(int TargetWedge, FavorContainer FavorValueHolder, bool SliOrRoo);
    public static event ReturnFavorEvent ReturnFavorValue;


    // Use this for initialization
    void Start ()
    {
		//Set the favor to the current value for all wedges and text prompts
        //NOTE TO SAM: Make sure that the Favor values remain saved in the save file!
        for (int i = 0; i < 3; i++)
        {
            WedgeAnimate(i, Mathf.RoundToInt(WedgePercentages[i]), true);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Testing the event sending a 10% increase to Swim Club's wedge
        if (Input.GetKeyDown(KeyCode.T))
        {
            FavorContainer fc = new FavorContainer();
            ReturnFavorValue(0, fc, false);
            Debug.Log(fc.FavorValue);
        }
	}

    void OnEnable ()
    {
        OnFavorChange += StartWedgeAnimate;
        ReturnFavorValue += GetFavorValue;
    }

    void OnDisable()
    {
        OnFavorChange -= StartWedgeAnimate;
        ReturnFavorValue -= GetFavorValue;
    }

    //When the pointer enters the circle, it is zoomed in
    public void OnPointerEnter (PointerEventData eventData)
    {
        //If the circle is not zoomed in, zoom it in when moused over.
        if (ZoomStatus == false)
        {
            StopCoroutine("ZoomFavorCircle");
            CurrentTimePassed = 0f;
            StartCoroutine(ZoomFavorCircle(!ZoomStatus, false));
            ZoomStatus = true;
        }
    }

    //When the pointer exits the circle, it is zoomed back out
    public void OnPointerExit (PointerEventData eventData)
    {
        StopCoroutine("ZoomFavorCircle");
        CurrentTimePassed = 0f;
        StartCoroutine(ZoomFavorCircle(false, false));
        ZoomStatus = false;
    }

    //Animation that plays when the percentage value for a wedge changes
    IEnumerator WedgeAnimate (int TargetWedge, int PercentChange, bool PlusOrMinus)
    {
        while (CurrentTimePassedWedge < TotalAnimationTime)
        {
            //Incrementing time
            CurrentTimePassedWedge += Time.deltaTime;

            //Creating animation curve to control animation
            float t = TotalAnimationTime / CurrentTimePassedWedge;
            //Fadeoff curve
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            //Getting new lerped Height & Width value for this frame
            float NewSize = Mathf.Lerp(WedgePercentages[TargetWedge], (WedgePercentages[TargetWedge] + PercentChange), t);

            //Changing text percentage
            TextPercentages[TargetWedge].text = (Mathf.RoundToInt(NewSize) + "%");

            //Lerping the Width & Height of the target wedge
            WedgeArray[TargetWedge].GetComponent<RectTransform>().sizeDelta = new Vector2 (NewSize, NewSize);

            yield return null;
        }

        //Resets all related values when the coroutine is finished.
        if (CurrentTimePassed >= TotalAnimationTime)
        {
            //Resetting time value
            CurrentTimePassedWedge = 0f;
            CurrentTimePassed = 0f;

            //Officially changing Favor value
            WedgePercentages[TargetWedge] = WedgePercentages[TargetWedge] + PercentChange;

            StartCoroutine(ZoomFavorCircle(false,false));
            StopCoroutine("WedgeAnimate");
        }
    }

    //Zooms the Favor Circle in or out (Bool true or false) depending on the animation's needs
    IEnumerator ZoomFavorCircle (bool InOrOut, bool WillIncrement)
    {
        //Zooming Circle In (Larger)
        while (InOrOut == true && CurrentTimePassed < TotalAnimationTime)
        {
            //Incrementing time
            CurrentTimePassed += Time.deltaTime;

            //Creating animation curve to control animation
            float t = TotalAnimationTime / CurrentTimePassed;
            //Smoothstep curve
            t = t * t * (3f - 2f * t);

            //Getting new lerped Scale value for this frame
            float NewScale = Mathf.Lerp(1.15f, 2f, t);
            float NewAlpha = Mathf.Lerp(0f, 1f, t);

            //Lerping the scale of the circle and the alpha of the enclosed text
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(NewScale, NewScale, NewScale);
            TextAlpha.alpha = NewAlpha;

            //Switching the current zoom state after the animation is finished
            if (CurrentTimePassed >= TotalAnimationTime)
            {
                ZoomStatus = true;
                StopCoroutine("ZoomFavorCircle");
            }

            yield return null;
        }

        //Zooming Circle Out (Smaller)
        while (InOrOut == false && CurrentTimePassed < TotalAnimationTime)
        {
            //Incrementing time
            CurrentTimePassed += Time.deltaTime;

            //Creating animation curve to control animation
            float t = TotalAnimationTime / CurrentTimePassed;
            //Smoothstep curve
            t = t * t * (3f - 2f * t);

            //Getting new lerped Scale value for this frame
            float NewScale = Mathf.Lerp(2f, 1.15f, t);
            float NewAlpha = Mathf.Lerp(1f, 0f, t);

            //Lerping the scale of the circle and the alpha of the enclosed text
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(NewScale, NewScale, NewScale);
            TextAlpha.alpha = NewAlpha;

            //Switching the current zoom state after the animation is finished
            if (CurrentTimePassed >= TotalAnimationTime)
            {
                ZoomStatus = false;
                StopCoroutine("ZoomFavorCircle");
            }

            yield return null;
        }
    }

    //Serves as the delay before the Wedge Increment starts, to account for the zoom time of the circle
    IEnumerator DelayIncrement (int TargetWedge, int PercentChange, bool PlusOrMinus)
    {
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(WedgeAnimate(TargetWedge, PercentChange, PlusOrMinus));
    }

    //External (Static) means of letting you call OnFavorChange
    public static void InvokeFavorChange(int TargetWedge, int PercentChange, bool PlusOrMinus, bool SliOrRoo)
    {
        OnFavorChange(TargetWedge, PercentChange, PlusOrMinus, SliOrRoo);
    }

    public static void InvokeFavorCheck(int TargetWedge, FavorContainer FavorValueHolder, bool SliOrRoo)
    {
        ReturnFavorValue(TargetWedge, FavorValueHolder, SliOrRoo);
    }

    //Starts the process of animating a wedge percentage increase, since event calls can't start coroutines.
    void StartWedgeAnimate (int TargetWedge, int PercentChange, bool PlusOrMinus, bool SliOrRoo)
    {
        //If the % is less than 100
        if (WedgePercentages[TargetWedge] <= 100 && SliOrRoo == SlimeanthaOrRoozelyn)
        {
            CurrentTimePassedWedge = 0f;
            CurrentTimePassed = 0f;

            StartCoroutine(ZoomFavorCircle(true, true));
            StartCoroutine(DelayIncrement(TargetWedge, PercentChange, PlusOrMinus));
        }
    }

    //Takes the writable container meant to hold the returnable favor value and adds in the requested favor value
    FavorContainer GetFavorValue (int TargetWedge, FavorContainer FavorValueHolder, bool SliOrRoo)
    {
        //If this event was delivered to the right person, read back their value
        if (SliOrRoo == SlimeanthaOrRoozelyn)
        {
            FavorValueHolder.FavorValue = Mathf.RoundToInt(WedgePercentages[TargetWedge]);
            return FavorValueHolder;
        }

        else
            return null;
    }
}
