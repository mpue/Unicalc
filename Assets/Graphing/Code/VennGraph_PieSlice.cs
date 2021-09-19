//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_PieSlice : MonoBehaviour 
{
    [Tooltip("The actual image of the pie slice. Should be a full circle, with Image type set to radial.")]
    public Image SliceImage;

    [Tooltip("The holder for the value text itself, used when moving the text around the circle.")]
    public RectTransform SliceTextParent;

    [Tooltip("The value text, indicates the percentage that a given slice makes up of the larger whole.")]
    public Text SliceText;


    //The actual size of the portion that this slice will represent
    public float m_myProportion { get; private set; } 


    //Set the actual slice radial size and percentage value text
    public float SetSliceProportion(float myProportion_, float prevPortion_)
    {
        if(SliceImage != null)
        {
            //Use image radial and fill amount to build out the actual graph
            m_myProportion = myProportion_;

            //Fill out the image based on the current portion and previous portion, then rotate the slice text parent to make sure it is visible at the halfway point of the slice itself
            SliceImage.fillAmount = myProportion_ + prevPortion_;   

            if (SliceTextParent != null && SliceText != null)
            {
                SliceTextParent.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 360.0f * ((myProportion_ * 0.5f) + prevPortion_));
                SliceText.transform.eulerAngles = Vector3.zero;
                SliceText.text = (myProportion_ * 100).ToString("####0.##") + "%";
            }
            else
            {
                Debug.LogError("NOTE: No SliceTextParent and/or SliceText is set to this object. The ratio value of the slice will not be displayed.");
            }

            return SliceImage.fillAmount;
        }
        else
        {
            Debug.LogError("ERROR: No SliceImage is set to this object. The pie slice for your pie chart cannot be displayed until it is set.");
        }

        return 0.0f;
    }


    //Set the color of the pie slice
    public void SetSliceColor(Color col_)
    {
        if(SliceImage != null)
            SliceImage.color = col_;
        else
            Debug.LogError("ERROR: You're attempting to set the pie slice color, but there is no SliceImage set to this object. Please check your prefab and try again.");
    }
}
