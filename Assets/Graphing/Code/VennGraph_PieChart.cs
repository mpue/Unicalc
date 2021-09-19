//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_PieChart : VennGraph 
{
    [Header("Pie Chart Elements")]
    [Tooltip("Prefab for each Slice of the Pie Chart should typically be an object with a circular image that is set to type 'Radial'.")]
    public VennGraph_PieSlice PieSlicePrefab;

    [Tooltip("The grid group that contains the legend for this pie chart, so users can reference what each part of the pie is related to.")]
    public GridLayoutGroup PieLegendGrid;

    [Tooltip("A circular mask used to hide the pie chart and reveal it in a stylish manner. Select Instant in ChartShowType below if you don't want your chart to be initially hidden.")]
    public Image ChartHideRect;


    [Header("Pie Chart Settings")]
    [Tooltip("Select in what order your slices display.\n\n-Input = Displayed in order provided in script\n-Ascending = smallest to largest\n-Descending = largest to smallest")]
    public PieChartDisplayOrder SliceDisplayOrder = PieChartDisplayOrder.InputOrder;

    [Tooltip("Indicate how you want the chart to be revealed to the user.\n\n-Instant = No delay\n-FullReveal = Reveal all in one single motion\n-SliceBySlice = Reveal one slice at a time")]
    public PieChartShowType ChartShowType = PieChartShowType.Instant;

    [Tooltip("Allows you to set the time it takes for the graph to be fully revealed when ChartHideRect is in place.")]
    [Range(0.0f, 3.0f)]
    public float ChartRevealTime = 0.0f;


    //Saved graph slices and legend display objects
    private List<VennGraph_PieSlice> m_chartSlices = new List<VennGraph_PieSlice>();
    private List<Text> m_chartLegends = new List<Text>();


    //Add a single x/y value set to the pie chart
    public override void AddSingleValueSetToGraph(string xVal_, float yVal_, Color displayColor_, string yValueLabel_ = "")
    {
        base.AddSingleValueSetToGraph(xVal_, yVal_, displayColor_, yValueLabel_);

        //Call to set up pie chart
        SetUpPieChart();
    }


    //Set up graph with preset x and y values
    public override void SetUpGraphViaValues(string[] xVals_, float[] yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaValues(xVals_, yVals_, displayColors_, yValueLabel_);

        //Call to set up pie chart
        SetUpPieChart();
    }


    //Generates pie chart slices and create the legend elements
    private void SetUpPieChart()
    {
        //Remove all previously made graph elements so we don't double up on our dispaly
        ClearOutGraphElements();

        //Change the order of the x and y elements in the game in order to improve 
        if(SliceDisplayOrder != PieChartDisplayOrder.InputOrder)
            ChangeDisplayOrder();

        //Save the total value of the various y elements that make up this chart so we can check the ratio of each individual value
        float yValTotal = 0.0f;

        //Reminder to avoid negative values if possible, as negative values in pie charts can be confusing for users to view
        bool negativeCheck = true;

        foreach (float val in m_ySavedValues)
        {
            if (val < 0.0f && negativeCheck)
            {
                Debug.LogError("NOTE: Pie Charts are made to show the total percentage that each value possesses in regards to the larger whole. Negative values can be shown this way, but may be confusing to users.");
                Debug.LogError("We recommend only using positive values with pie charts, and using other graph types (such as Area or Line Graphs) to represent positive and negative values in tandem.");
                negativeCheck = false;
            }

            //Add to total y value
            yValTotal += Mathf.Abs(val);
        }

        //Save the previous portion total in order to ensure we can accurately dispaly each portion of the pie chart in relation to all the others
        float prevPortion = 0.0f;

        //Go through all the saved values and display them
        for (int i = 0; i < m_xSavedValues.Count; i++)
        {
            //Instantiate a pie slice with parent of GraphBounds
            VennGraph_PieSlice slice = Instantiate(PieSlicePrefab, GraphBounds) as VennGraph_PieSlice;

            //Set pie slice to the center of the parent
            slice.transform.localPosition = Vector2.zero;

            //Set this slice size as equal to prevportion after it's been set up, then move it to the front
            prevPortion = slice.SetSliceProportion((Mathf.Abs(m_ySavedValues[i]) / yValTotal), prevPortion);
            slice.transform.SetAsFirstSibling();

            //Set the color of the slice
            slice.SetSliceColor(m_savedDisplayColors[i]);

            //Add to saved chart slices list
            m_chartSlices.Add(slice);

            //Instantiate a legend piece, and apply text that tells user what it's like
            Text legendText = Instantiate(AxisValueTextPrefab) as Text;
            legendText.text = m_xSavedValues[i] + "\n\n" + m_ySavedValues[i].ToString("#####0.#") + " " + m_savedYValueLabel;

            //Set to pie legend grid as parent and set in order
            legendText.transform.SetParent(PieLegendGrid.transform);
            legendText.transform.SetSiblingIndex(i);

            //Add to saved chart legends list
            m_chartLegends.Add(legendText);

            //If there is any legend image in the text object
            Image legendImage = legendText.GetComponentInChildren<Image>();

            //If there is an image, set the color to it, otherwise change the legend text color instead
            if (legendImage != null)
                legendImage.color = m_savedDisplayColors[i];
            else
                legendText.color = m_savedDisplayColors[i];
        }

        //Apply chart hide rect reveal based on reveal type
        if(ChartHideRect != null)
        {
            if (ChartShowType == PieChartShowType.Instant)
            {
                ChartHideRect.fillAmount = 0.0f;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(RevealPieChart());
            }   
        }
    }


    //Change order of x and y values to display type desired
    private void ChangeDisplayOrder()
    {
        //Set up slices as x, y, and color with temporary internal pie slices so we don't impact our original arrays
        List<InternalPieSlice> tempSlices = new List<InternalPieSlice>();

        for (int i = 0; i < m_xSavedValues.Count; i++)
        {
            InternalPieSlice slice = new InternalPieSlice();
            slice.xVal = m_xSavedValues[i];
            slice.yVal = m_ySavedValues[i];
            slice.col = m_savedDisplayColors[i];

            tempSlices.Add(slice);
        }

        //Sort based on y value
        tempSlices.Sort((slice1, slice2) => Mathf.Abs(slice1.yVal).CompareTo(Mathf.Abs(slice2.yVal)));

        //Clear out all the x, y, and color values so we can re-add them in our new order
        m_xSavedValues.Clear();
        m_ySavedValues.Clear();
        m_savedDisplayColors.Clear();

        //Go through each temporary pie slice and re-add to each saved value list
        foreach(InternalPieSlice pieSlice in tempSlices)
        {
            if(SliceDisplayOrder == PieChartDisplayOrder.AscendingOrder)
            {
                //Add them back in as smallest to largest
                m_xSavedValues.Add(pieSlice.xVal);
                m_ySavedValues.Add(pieSlice.yVal);
                m_savedDisplayColors.Add(pieSlice.col);
            }
            else
            {
                //Add them back in as largest to smallest
                m_xSavedValues.Insert(0, pieSlice.xVal);
                m_ySavedValues.Insert(0, pieSlice.yVal);
                m_savedDisplayColors.Insert(0, pieSlice.col);
            }
        }
    }


    //Remove all slices and legend pieces, then clear lists as well
    public override void ClearOutGraphElements()
    {
        base.ClearOutGraphElements();

        if(m_chartSlices.Count > 0 || m_chartLegends.Count > 0)
        {
            foreach (VennGraph_PieSlice slice in m_chartSlices)
                Destroy(slice.gameObject);

            foreach (Text legend in m_chartLegends)
                Destroy(legend.gameObject);

            m_chartSlices.Clear();
            m_chartLegends.Clear();
        }
    }


    //Reveal the pie chart itself, either by full reveal or slice by slice
    private IEnumerator RevealPieChart()
    {
        //Set chart hide rect to full
        ChartHideRect.fillAmount = 1.0f;

        //Save and hold time as it changes frame by frame
        float timePassed = 0.0f;

        yield return new WaitForSeconds(0.25f);

        if(ChartShowType == PieChartShowType.SliceBySlice)
        {
            float totalPortion = 0.0f;

            for (int i = 0; i < m_chartSlices.Count; i++)
            {
                yield return new WaitForSeconds(0.25f);

                //Go through each slice, and reveal it based on time associated
                while(timePassed <= ChartRevealTime)
                {
                    //Determine the size to reveal at this point, and the portion of the actual slice its associated with
                    float percentSize = (timePassed / ChartRevealTime);
                    float portionControl = totalPortion + (percentSize * m_chartSlices[i].m_myProportion);

                    //Make sure it doesn't get larger than it should be
                    if (portionControl > 1.0f)
                        portionControl = 1.0f;

                    //
                    ChartHideRect.fillAmount = 1.0f - portionControl;

                    timePassed += Time.deltaTime;

                    yield return null;
                }

                timePassed = 0.0f;
                totalPortion += m_chartSlices[i].m_myProportion;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.25f);

            //Set size of hide rect and reveal based on time input
            while(timePassed <= ChartRevealTime)
            {
                float percentSize = (timePassed / ChartRevealTime);

                if (percentSize > 1.0f)
                    percentSize = 1.0f;

                ChartHideRect.fillAmount = 1.0f - percentSize;

                timePassed += Time.deltaTime;

                yield return null;
            }
        }

        //Make sure the hide rect is set back to 0
        ChartHideRect.fillAmount = 0.0f;
    }
}


//Enum used to determine what order values are supposed to be in
public enum PieChartDisplayOrder
{
    InputOrder,
    AscendingOrder,
    DescendingOrder,
}


//Enum used to determine how and in what way should a pie chart
public enum PieChartShowType
{
    Instant,
    FullReveal,
    SliceBySlice
}


//Temporary class used for when changing display order
class InternalPieSlice
{
    public string xVal;
    public float yVal;
    public Color col;
}
