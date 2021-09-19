using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DemoPlot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// plot(sin(x),0,360))
    /// </summary>
    /// <param name="expression"></param>

    public void Plot(string expression)
    {
        string value = Regex.Match(expression, @"(?<=\().+?(?=\))").Value;
        Debug.Log(value);
    }

}
