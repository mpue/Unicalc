using Assets;
using NCalc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shell : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    
    private string buffer = "";

    private int row = 0;
    private int col = 0;

    private List<string> buffers;
    const char NewLine = '\n';
    const char CarriageReturn = '\r';
    const char BackSpace = '\b';

    private float currentTime = 0;
    public float cursorBlinkInterval = 0.5f;
    private bool showCursor = false;

    public ScrollRect scrollRect;

    public void Clear()
    {
        buffers = new List<string>();
        buffers.Add("");
        row = 0;
    }

    void Start()
    {
        Clear();
    }

    public void AddCommand(string command, bool useInterpreter = true)
    {
        buffers.Add(command);
        row++;

    }

    public void SetText(string input)
    {
        buffers[row] = input;
        col = input.Length;
    }

    public void AddText(string input)
    {

        buffers[row] += input;
        col += input.Length;
    }

    public void Delete()
    {
        if (col > 0)
        {
            col--;
            if (buffers[row].Length > 0)
            {
                buffers[row] = buffers[row].Substring(0, buffers[row].Length - 1);
            }
        }

    }

    public void Enter()
    {
        AddCommand(buffers[row]);
        scrollRect.verticalScrollbar.value = 1f;
        buffers.Add("");
        row++;

    }

    void Update()
    {        

        buffer = "";
        for (int i = 0; i < buffers.Count; i++)
        {
            if (i >= buffers.Count - 1)
            {
                buffer += ">";
                buffer += buffers[i];
            }
            else
            {
                buffer += " " + buffers[i] + "\n";
            }
        }

        currentTime += Time.deltaTime;

        if (currentTime > cursorBlinkInterval)
        {
            currentTime = 0;
            showCursor = !showCursor;
        }

        if (showCursor)
        {
            buffer += "_";
        }

        text.text = buffer;
    }

    public string GetCurrentBuffer()
    {
        return buffers[row];
    }

    public void SetCurrentBuffer(string text)
    {
        buffers[row] = text;
    }

    private static bool IsPrintable(char c)
    {
        return !IsNewLine(c) && (
            char.IsLetterOrDigit(c) ||
            char.IsWhiteSpace(c) && !IsNewLine(c) ||
            char.IsPunctuation(c) ||
            char.IsSymbol(c));
    }

    private static bool IsNewLine(char c)
    {
        return c == NewLine || c == CarriageReturn;
    }
}
