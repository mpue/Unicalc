using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Assets;

public class Calculator : MonoBehaviour
{
    private float value;

    private Dictionary<string, string> parameters = new Dictionary<string, string>();

    string currentValue = "";

    private string memory = "";
    private string lastAnswer = "";

    bool isClear = true;

    public TextMeshProUGUI drgText;
    public TextMeshProUGUI baseText;
    public TextMeshProUGUI storeText;

    public GameObject helpPanel;
    public Shell shell;

    private enum Base
    {
        DECIMAL,
        BINARY,
        HEX
    }

    private Base _currentBase = Base.DECIMAL;

    private void Start()
    {
        Application.targetFrameRate = 25;
        helpPanel.SetActive(false);
        storeText.text = "";
    }

    public void addToken(string token)
    {

        if (isClear)
        {
            currentValue = "";
            isClear = false;
        }

        currentValue += token;
        shell.AddText(token);
    }

    public void Zero()
    {
        isClear = true;
        value = 0;
        currentValue = "";
        shell.Clear();
        
    }

    public void Delete()
    {
        shell.Delete();        
    }

    public void Execute()
    {
        if (shell.GetCurrentBuffer().Length > 0)
        {
            Evaluate(shell.GetCurrentBuffer());
        }
        currentValue = "";
    }

    public void ShowHelp()
    {
        helpPanel.SetActive(true); 
    }

    public void OpenKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true, false);
    }

    public void Sinus()
    {
        addToken("Sin(");
    }

    public void Cosinus()
    {
        addToken("Cos(");
    }
    public void Tangens()
    {
        addToken("tan(");
    }

    public void PowXY()
    {
        addToken("Pow(");
    }

    public void Invert()
    {
        addToken("1/");
    }

    public void Log10()
    {
        addToken("Log10(");
    }

    public void Plot()
    {
        addToken("plot(");
    }

    public void Pi()
    {
        addToken(Mathf.PI.ToString().Replace(",", "."));
    }

    public void Sqrt()
    {
        addToken("Sqrt(");
    }

    public void Ans()
    {
        currentValue = lastAnswer;
        addToken(currentValue);
    }

    public void Store()
    {
        memory = currentValue;
        storeText.text = "RCL";
    }

    public void Recall()
    {
        if (!isClear)
        {
            currentValue += memory;
        }
        else
        {
            currentValue = memory;
            isClear = false;
        }
        addToken(currentValue);
    }

    public void ClearMemory()
    {
        memory = "";
        storeText.text = "";
    }

    public void Decimal()
    {
        baseText.text = "DEC";
        SwitchToDecimal(_currentBase);
        _currentBase = Base.DECIMAL;
    }

    public void Hex()
    {
        baseText.text = "HEX";
        SwitchToHex(_currentBase);
        _currentBase = Base.HEX;
    }

    public void Binary()
    {
        baseText.text = "BIN";
        SwitchToBinary(_currentBase);
        _currentBase = Base.BINARY;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Execute();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Delete();
        }
        else
        {
            if (Input.inputString.Length > 0)
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    currentValue = Input.inputString;
                    shell.SetText(currentValue);
                }
                else
                {
                    addToken(Input.inputString);
                }

            }
        }
    }

    private void HandleException(Exception e)
    {
        shell.AddCommand("what?");
        shell.AddCommand("");
    }

    private void SwitchToBinary(Base b)
    {
        if (currentValue == "0.")
        {
            return;
        }
        try
        {
            switch (b)
            {
                case Base.DECIMAL:
                    if (shell.GetCurrentBuffer().Length > 0)
                    {
                        int value = int.Parse(shell.GetCurrentBuffer());
                        shell.AddCommand(Dec2Bin(value));
                        shell.AddCommand("");   
                    }
                    break;
                case Base.BINARY:
                    break;
                case Base.HEX:
                    if (shell.GetCurrentBuffer().Length > 0)
                    {                       
                        shell.AddCommand(Dec2Bin(Hex2Dec(shell.GetCurrentBuffer())));
                        shell.AddCommand("");
                    }                    
                    break;
            }

        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    private void SwitchToHex(Base b)
    {
        if (currentValue == "0.")
        {
            return;
        }
        try
        {
            switch (b)
            {
                case Base.DECIMAL:
                    if (shell.GetCurrentBuffer().Length > 0)
                    {
                        int value = int.Parse(shell.GetCurrentBuffer());
                        shell.AddCommand(Dec2Hex(value));
                        shell.AddCommand("");
                        
                    }
                    break;
                case Base.BINARY:
                    if (shell.GetCurrentBuffer().Length > 0)
                    {
                        shell.AddCommand(Dec2Hex(BinToDec(shell.GetCurrentBuffer())));
                        shell.AddCommand("");
                    }
                    break;
                case Base.HEX:
                    break;
            }
        }
        catch (Exception e)
        {
            HandleException(e);
        }

    }

    private void SwitchToDecimal(Base b)
    {
        if (currentValue == "0.")
        {
            return;
        }
        try
        {
            switch (b)
            {
                case Base.DECIMAL:
                    break;
                case Base.BINARY:
                    shell.AddCommand(BinToDec(shell.GetCurrentBuffer()).ToString());
                    shell.AddCommand("");
                    break;
                case Base.HEX:
                    shell.AddCommand(Hex2Dec(shell.GetCurrentBuffer()).ToString());
                    shell.AddCommand("");
                    break;
            }
        }
        catch (Exception e)
        {
            HandleException(e);
        }


    }

    private static int BinToDec(String bin)
    {
        if (bin.Length > 0)
        {
            return Convert.ToInt32(bin, 2);
        }
        return 0;
    }
    private static string Dec2Bin(int value)
    {
        return Convert.ToString(value, 2);
    }

    private static int Hex2Dec(string hex)
    {
        return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
    }

    private static string Dec2Hex(int value)
    {
        return value.ToString("x");
    }

    public void ClearVariables()
    {
        parameters.Clear();
        shell.AddCommand("parameters cleared.");
        shell.AddCommand("");
    }

    private void Evaluate(string s)
    {
        if (s.Contains("="))
        {
            string assignment = s;
            string[] tokens = assignment.Split('=');
            if (parameters.ContainsKey(tokens[0]))
            {
                parameters[tokens[0]] = tokens[1];
            }
            else
            {
                parameters.Add(tokens[0], tokens[1]);
            }
            shell.AddCommand(tokens[0] + "=" + tokens[1]);
        }
        else
        {
            try
            {

                switch (_currentBase)
                {
                    case Base.DECIMAL:
                        break;
                    case Base.BINARY:
                        string[] numbers = Regex.Split(s, @"\D+");
                        foreach(string num in numbers)
                        {
                            s = s.ReplaceNthOccurrence(num, BinToDec(num).ToString(),1);
                        }
                        break;
                    case Base.HEX:
                        MatchCollection tokens = Regex.Matches(s, @"0[xX][0-9a-fA-F]+");
                        int group = 0;
                        
                        foreach(Match m in tokens)
                        {
                            s = s.ReplaceNthOccurrence(m.Value, Hex2Dec(m.Value.Substring(2)).ToString(), 1);
                            group++;
                        }
                        //foreach (string num in tokens)
                        //{
                        //}
                        break;
                }

                Expression e = new Expression(s);
                foreach (string key in parameters.Keys)
                {
                    e.Parameters[key] = float.Parse(parameters[key], CultureInfo.InvariantCulture.NumberFormat);
                }

                string value = e.Evaluate().ToString();
                lastAnswer = value;

                switch (_currentBase)
                {
                    case Base.DECIMAL:
                        break;
                    case Base.BINARY:

                        
                        value = Dec2Bin(int.Parse(value));
                        break;
                    case Base.HEX:
                        value = "0x"+Dec2Hex(int.Parse(value));

                        break;
                }

                shell.AddCommand(currentValue + " = " + value);
                shell.AddCommand("");


            }
            catch (Exception e)
            {
                HandleException(e);
            }

        }
    }



}
