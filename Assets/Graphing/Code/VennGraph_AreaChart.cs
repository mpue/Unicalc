//  [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph_AreaChart : VennGraph
{
    [Header("Area Chart Elements")]
    [Tooltip("Prefab used to set up the bars on the graph.")]
    public VennGraph_AreaBar AreaBarPrefab;

    [Tooltip("Prefab used to set up the points where lines intersect on the graph.")]
    public VennGraph_Point AreaPointPrefab;

    [Tooltip("Prefab used to set up the lines that are placed on the top of the graph.")]
    public RectTransform AreaLinePrefab;

    [Tooltip("Object used to hide chart, used to make the graph look more slick. Leave empty or set ChartRevealTime as 0.0f if you don't want to have your graph hidden beforehand.")]
    public RectTransform ChartHideRect;


    [Header("Area Chart Display Settings")]
    [Tooltip("Set the thickness of the lines being drawn on the top of each of the bars on the graph.")]
    [Range(0.0f, 100.0f)]
    public float LineThickness = 1.0f;

    [Tooltip("Allows you to set the buffer added to the top and bottom of the graph in terms of max and min y values.")]
    [Range(0.0f, 1.0f)]
    public float HeightLeewayPercentage = 0.0f;

    [Tooltip("Allows you to set the time it takes for the graph to be fully revealed when ChartHideRect is in place.")]
    [Range(0.0f, 3.0f)]
    public float ChartRevealTime = 1.0f;


    [Header("Area Chart Axis Settings")]
    [Tooltip("The number of x axis markers to be shown. Set to be greater than the number of elements being provided to ensure all are shown.")]
    [Range(2, 20)]
    public int XAxisElementNumber = 2;

    [Tooltip("The number of y axis markers to be shown (starting with maximum and minimum, and the rest evenly going between them.")]
    [Range(2, 20)]
    public int YAxisElementNumber = 2;

    [Tooltip("Buffer of space between GraphBounds bottom and X Axis markers.")]
    [Range(0, 250)]
    public int XAxisTextSpacing = 30;

    [Tooltip("Buffer of space between GraphBounds left side and Y Axis markers.")]
    [Range(0, 250)]
    public int YAxisTextSpacing = 30;


    //The width of the hidden chart rect
    private float m_ChartHideWidth = 0.0f;

    //Saved graph bars, points, and lines
    private List<VennGraph_AreaBar> m_graphBars = new List<VennGraph_AreaBar>();
    private List<VennGraph_Point> m_graphPoints = new List<VennGraph_Point>();
    private List<RectTransform> m_graphLines = new List<RectTransform>();


    private void Awake()
    {
        //Set up chart hide width if desired
        if(ChartHideRect != null)
            m_ChartHideWidth = ChartHideRect.sizeDelta.x;
    }


    //Add a single x/y value set to the line graph
    public override void AddSingleXAndYValueToGraph(string xVal_, float yVal_, string yValueLabel_ = "")
    {
        base.AddSingleXAndYValueToGraph(xVal_, yVal_, yValueLabel_);

        //Call to set up area chart 
        SetUpAreaChart(false);
    }


    //Set up graph with preset x and y values
    public override void SetUpGraphViaValues(string[] xVals_, float[] yVals_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaValues(xVals_, yVals_, yValueLabel_);

        //if there are less than 2 x or y values, add to them so that the area chart is able to display properly
        if (m_xSavedValues.Count == 1)
            m_xSavedValues.Add(m_xSavedValues[0]);

        if (m_ySavedValues.Count == 1)
            m_ySavedValues.Add(m_ySavedValues[0]);

        //Call to set up area chart
        SetUpAreaChart(false);
    }


    //Set up multi-line graph with preset x values and multiple y value arrays
    public override void SetUpGraphViaMultipleValueSets(string[] xVals_, List<float[]> yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        base.SetUpGraphViaMultipleValueSets(xVals_, yVals_, displayColors_, yValueLabel_);

        //Call to set up area chart
        SetUpAreaChart(true);
    }


    //Vectors that hold where the graphs are being placed so we can later set our x axis markers in parent class
    private List<Vector2> m_graphVectors = new List<Vector2>();


    //Display area chart graph bars, first determining if we're using multiple values sets (such as with a multi-set graph) or just a single value set
    private void SetUpAreaChart(bool multiValues_)
    {
        //Remove all previously made graph elements so we don't double up on our dispaly
        ClearOutGraphElements();

        //Clear out previously saved graph vectors
        m_graphVectors.Clear();

        //Set up initial minimum y and maximum y values
        float MinY = 0.0f;
        float MaxY = 0.0f;

        //Enter in our values and return them with the values we now have
        SetMinAndMaxYValues(out MinY, out MaxY, HeightLeewayPercentage);

        //Set up bar size based on axis width
        float xSize = (m_AxisWidth * (1.0f / (float)(m_xSavedValues.Count - 1)));

        //Determine if we're using multiple value sets or a single value set
        if(multiValues_)
        {
            //go through each set of lines that are going to be built out, as defined by the number of y values associated with each x value  
            for (int i = 0; i < m_multiSavedValues[m_xSavedValues[0]].Length; i++)
            {
                List<float> tempYs = new List<float>();

                //Create the set of y values associated with a single bar graph
                for (int j = 0; j < m_xSavedValues.Count; j++)
                {
                    float yValue = m_multiSavedValues[m_xSavedValues[j]][i];
                    tempYs.Add(yValue);
                }

                //build that individual graph and repeat process
                BuildChartWithYValues(MinY, MaxY, xSize, tempYs.ToArray(), i == 0, m_savedDisplayColors[i]);
            }
        }
        else
        {
            //Build with a single value set
            BuildChartWithYValues(MinY, MaxY, xSize, m_ySavedValues.ToArray(), true);
        }

        //Add one additional vector to the end of the graph
        m_graphVectors.Add(m_graphVectors[m_graphVectors.Count - 1] + new Vector2(xSize, 0));

        //Set up x and y axis markers
        SetXAxisMarkers(m_xSavedValues.ToArray(), m_graphVectors.ToArray(), XAxisElementNumber, XAxisTextSpacing);
        SetYAxisMarkers(MinY, MaxY, YAxisElementNumber, YAxisTextSpacing);

        //Determine if chart hide rect is available, then either set it to start revealing the area graph, or just set it to revealed
        if (ChartHideRect != null)
        {
            if(ChartRevealTime > 0.0f)
            {
                StopAllCoroutines();
                StartCoroutine(ShowAreaChartViaMask());
            }
            else
            {
                ChartHideRect.sizeDelta = new Vector2(0, ChartHideRect.sizeDelta.y);
            }
        }

        //Move all of our lines and points to the front of the graph so if they are layered, the bars appear in a readable manner
        MoveAllLinesAndPointsToFront();
    }


    //Build out the chart itself using the supplied y values
    private void BuildChartWithYValues(float minY_, float maxY_, float xSize_, float[] yVals_, bool addVectors_, Color col_ = new Color())
    {
        //Save previous y value so that the x position and ramp of the area graph bar can be adjusted accordingly
        float prevYValue = 0.0f;

        for (int i = 0; i < yVals_.Length; i++)
        {
            //Just set the previous value if it's the first value, as we need at least two values to set up the area graph bar
            if (i == 0)
            {
                prevYValue = yVals_[i];
                continue;
            }

            //Instantiate area bar with parent of GraphBounds
            VennGraph_AreaBar areaBar = Instantiate(AreaBarPrefab, GraphBounds) as VennGraph_AreaBar;

            //Set height of the bar's core and the topper based on the difference between the value and minimum (Main) and the difference between prevValue and minimum (Prev)
            float percentageAmntY_Main = ((yVals_[i] - minY_) / (maxY_ - minY_));
            float percentageAmntY_Prev = ((prevYValue - minY_) / (maxY_ - minY_));

            //Apply these heights to the actual size of the graph bounds so we can build out the graph itself
            float mainYHeight = m_AxisHeight * percentageAmntY_Main;
            float prevYHeight = m_AxisHeight * percentageAmntY_Prev;

            //Set siz and position of the bar and ramp topper
            areaBar.SetBarSizeAndPosition(xSize_, i, prevYHeight, mainYHeight, GraphBounds.rect.min);

            //If a color has been specified for this individual graph value set, set the bar and topper to be that color
            if (col_ != new Color())
                areaBar.SetBarColor(new Color(col_.r, col_.g, col_.b, 0.5f));

            //Set bar as last sibling and add to our saved graph bars list
            areaBar.transform.SetAsLastSibling();
            m_graphBars.Add(areaBar);

            //Set previous yValue as this one
            prevYValue = yVals_[i];

            //If we're still adding vectors, do so here
            if(addVectors_)
                m_graphVectors.Add(areaBar.transform.localPosition);

            //If we're past the first y value and we can build out lines, place a line atop of the bar
            if (i > 0 && AreaLinePrefab != null)
            {
                //Instantiate bar line with parent of GraphBounds
                RectTransform barLine = Instantiate(AreaLinePrefab, GraphBounds) as RectTransform;
                barLine.SetAsFirstSibling();

                //If a color has been specified for this graph, set it here
                if (col_ != new Color())
                    barLine.GetComponent<Image>().color = col_;

                //Set up point positions based on previous and main height established above
                Vector2 oldPos = new Vector2(m_graphVectors[i - 1].x, prevYHeight - (m_AxisHeight * 0.5f));
                Vector2 newPos = new Vector2(areaBar.transform.localPosition.x + xSize_, mainYHeight - (m_AxisHeight * 0.5f));

                //Place barline directly centered between each point
                barLine.transform.localPosition = Vector2.Lerp(oldPos, newPos, 0.5f);

                //Rotate barline to be facing with forward towards new point
                Vector3 startDir = oldPos - newPos;
                float angle = Mathf.Atan2(startDir.y, startDir.x) * Mathf.Rad2Deg;
                barLine.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

                //Find distance between these two points, and apply it to the scale/anchors of barline
                barLine.sizeDelta = new Vector2(LineThickness, Vector3.Distance(oldPos, newPos));

                m_graphLines.Add(barLine);
                barLine.SetAsLastSibling();

                //If points for lines are not null, set them up here
                if (AreaPointPrefab != null)
                {
                    VennGraph_Point barPoint = Instantiate(AreaPointPrefab, GraphBounds) as VennGraph_Point;
                    barPoint.transform.SetAsFirstSibling();
                    barPoint.transform.localPosition = newPos;

                    if(col_ != new Color())
                        barPoint.SetIconColor(col_);

                    m_graphPoints.Add(barPoint);
                }
            }
        }
    }


    //Ensures that all the lines and points are visible on the graph ahead of any other elements
    private void MoveAllLinesAndPointsToFront()
    {
        foreach(VennGraph_Point point in m_graphPoints)
            point.transform.SetAsLastSibling();

        foreach(RectTransform line in m_graphLines)
            line.transform.SetAsLastSibling();
    }


    //Remove all bars, points, and lines, then clear lists as well
    public override void ClearOutGraphElements()
    {
        base.ClearOutGraphElements();

        if (m_graphBars.Count > 0 || m_graphLines.Count > 0 || m_graphPoints.Count > 0)
        {
            foreach (VennGraph_AreaBar bar in m_graphBars)
                Destroy(bar.gameObject);

            foreach (RectTransform line in m_graphLines)
                Destroy(line.gameObject);

            foreach (VennGraph_Point point in m_graphPoints)
                Destroy(point.gameObject);
            
            m_graphBars.Clear();
            m_graphLines.Clear();
            m_graphPoints.Clear();
        }
    }


    //Show the area chart by adjusting the size of the ChartHideRect
    private IEnumerator ShowAreaChartViaMask()
    {
        float timePassed = 0.0f;
        float ySize = ChartHideRect.sizeDelta.y;

        //Set it to a starting point
        ChartHideRect.sizeDelta = new Vector2(m_ChartHideWidth, ySize);

        yield return new WaitForSeconds(0.5f);

        //After a delay, reveal chart point by point
        while (timePassed <= ChartRevealTime)
        {
            float percentSize = (timePassed/ChartRevealTime);

            if (percentSize > 1.0f)
                percentSize = 1.0f;

            ChartHideRect.sizeDelta = new Vector2(m_ChartHideWidth - (m_ChartHideWidth * percentSize), ySize);

            timePassed += Time.deltaTime;

            yield return null;
        }

        ChartHideRect.sizeDelta = new Vector2(0, ySize);
    }
}
