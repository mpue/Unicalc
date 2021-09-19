//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_BarChart : VennGraph
{
    [Header("Bar Graph Elements")]
    [Tooltip("Prefab used to set up the bars on the graph. Should usually be a square for ease of use.")]
    public VennGraph_SingleBar SingleBarPrefab;


    [Header("Bar Graph Display Settings")]
    [Tooltip("Indicates what kind of graph reveal type is used.\n\n-Instant: Show all bars on the graph immediately.\n-ShowOneByOne: Show all bars one by one after brief delay on each.\n-AnimateOneByOne: Have each bar grow to their respective height one by one.\n-AnimateAllAtOnce: Have all bars grow to their respective heights at once.")]
    public BarGraphDisplayType GraphDisplayType = BarGraphDisplayType.Instant;

    [Tooltip("Indicates amount of time each bar takes to be shown when animated or shown one by one.")]
    [Range(0.01f, 5.0f)]
    public float BarShowTime = 0.0f;

    [Tooltip("Allows you to set the buffer added to the top and bottom of the graph in terms of max and min y values.")]
    [Range(0.0f, 1.0f)]
    public float HeightLeewayPercentage = 0.0f;


    [Header("Bar Graph Axis Settings")]
    [Tooltip("The number of x axis markers to be shown.Set to be greater than the number of elements being provided to ensure all are shown.")]
    [Range(2, 20)]
    public int XAxisElementNumber = 2;

    [Tooltip("The number of y axis markers to be shown (starting with maximum and minimum, and the rest evenly going between them.")]
    [Range(2, 20)]
    public int YAxisElementNumber = 2;

    [Tooltip("Buffer of space between GraphBounds bottom and X Axis markers.")]
    [Range(0, 150)]
    public int XAxisTextSpacing = 30;

    [Tooltip("Buffer of space between GraphBounds left side and Y Axis markers.")]
    [Range(0, 150)]
    public int YAxisTextSpacing = 30;


    //Save individual bars for the graph
    private List<VennGraph_SingleBar> m_graphBars = new List<VennGraph_SingleBar>();


    //Add a single x/y value set to the line graph
    public override void AddSingleValueSetToGraph(string xVal_, float yVal_, Color displayColor_, string yValueLabel_ = "")
    {
        base.AddSingleValueSetToGraph(xVal_, yVal_, displayColor_, yValueLabel_);
        StopAllCoroutines();

        //Set up bar graph elements
        StartCoroutine(SetUpBarGraph());
    }


    //Set up graph with preset x and y values
    public override void SetUpGraphViaValues(string[] xVals_, float[] yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaValues(xVals_, yVals_, displayColors_, yValueLabel_);
        StopAllCoroutines();

        //If x or y values provided only have one, add another to ensure graph can display properly
        if (m_xSavedValues.Count == 1)
            m_xSavedValues.Add(m_xSavedValues[0]);

        if (m_ySavedValues.Count == 1)
            m_ySavedValues.Add(m_ySavedValues[0]);

        //Set up bar graph elements
        StartCoroutine(SetUpBarGraph());
    }


    //Coroutine used to set up bar graph and place axis elements
    private IEnumerator SetUpBarGraph()
    {
        //use height max and compare floats to find percentage of m_axisheight to use
        ClearOutGraphElements();

        //Set up initial min and max y values
        float MinY = 0.0f;
        float MaxY = 0.0f;

        //Apply min and max y size based on saved values and height buffer
        SetMinAndMaxYValues(out MinY, out MaxY, HeightLeewayPercentage);

        //Set up width of the actual bars based on the size of the graph bounds and number of value sets
        float xWidth = ((m_AxisWidth * 0.5f)) * (1.0f / (float)(m_ySavedValues.Count));

        //Temporary graph vectors list for use when setting up x axis markers
        List<Vector2> graphVectors = new List<Vector2>();

        //Go through y values and generate bars
        for (int i = 0; i < m_ySavedValues.Count; i++)
        {
            //Instantiate bar with parent of GraphBounds
            VennGraph_SingleBar setBar = Instantiate(SingleBarPrefab, GraphBounds) as VennGraph_SingleBar;

            //Based on percentage of x and y to determine the actual position and height of the bar
            float percentageAmntX = (((float)i / (float)(m_ySavedValues.Count - 1)));
            float percentageAmntY = ((m_ySavedValues[i] - MinY) / (MaxY - MinY));

            float xPos = ((m_AxisWidth) * percentageAmntX);
            float yHeight = m_AxisHeight * percentageAmntY;

            //Moves bars closer together to reduce empty space left behind when there are less than 3 bar elements
            if (m_ySavedValues.Count < 3)
                xPos *= 0.5f;

            if (GraphDisplayType == BarGraphDisplayType.Instant || GraphDisplayType == BarGraphDisplayType.ShowOneByOne)
            {
                //Apply changes to bar so it fits on grid properly
                setBar.SetBarSizeAndPosition(xPos, yHeight, xWidth, GraphBounds.rect.min);

                //Set up bar to either be shown later or be shown immediately
                if (GraphDisplayType == BarGraphDisplayType.ShowOneByOne)
                {
                    setBar.SetBarText("");
                    setBar.SetBarColor(Color.clear);
                }
                else
                {
                    setBar.SetBarText(m_ySavedValues[i].ToString("####0.#") + " " + m_savedYValueLabel);
                    setBar.SetBarColor(m_savedDisplayColors[i]);
                }
            }
            else
            {
                //Set up bar to be shown after being animated
                setBar.SetBarSizeAndPosition(xPos, yHeight, xWidth, GraphBounds.rect.min, BarShowTime);

                setBar.SetBarText(m_ySavedValues[i].ToString("####0.#") + " " + m_savedYValueLabel);
                setBar.SetBarColor(m_savedDisplayColors[i]);
            }

            //Add to graph bars list and vectors list
            m_graphBars.Add(setBar);
            graphVectors.Add(setBar.transform.localPosition);
        }

        //Set up x and y axis markers
        SetXAxisMarkers(m_xSavedValues.ToArray(), graphVectors.ToArray(), XAxisElementNumber, XAxisTextSpacing);
        SetYAxisMarkers(MinY, MaxY, YAxisElementNumber, YAxisTextSpacing);

        //Call actual coroutines on each bar if they're to be shown one by one or animated
        if (GraphDisplayType == BarGraphDisplayType.ShowOneByOne)
        {
            for (int i = 0; i < m_graphBars.Count; i++)
            {
                yield return new WaitForSeconds(BarShowTime);
                m_graphBars[i].SetBarText(m_ySavedValues[i].ToString("####0.#") + " " + m_savedYValueLabel);
                m_graphBars[i].SetBarColor(m_savedDisplayColors[i]);
            }   
        }
        else if(GraphDisplayType == BarGraphDisplayType.AnimateOneByOne || GraphDisplayType == BarGraphDisplayType.AnimateAllAtOnce)
        {
            for (int i = 0; i < m_graphBars.Count; i++)
            {
                StartCoroutine(m_graphBars[i].AnimateBarSize());

                if(GraphDisplayType == BarGraphDisplayType.AnimateOneByOne)
                {
                    while (!m_graphBars[i].DoneGrowing)
                        yield return null;   
                }
            }
        }
    }


    //Remove all bars and then clear lists as well
    public override void ClearOutGraphElements()
    {
        base.ClearOutGraphElements();

        if(m_graphBars.Count > 0)
        {
            foreach (VennGraph_SingleBar bar in m_graphBars)
                Destroy(bar.gameObject);

            m_graphBars.Clear();
        }
    }
}


//Enum used to determine the bar graph's display style
public enum BarGraphDisplayType
{
    Instant,
    ShowOneByOne,
    AnimateOneByOne,
    AnimateAllAtOnce
}
