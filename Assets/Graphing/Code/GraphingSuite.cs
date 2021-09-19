//          [VENN INTERACTIVE GRAPHING SUITE]
//  Created by Samson Jinks
//  Copyright (©) 2019 Venn Interactive Incorporated, LLC

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphingSuite : MonoBehaviour
{
    [Header("Test Graph Settings")]
    [Tooltip("Select between the four graph types this package provides: Bar, Line, Area, and Pie.")]
    public TestGraphType MyGraphType;

    [Tooltip("Select how many value groups you'd like to have on this example graph. (ONLY USED WITH LINE AND AREA GRAPHS)")]
    public TestGraphSetNum GraphSetNum;

    [Tooltip("Select how many X,Y data sets you'd like the example graph to display.")]
    [Range(2, 100)]
    public int TotalGraphPoints = 5;

    //These are the various types of graphs we have available
    [Header("Test Graph Examples")]
    [Tooltip("Bar Chart Example: Useful for representing values that should be directly compared to each other.")]
    public VennGraph_BarChart BarChartEx;

    [Tooltip("Line Graph Example: Useful for representing values as they occur chronologically.")]
    public VennGraph_LineChart LineGraphEx;

    [Tooltip("Area Graph Example: Useful for representing values chronologically, especially with multiple y value sets.")]
    public VennGraph_AreaChart AreaChartEx;

    [Tooltip("Pie Chart Example: Useful for representing values in regards to their ratio to the total of all the values input.")]
    public VennGraph_PieChart PieChartEx;

	void Start ()
    {
        ActivateGraphTest();
	}

    protected void ActivateGraphTest()
    {
        BarChartEx.gameObject.SetActive(MyGraphType == TestGraphType.Bar);
        LineGraphEx.gameObject.SetActive(MyGraphType == TestGraphType.Line);
        AreaChartEx.gameObject.SetActive(MyGraphType == TestGraphType.Area);
        PieChartEx.gameObject.SetActive(MyGraphType == TestGraphType.PieChart);

        if (MyGraphType == TestGraphType.Bar)
            TestBarGraph();
        else if (MyGraphType == TestGraphType.Line)
            TestLineGraph();
        else if (MyGraphType == TestGraphType.Area)
            TestAreaGraph();
        else if (MyGraphType == TestGraphType.PieChart)
            TestPieChart();
    }

    protected void TestBarGraph()
    {
        string[] xVals = new string[TotalGraphPoints];
        float[] yVals = new float[TotalGraphPoints];
        Color[] cols = new Color[TotalGraphPoints];

        for (int i = 0; i < xVals.Length; i++)
        {
            xVals[i] = (i + 1).ToString();
            yVals[i] = Random.Range(0.0f, 250.0f);
            cols[i] = Random.ColorHSV();
        }

        BarChartEx.SetAxisTitlesAndBounds("Day", "Hours");
        BarChartEx.SetUpGraphViaValues(xVals, yVals, cols, "Hrs");
    }

    protected void TestLineGraph()
    {
        if (GraphSetNum == TestGraphSetNum.OneSet)
            TestLineGraph_OneSet();
        else if (GraphSetNum == TestGraphSetNum.TwoSets)
            TestLineGraph_MultiSet(2);
        else if (GraphSetNum == TestGraphSetNum.ThreeSets)
            TestLineGraph_MultiSet(3);
    }

    protected void TestLineGraph_OneSet()
    {
        string[] xVals = new string[TotalGraphPoints];
        float[] yVals = new float[TotalGraphPoints];

        for (int i = 0; i < xVals.Length; i++)
        {
            xVals[i] = (i + 1).ToString();
            yVals[i] = Random.Range(0.0f, 250.0f);
        }

        LineGraphEx.SetAxisTitlesAndBounds("Day", "Weight");
        LineGraphEx.SetUpGraphViaValues(xVals, yVals, "Lbs");
    }

    protected void TestLineGraph_MultiSet(int num_)
    {
        string[] xVals = new string[TotalGraphPoints];

        List<float[]> yValSets = new List<float[]>();

        for (int i = 0; i < xVals.Length; i++)
            xVals[i] = (i + 1).ToString();

        for (int i = 0; i < xVals.Length; i++)
        {
            float[] yVals = new float[num_];

            for (int j = 0; j < yVals.Length; j++)
                yVals[j] = Random.Range(0.0f, 250.0f);

            yValSets.Add(yVals);
        }

        Color[] cols = new Color[num_];

        for (int i = 0; i < cols.Length; i++)
            cols[i] = Random.ColorHSV();

        LineGraphEx.SetAxisTitlesAndBounds("Day", "Weight");
        LineGraphEx.SetUpGraphViaMultipleValueSets(xVals, yValSets, cols);
    }

    protected void TestAreaGraph()
    {
        if (GraphSetNum == TestGraphSetNum.OneSet)
            TestAreaGraph_OneSet();
        else if (GraphSetNum == TestGraphSetNum.TwoSets)
            TestAreaGraph_MultiSets(2);
        else
            TestAreaGraph_MultiSets(3);
    }

    private void TestAreaGraph_OneSet()
    {
        string[] xVals = new string[TotalGraphPoints];
        float[] yVals = new float[TotalGraphPoints];

        for (int i = 0; i < xVals.Length; i++)
        {
            xVals[i] = (i + 1).ToString();
            yVals[i] = Random.Range(0.0f, 250.0f);
        }

        AreaChartEx.SetAxisTitlesAndBounds("Day", "Profit");
        AreaChartEx.SetUpGraphViaValues(xVals, yVals);
    }

    protected void TestAreaGraph_MultiSets(int num_)
    {
        string[] xVals = new string[TotalGraphPoints];

        List<float[]> yValSets = new List<float[]>();

        for (int i = 0; i < xVals.Length; i++)
            xVals[i] = (i + 1).ToString();

        for (int i = 0; i < xVals.Length; i++)
        {
            float[] yVals = new float[num_];

            for (int j = 0; j < yVals.Length; j++)
                yVals[j] = Random.Range(0.0f, 250.0f);

            yValSets.Add(yVals);
        }

        Color[] cols = new Color[num_];

        for (int i = 0; i < cols.Length; i++)
            cols[i] = Random.ColorHSV();

        AreaChartEx.SetAxisTitlesAndBounds("Day", "Profit");
        AreaChartEx.SetUpGraphViaMultipleValueSets(xVals, yValSets, cols);
    }

    protected void TestPieChart()
    {
        string[] xVals = new string[TotalGraphPoints];
        float[] yVals = new float[TotalGraphPoints];
        Color[] cols = new Color[TotalGraphPoints];

        for (int i = 0; i < xVals.Length; i++)
        {
            xVals[i] = "Slice #" + (i + 1).ToString();
            yVals[i] = Random.Range(0.0f, 250.0f);
            cols[i] = Random.ColorHSV();
            cols[i] = new Color(cols[i].r, cols[i].g, cols[i].b, 0.65f);
        }

        PieChartEx.SetAxisTitlesAndBounds("Days", "Avg. Tickets Sold");
        PieChartEx.SetUpGraphViaValues(xVals, yVals, cols, "Tix");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(GraphSetNum == TestGraphSetNum.OneSet || MyGraphType == TestGraphType.PieChart || MyGraphType == TestGraphType.Bar)
            {
                if (MyGraphType == TestGraphType.Bar)
                    BarChartEx.AddSingleValueSetToGraph((BarChartEx.GetTotalXValues() + 2).ToString(), Random.Range(0.0f, 250.0f), Random.ColorHSV());
                else if (MyGraphType == TestGraphType.Line)
                    LineGraphEx.AddSingleXAndYValueToGraph((LineGraphEx.GetTotalXValues() + 2).ToString(), Random.Range(0.0f, 250.0f));
                else if (MyGraphType == TestGraphType.Area)
                    AreaChartEx.AddSingleXAndYValueToGraph((AreaChartEx.GetTotalXValues() + 2).ToString(), Random.Range(0.0f, 250.0f));
                else if (MyGraphType == TestGraphType.PieChart)
                    PieChartEx.AddSingleValueSetToGraph("Slice #" + (PieChartEx.GetTotalXValues() + 2).ToString(), Random.Range(0.0f, 150.0f), Random.ColorHSV());   
            }
            else
            {
                Debug.LogError("ERROR: You cannot add individual value sets to this graph.");

                if (MyGraphType == TestGraphType.Area || MyGraphType == TestGraphType.Line)
                    Debug.LogError("You can only add individual sets to graphs that have one set of x and y values. You'll need to change all the values via SetUpGraphViaMultiValues to update them.");
                else
                    Debug.LogError("You will need to set your GraphSetNum to OneSet if you want to be able to add individual x and y values.");
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            MyGraphType = (TestGraphType)Random.Range(0, 4);
            GraphSetNum = (TestGraphSetNum)Random.Range(0, 3);
            TotalGraphPoints = Random.Range(2, 20);

            ActivateGraphTest();
        }
    }
}

public enum TestGraphType
{
    Bar,
    Line,
    Area,
    PieChart,
}

public enum TestGraphSetNum
{
    OneSet,
    TwoSets,
    ThreeSets
}
