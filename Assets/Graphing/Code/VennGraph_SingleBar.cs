//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_SingleBar : MonoBehaviour 
{
    [Tooltip("The rect transform that is used to grow the bar.")]
    public RectTransform BarRect;

    [Tooltip("The actual image that makes up the bar display.")]
    public Image BarImage;

    [Tooltip("The value text of the bar, if set up it will show the y value and it's label.")]
    public Text BarText;


    //Used to ensure that bars are able to grow one by one
    [HideInInspector]
    public bool DoneGrowing = true;

    //Saved width, height, and animation time for use when growing the bars on the app
    private float m_finalWidth = 0.0f;
    private float m_finalHeight = 0.0f;
    private float m_finalAnimTime = 0.0f;


    //Set size and postion of bar based on values provided
    public void SetBarSizeAndPosition(float xPos_, float height_, float width_, Vector2 graphMin_, float barAnimationTime = 0.0f)
    {
        if (BarRect != null)
        {
            m_finalWidth = width_;
            m_finalHeight = height_;

            BarRect.sizeDelta = new Vector2(width_, 0.0f);

            //Change size and offset of rect as expected, calling a grow bar routine if appropriate
            if (barAnimationTime <= 0.0f)
            {
                BarRect.offsetMax = new Vector2(BarRect.offsetMax.x, m_finalHeight);
            }
            else
            {
                DoneGrowing = false;
                m_finalAnimTime = barAnimationTime;
            }

            transform.localPosition = new Vector2(graphMin_.x + xPos_, graphMin_.y);
        }
        else
        {
            Debug.LogError("ERROR: You're attempting to set the size and position of this bar, but there is no BarRect set to this object. Please check your prefab and try again.");
        }
    }


    //Make the bar grow over the time specified
    public IEnumerator AnimateBarSize()
    {
        BarRect.sizeDelta = new Vector2(m_finalWidth, BarRect.sizeDelta.y);

        yield return null;

        float timePassed = 0.0f;

        while(timePassed <= m_finalAnimTime)
        {
            float percentSize = ((float)timePassed / (float)m_finalAnimTime);

            if (percentSize > 1.0f)
                percentSize = 1.0f;
            
            BarRect.offsetMax = new Vector2(BarRect.offsetMax.x, m_finalHeight * percentSize);

            timePassed += Time.deltaTime;

            yield return null;
        }

        BarRect.offsetMax = new Vector2(BarRect.offsetMax.x, m_finalHeight);

        DoneGrowing = true;
    }


    //Set the color of the bar display
    public void SetBarColor(Color col_)
    {
        if (BarImage != null)
            BarImage.color = col_;
        else
            Debug.LogError("ERROR: You're attempting to set the bar color, but there is no BarImage set to this object. Please check your prefab and try again.");
    }


    //Set the text of the bar display
    public void SetBarText(string val_)
    {
        if(BarText != null)
            BarText.text = val_;
        else
            Debug.LogError("ERROR: You're attempting to set the bar text, but there is no BarText set to this object. Please check your prefab and try again.");
    }
}
