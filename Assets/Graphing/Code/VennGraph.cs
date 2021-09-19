//          [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VennGraph : MonoBehaviour
{
    [Header("Basic Graph Elements")]
    [Tooltip("The boundaries of the graph itself. Should indicate the full rect that you want the graph to fill.")]
    public RectTransform GraphBounds;
    [Tooltip("Prefab that is used to place text on the axis of your graph, showing x and y values (only used with y values on Pie Charts).")]
    public Text AxisValueTextPrefab;
    [Tooltip("Text used for title of the X Axis, indicating the name of the x values on the graph.")]
    public Text AxisTitle_X;
    [Tooltip("Text used for title of the Y Axis, indicating the name of hte y values on the graph (or the title of the chart Legend on Pie Charts).")]
    public Text AxisTitle_Y;

    //The width of the graph boundaries
    protected float m_AxisWidth = 0.0f;
    //The height of the graph boundaries
    protected float m_AxisHeight = 0.0f;

    //The saved x and y values of the graph, used when only using one set of values
    protected List<string> m_xSavedValues = new List<string>();
    protected List<float> m_ySavedValues = new List<float>();

    //The saved dicitonary of x and y values on the graph, used when using multiple sets of values (such as on a multi-line graph)
    protected Dictionary<string, float[]> m_multiSavedValues = new Dictionary<string, float[]>();

    //The saved colors that are used when displaying pie slices, bars, and multiple sets of line or area graphs
    protected List<Color> m_savedDisplayColors = new List<Color>();

    //The saved label associated with your y values (such as "Lbs." or "Dollars")
    protected string m_savedYValueLabel = "";

    //The saved set of text objects that make up your x and y axis markers
    private List<Text> m_xAxisMarkers = new List<Text>();
    private List<Text> m_yAxisMarkers = new List<Text>();


    //Set the title of your two x and y axis, and then determine the height and width of your graph's bounds
    public void SetAxisTitlesAndBounds(string xAXis_, string yAxis_)
    {
        //Set titles based on xAxis_ and yAxis_ strings
        AxisTitle_X.text = xAXis_;
        AxisTitle_Y.text = yAxis_;

        //Set the width and height based on the rect size of GraphBounds
        m_AxisWidth = Mathf.Abs(GraphBounds.rect.xMax - GraphBounds.rect.xMin);
        m_AxisHeight = Mathf.Abs(GraphBounds.rect.yMax - GraphBounds.rect.yMin);
    }


    //Set the x axis markers so they're lined up with their respective y values on the graph
    public void SetXAxisMarkers(string[] xValues_, Vector2[] axisPoints_, int marksToShow_, int axisTextSpacing_)
    {
        if (AxisValueTextPrefab == null)
        {
            Debug.LogError("NOTE: You have not set up AxisValueTextPrefab on this Graph. No X Axis markers will be displayed.");
            return;
        }

        //Modifier depending on whether both the values given and the marks request are both even or odd
        int markMod = 0;

        if (marksToShow_ % 2 != xValues_.Length % 2)
            markMod = 1;

        //Reduce the number of marsk to show if the marks to show are larger than the actual length of the values being provided
        if (xValues_.Length < marksToShow_)
            marksToShow_ = xValues_.Length;

        //Find which values we're actually going to show on the graph, starting with the first and last one, which we know we'll need to be shown
        List<int> markIndexes = new List<int>();

        markIndexes.Add(0);
        markIndexes.Add(xValues_.Length - 1);

        //Check if there are any more marks to show than the the ones represented here
        if(marksToShow_ > 2)
        {
            float indexCheck = (float)(xValues_.Length - 1) / (float)(marksToShow_ - markMod);

            for (int i = 1; i < marksToShow_; i++)
                markIndexes.Add((int)(indexCheck * i));
        }

        //Set up the x axis markers
        for (int i = 0; i < markIndexes.Count; i++)
        {
            int markIndex = markIndexes[i];

            //Instantiate text object, parented to GraphBounds, and mark it equal to the xValue desired
            Text axisText = Instantiate(AxisValueTextPrefab, GraphBounds) as Text;
            axisText.text = xValues_[markIndex];

            //Set the position of the marker itself inside of GraphBounds
            axisText.transform.localPosition = new Vector2(axisPoints_[markIndex].x, (GraphBounds.rect.yMin - axisTextSpacing_));

            //Add to the xAxis Markers list so if we rebuild this graph, we can remove them later in ClearOutGraphElements
            m_xAxisMarkers.Add(axisText);
        }
    }


    //Set the y axis markers so that the user has an indicative value on the graph regarding how the value of each set (with at least the min and max being shown)
    public void SetYAxisMarkers(float yMin_, float yMax_, int marksToShow_, int axisTextSpacing_)
    {
        if (AxisValueTextPrefab == null)
        {
            Debug.LogError("NOTE: You have not set up AxisValueTextPrefab on this Graph. No Y Axis markers will be displayed.");
            return;
        }

        //Go through all the marks that are going to be shown
        for (int i = 0; i < marksToShow_; i++)
        {
            //Find how high up the graph this mark should be shown, then apply it to the total given
            float percentageAmnt = (((float)i / (float)(marksToShow_ - 1)));
            float axisAmnt = ((yMax_ - yMin_) * percentageAmnt) + yMin_;

            //Instantiate the text prefab with the parent of GraphBounds
            Text axisText = Instantiate(AxisValueTextPrefab, GraphBounds) as Text;

            //If axis amount to be shown is greater than 0, make sure it is shown as a whole number for ease of viewability
            if (axisAmnt >= 0.0f)
                axisText.text = RoundToFloat(axisAmnt, false).ToString();
            else
                axisText.text = axisAmnt.ToString("######");

            //Set the position of the marker itself inside of GraphBounds
            axisText.transform.localPosition = new Vector2(GraphBounds.rect.xMin - axisTextSpacing_, GraphBounds.rect.yMin + (m_AxisHeight * percentageAmnt));

            //Add to the yAxis Markers list so if we rebuild this graph, we can remove them later in ClearOutGraphElements
            m_yAxisMarkers.Add(axisText);
        }
    }


    //Set up the minimum x and y values that will be used to determine the position/size of each value on the graph
    public void SetMinAndMaxYValues(out float yMin_, out float yMax_, float heightLeeWay_)
    {
        //Check if we're looking through just a single set of values or multiple values sets (such as on a multi-line graph)
        if(m_multiSavedValues.Count > 0)
        {
            //Set up as default of 0
            yMin_ = 0.0f;
            yMax_ = 0.0f;

            //Go through all the values in the dictionary and determine which is the largest and smallest
            for (int i = 0; i < m_xSavedValues.Count; i++)
            {
                for (int j = 0; j < m_multiSavedValues[m_xSavedValues[i]].Length; j++)
                {
                    float check = m_multiSavedValues[m_xSavedValues[i]][j];

                    if (check > yMax_)
                        yMax_ = check;
                    else if (check < yMin_)
                        yMin_ = check;
                }
            }
        }
        else
        {
            //Set up as default value of the first value in the set
            yMin_ = m_ySavedValues[0];
            yMax_ = m_ySavedValues[0];   

            //Go through all the values in the single set of y values being used
            for (int i = 0; i < m_ySavedValues.Count; i++)
            {
                if (m_ySavedValues[i] > yMax_)
                    yMax_ = m_ySavedValues[i];
                else if (m_ySavedValues[i] < yMin_)
                    yMin_ = m_ySavedValues[i];
            }
        }

        //If leeway percentage has been set to be greater than 0.0, put a buffer between minimum and maximum equal to the total value between the two of them
        if (heightLeeWay_ > 0.0f)
        {
            //Determine the amount that buffer provides
            float leeway = (yMax_ - yMin_) * heightLeeWay_;

            //Apply that buffer to the maximum
            yMax_ += leeway;

            //Apply that buffer to the minumum if applicable
            if ((yMin_ - (leeway)) > 0.0f || yMin_ < 0.0f)
                yMin_ -= (leeway);
            else
                yMin_ = 0.0f;
        }
        else if(yMin_ > 0.0f)
        {
            //Set to 0 if height leeway is equal to 0, and the yMin itself is equal to or greater than 0 as well. Leave as is if yMin is a negative value
            yMin_ = 0.0f;
        }
    }


    //Add a single x and y value to your graph (useful when showing changes in data on the fly or when the user makes changes they want recorded)
    public virtual void AddSingleXAndYValueToGraph(string xVal_, float yVal_, string yValueLabel_ = "")
    {
        if(m_multiSavedValues.Count > 0)
        {
            Debug.LogError("ERROR: You cannot add to a graph with multiple y value sets using this method. In order to access this method, do not use SetUpGraphViaMultiValues beforehand.");
            return;    
        }

        //Set up the saved y label of the graph
        if (yValueLabel_ != "")
            m_savedYValueLabel = yValueLabel_;

        //Add x and y value to then end of our existing lists
        m_xSavedValues.Add(xVal_);
        m_ySavedValues.Add(yVal_);
    }


    //Add values to the graph, as well as with the color type (used with bar graphs and pie charts)
    public virtual void AddSingleValueSetToGraph(string xVal_, float yVal_, Color displayColor_, string yValueLabel_ = "")
    {
        AddSingleXAndYValueToGraph(xVal_, yVal_, yValueLabel_);

        m_savedDisplayColors.Add(displayColor_);
    }


    //Set up graphs via preset x and y values (used with Area Chart and Line Chart)
    public virtual void SetUpGraphViaValues(string[] xVals_, float[] yVals_, string yValueLabel_ = "")
    {
        if (xVals_.Length != yVals_.Length || yVals_.Length == 0)
        {
            Debug.LogError("ERROR: Check your arrays to be sure the same number of elements are in both.");
            Debug.LogError("X Value Elements: " + xVals_.Length + ", Y Value Elements: " + yVals_.Length);
            return;
        }

        //Set up the saved y label of the graph
        if (yValueLabel_ != "")
            m_savedYValueLabel = yValueLabel_;

        //Clear out all of the values that have been saved previously
        m_xSavedValues.Clear();
        m_ySavedValues.Clear();
        m_multiSavedValues.Clear();

        //Add range of x Values and y Values to saved lists
        m_xSavedValues.AddRange(xVals_);
        m_ySavedValues.AddRange(yVals_);
    }


    //Set up graphs via preset x, y, and color values (used with Bar Graph and Pie Chart)
    public virtual void SetUpGraphViaValues(string[] xVals_, float[] yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        //Apply x and y values
        SetUpGraphViaValues(xVals_, yVals_, yValueLabel_);

        //Clear out saved colors, then add range to saved color list
        m_savedDisplayColors.Clear();
        m_savedDisplayColors.AddRange(displayColors_);
    }


    //Set up a graph with multiple value sets (such as a multi-line graph or multiset area chart). This requires a single array of x values and multiple arrays of y values associated with each x value
    public virtual void SetUpGraphViaMultipleValueSets(string[] xVals_, List<float[]> yVals_, Color[] displayColors_, string yValueLabel_ = "")
    {
        if (xVals_.Length != yVals_.Count || yVals_.Count == 0)
        {
            Debug.LogError("ERROR: Check your arrays to be sure the same number of elements are in both.");
            Debug.LogError("X Value Elements: " + xVals_.Length + ", Y Value Elements: " + yVals_.Count);
            return;
        }

        //Set up the saved y label of the graph
        if (yValueLabel_ != "")
            m_savedYValueLabel = yValueLabel_;

        //Clear out our saved x, y, and color values
        m_xSavedValues.Clear();
        m_ySavedValues.Clear();
        m_multiSavedValues.Clear();
        m_savedDisplayColors.Clear();

        //Add to the saved range of x values and colors
        m_xSavedValues.AddRange(xVals_);
        m_savedDisplayColors.AddRange(displayColors_);

        //Go through x values and add them to the dictionary, along with each array of y values
        for (int i = 0; i < m_xSavedValues.Count; i++)
        {
            if (!m_multiSavedValues.ContainsKey(m_xSavedValues[i]))
            {
                m_multiSavedValues.Add(m_xSavedValues[i], yVals_[i]);
            }
            else
            {
                Debug.LogError("ERROR: You have entered an X value that is the same as a previous x value in this listing. Please check your values again and change this for the optimum experience.");
                Debug.LogError("Multiple value sets are stored via a dictionary, which requires an x value that is unique to that value in order to work properly.");
            }
        }
    }


    //Remove all x and y axis markers
    public virtual void ClearOutGraphElements()
    {
        if(m_xAxisMarkers.Count > 0)
        {
            foreach (Text xText in m_xAxisMarkers)
                Destroy(xText.gameObject);
        }

        if(m_yAxisMarkers.Count > 0)
        {
            foreach (Text yText in m_yAxisMarkers)
                Destroy(yText.gameObject);   
        }

        m_xAxisMarkers.Clear();
        m_yAxisMarkers.Clear();
    }


    //Round to 0.5f or 1.0f
    public float RoundToFloat(float roundNum_, bool halfFloat)
    {
        float roundedVal = Mathf.Floor(roundNum_ * 10);
        float mod = roundedVal % 10;

        roundedVal -= mod;

        if (mod >= 5.1f)
            roundedVal += 10;
        else if (mod >= 2.5f && halfFloat)
            roundedVal += 5;

        if (roundedVal / 10 < 0)
            return 0;

        return roundedVal / 10;
    }

    public int GetTotalXValues()
    {
        if (m_xSavedValues.Count > 0)
            return m_xSavedValues.Count;

        return 0;
    }
}