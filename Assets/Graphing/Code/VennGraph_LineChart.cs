//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_LineChart : VennGraph 
{
    [Header("Line Graph Elements")]
    [Tooltip("Prefab used to set up the points on each line in the graph.")]
    public VennGraph_Point LinePointPrefab;

    [Tooltip("Prefab used to set up the lines between each point on the graph.")]
    public RectTransform LineBodyPrefab;

    [Tooltip("Object used to hide chart, used to make the graph look more slick. Leave empty or set ChartRevealTime as 0.0f if you don't want to have your graph hidden beforehand.")]
    public RectTransform ChartHideRect;


    [Header("Line Graph Display Settings")]
    [Tooltip("Set the thickness of the lines being drawn on the graph.")]
    [Range(0f, 100f)]
    public float LineThickness = 1f;

    [Tooltip("Allows you to set the buffer added to the top and bottom of the graph in terms of max and min y values.")]
    [Range(0.0f, 1f)]
    public float HeightLeewayPercentage = 0.0f;

    [Tooltip("Allows you to set the time it takes for the graph to be fully revealed when ChartHideRect is in place.")]
    [Range(0.0f, 3.0f)]
    public float ChartRevealTime = 0.0f;


    [Header("Line Graph Axis Settings")]
    [Tooltip("The number of x axis markers to be shown. Set to be greater than the number of elements being provided to ensure all are shown.")]
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


    //The width of the hidden chart rect
    private float m_ChartHideWidth = 0.0f;

    //Saved graph points and lines
    private List<VennGraph_Point> m_graphPoints = new List<VennGraph_Point>();
    private List<RectTransform> m_graphLines = new List<RectTransform>();


    private void Awake()
    {
        //Set up chart hide width if desired
        if (ChartHideRect != null)
            m_ChartHideWidth = ChartHideRect.sizeDelta.x;
    }


    //Add a single x/y value set to the line graph
    public override void AddSingleXAndYValueToGraph(string xVal_, float yVal_, string yValueLabel_ = "")
    {
        base.AddSingleXAndYValueToGraph(xVal_, yVal_, yValueLabel_);

        //Call to set up line graph
        SetLineGraphPoints(false);
    }
	

    //Set up graph with preset x and y values
    public override void SetUpGraphViaValues(string[] xVals_, float[] yVals_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaValues(xVals_, yVals_, yValueLabel_);

        //if there are less than 2 x or y values, add to them so that the line graph is able to display properly
        if (m_xSavedValues.Count == 1)
            m_xSavedValues.Add(m_xSavedValues[0]);

        if (m_ySavedValues.Count == 1)
            m_ySavedValues.Add(m_ySavedValues[0]);

        //Call to set up line graph
        SetLineGraphPoints(false);
    }


    //Set up multi-line graph with preset x values and multiple y value arrays
    public override void SetUpGraphViaMultipleValueSets(string[] xVals_, List<float[]> yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaMultipleValueSets(xVals_, yVals_, displayColors_, yValueLabel_);

        //Call to set up multi-line graph
        SetLineGraphPoints(true);
    }


    //Vectors that hold where the graphs are being placed so we can later set our x axis markers in parent class
    private List<Vector2> m_graphVectors = new List<Vector2>();


    //Display line graph points, first determining if we're using multiple values sets (such as with a multi-line graph) or just a single value set
    private void SetLineGraphPoints(bool multiValues_)
    {
        //Remove all previously made graph elements so we don't double up on our dispaly
        ClearOutGraphElements();

        //Set up initial minimum y and maximum y values
        float MinY = 0.0f;
        float MaxY = 0.0f;

        //Enter in our values and return them with the values we now have
        SetMinAndMaxYValues(out MinY, out MaxY, HeightLeewayPercentage);

        //Determine if we're using multiple value sets or a single value set
        if(multiValues_)
        {
            //go through each set of lines that are going to be built out, as defined by the number of y values associated with each x value  
            for (int i = 0; i < m_multiSavedValues[m_xSavedValues[0]].Length; i++)
            {
                List<float> tempYs = new List<float>();

                //Create the set of y values associated with a single line graph
                for (int j = 0; j < m_xSavedValues.Count; j++)
                {
                    float yValue = m_multiSavedValues[m_xSavedValues[j]][i];
                    tempYs.Add(yValue);
                }

                //Build that individual graph and repeat the process for each set of y values
                BuildChartWithYValues(MinY, MaxY, tempYs.ToArray(), i == 0, m_savedDisplayColors[i]);
            }
        }
        else
        {
            //Build out y values with the single value set
            BuildChartWithYValues(MinY, MaxY, m_ySavedValues.ToArray(), true);
        }

        //To ensure all major elements of the lines themselves are visible, we move them to the front of the graph by setting their sibling index to last
        MoveAllLinesAndPointsToFront();

        //Set up x Axis Markers
        SetXAxisMarkers(m_xSavedValues.ToArray(), m_graphVectors.ToArray(), XAxisElementNumber, XAxisTextSpacing);

        //Set up y Axis Markers after determining how many vectors there are to be used
        int markersToShow = YAxisElementNumber <= m_graphVectors.Count ? YAxisElementNumber : m_graphVectors.Count;
        SetYAxisMarkers(MinY, MaxY, markersToShow, YAxisTextSpacing);

        //Determine if chart hide rect is available, then either set it to start revealing the line graph, or just set it to revealed
        if (ChartHideRect != null)
        {
            if(ChartRevealTime > 0.0f)
            {
                StopAllCoroutines();
                StartCoroutine(ShowLineChartViaMask());
            }
            else
            {
                ChartHideRect.sizeDelta = new Vector2(0, ChartHideRect.sizeDelta.y);
            }
        }
    }


    //Build actual points and lines on the chart using the y values given
    private void BuildChartWithYValues(float minY_, float maxY_, float[] yVals_, bool addVectors_, Color col_ = new Color())
    {
        //Prev point position that is used to determine where the line is going to be placed when setting it between the two points
        Vector3 prevPointPosition = new Vector3();

        //Go through y values that are going to be set up as points
        for (int i = 0; i < yVals_.Length; i++)
        {
            //Instantiate line graph point with parent object of GraphBounds
            VennGraph_Point setPoint = Instantiate(LinePointPrefab, GraphBounds) as VennGraph_Point;

            //Find the x position and y position of the point itself based on percentage
            float percentageAmntX = (((float)i / (float)(yVals_.Length - 1)));
            float percentageAmntY = ((yVals_[i] - minY_) / (maxY_ - minY_));

            //Apply point and set text equal to the value provided
            Vector2 adjustPoint = new Vector2(m_AxisWidth * percentageAmntX, m_AxisHeight * percentageAmntY);

            setPoint.transform.localPosition = GraphBounds.rect.min + adjustPoint;
            setPoint.SetTextValue(yVals_[i].ToString("#####.#") + " " + m_savedYValueLabel);

            //If a color has been set up for this line, change the icon to that color
            if(col_ != new Color())
                setPoint.SetIconColor(col_);

            //Add to our saved graph points list
            m_graphPoints.Add(setPoint);

            //Only applies lines to points after the first one
            if (i > 0)
            {
                //Instantiate a line graph point with parent object of GraphBounds
                RectTransform setLine = Instantiate(LineBodyPrefab, GraphBounds) as RectTransform;
                setLine.SetAsFirstSibling();

                //If a color has been set up up for this line, change the line to that color
                if (col_ != new Color())
                    setLine.GetComponent<Image>().color = col_;

                //Place graph line directly centered between each point
                setLine.transform.localPosition = Vector2.Lerp(prevPointPosition, setPoint.transform.localPosition, 0.5f);

                //Rotate graph line to be facing with forward towards new point
                Vector3 startDir = prevPointPosition - setPoint.transform.localPosition;
                float angle = Mathf.Atan2(startDir.y, startDir.x) * Mathf.Rad2Deg;
                setLine.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

                //Find distance between these two points, and apply it to the scale/anchors of the graph line
                setLine.sizeDelta = new Vector2(LineThickness, Vector3.Distance(prevPointPosition, setPoint.transform.localPosition));

                //Add to our saved graph lines list
                m_graphLines.Add(setLine);
            }

            //Set the previous point position
            prevPointPosition = setPoint.transform.localPosition;

            if(addVectors_)
                m_graphVectors.Add(new Vector2(prevPointPosition.x, prevPointPosition.y));
        }
    }


    //Ensures that all the lines and points are visible on the graph ahead of any other elements
    private void MoveAllLinesAndPointsToFront()
    {
        foreach (RectTransform line in m_graphLines)
            line.transform.SetAsLastSibling();

        foreach (VennGraph_Point point in m_graphPoints)
            point.transform.SetAsLastSibling();
    }


    //Remove all points and lines, then clear lists as well
    public override void ClearOutGraphElements()
    {
        base.ClearOutGraphElements();

        if (m_graphPoints.Count > 0 || m_graphLines.Count > 0)
        {
            foreach (VennGraph_Point point in m_graphPoints)
                Destroy(point.gameObject);

            foreach (RectTransform line in m_graphLines)
                Destroy(line.gameObject);

            m_graphPoints.Clear();
            m_graphLines.Clear();
        }

        m_graphVectors.Clear();
    }


    //Reveal line graph by changing size delta of ChartHideRect
    private IEnumerator ShowLineChartViaMask()
    {
        //Set up time passed and original y size of rect
        float timePassed = 0.0f;
        float ySize = ChartHideRect.sizeDelta.y;

        //Make sure it's set up in full
        ChartHideRect.sizeDelta = new Vector2(m_ChartHideWidth, ySize);

        //Brief delay
        yield return new WaitForSeconds(0.5f);

        //Reveal bit by bit based on time input
        while (timePassed <= ChartRevealTime)
        {
            float percentSize = (timePassed / ChartRevealTime);

            if (percentSize > 1.0f)
                percentSize = 1.0f;

            ChartHideRect.sizeDelta = new Vector2(m_ChartHideWidth - (m_ChartHideWidth * percentSize), ySize);

            timePassed += Time.deltaTime;

            yield return null;
        }

        //Reveal in full by setting hide rect width to 0
        ChartHideRect.sizeDelta = new Vector2(0, ySize);
    }
}
