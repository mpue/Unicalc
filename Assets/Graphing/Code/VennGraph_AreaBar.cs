//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_AreaBar : MonoBehaviour
{
    [Tooltip("The top of the bar, used to fill out the 'ramp' created by the bar to indicate a change in value.")]
    public RectTransform BarTopper;

    [Tooltip("The core rect of the bar itself, used to change the size and position of the bar. Should hold both the BarTopperImage and BarBodyImage.")]
    public RectTransform BarRect;

    [Tooltip("The top of the bar's display image, used to change color.")]
    public Image BarTopperImage;

    [Tooltip("The bar's core display image, used to change color.")]
    public Image BarBodyImage;


    //Set size and postion of bar based on values provided
    public void SetBarSizeAndPosition(float width_, int barNum_, float prevHeight_, float nextHeight_, Vector2 graphMin_)
    {
        //Works similar to single bar, except it also sets the top of the bar based on current height, and then the height of the previous value
        if (BarRect != null)
        {
            //Set up the core height as smaller of two heights, topper height as absolute value diffence in size between the two and doubled to make up for shortfall
            float coreHeight = prevHeight_ <= nextHeight_ ? prevHeight_ : nextHeight_;
            float topperHeight = (Mathf.Abs(nextHeight_ - prevHeight_) * 2) + BarTopper.sizeDelta.y;

            if (prevHeight_ <= nextHeight_)
                BarTopper.localEulerAngles = new Vector3(0, 180, 0);

            //Set size of bar rect and bar topper directly 
            BarRect.offsetMax = new Vector2(width_, coreHeight);
            BarTopper.offsetMax = new Vector2(width_, topperHeight);

            BarTopper.localPosition = new Vector2(BarTopper.localPosition.x, (BarRect.localPosition.y + (BarRect.sizeDelta.y)));
            transform.localPosition = new Vector2(graphMin_.x + (width_ * (barNum_ - 1)), graphMin_.y);
        }
        else
        {
            Debug.LogError("ERROR: You're attempting to set the size and position of this bar, but there is no BarRect set to this object. Please check your prefab and try again.");
        }
    }


    //Change the color of both the bar and bar topper
    public void SetBarColor(Color col_)
    {
        if (BarTopper != null && BarBodyImage != null)
        {
            BarTopperImage.color = col_;
            BarBodyImage.color = col_;
        }
        else
        {
            Debug.LogError("ERROR: You're attempting to set the color of this bar, but there is either no BarTopperImage or BarBodyImage set on this object. Please check your prefab and try again.");
        }
    }
}
