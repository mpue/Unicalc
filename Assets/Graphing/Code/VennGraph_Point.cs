//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_Point : MonoBehaviour 
{
    [Tooltip("The text that is used to display the actual y value of the point.")]
    public Text ValueText;

    [Tooltip("The actual image of the point, used to change its color or sprite.")]
    public Image PointIcon;

    [Tooltip("An optional additional image may be placed in relation to the point as well.")]
    public Image SpriteHolder;


    //Set the value shown on the text
	public void SetTextValue(string val_)
    {
        if (ValueText != null)
            ValueText.text = val_;
        else
            Debug.LogError("ERROR: You are trying to access ValueText when it has not been set on VennGraph_Point.");
    }


    //Set the color of the point icon
    public void SetIconColor(Color col_)
    {
        if(PointIcon != null)
            PointIcon.color = col_;
        else
            Debug.LogError("ERROR: You are trying to access PointIcon when it has not been set on VennGraph_Point.");
    }


    //Set the sprite of the point icon
    public void SetIconImage(Sprite img_)
    {
        if(PointIcon != null)
            PointIcon.sprite = img_;
        else
            Debug.LogError("ERROR: You are trying to access PointIcon when it has not been set on VennGraph_Point.");
    }


    //Set the color of the image shown outside of the actual point
    public void SetSpriteColor(Color col_)
    {
        if(SpriteHolder != null)
            SpriteHolder.color = col_;
        else
            Debug.LogError("ERROR: You are trying to access SpriteHolder when it has not been set on VennGraph_Point.");
    }


    //Set the sprite of the image shown outside of the actual point
    public void SetSpriteImage(Sprite img_)
    {
        if(SpriteHolder != null)
            SpriteHolder.sprite = img_;
        else
            Debug.LogError("ERROR: You are trying to access SpriteHolder when it has not been set on VennGraph_Point.");
    }
}
