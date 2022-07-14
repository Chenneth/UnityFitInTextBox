using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//if someone has a better idea than creating a second set of functions for gui vs canvas tmp please 
public class TextMeshProFit : MonoBehaviour
{
    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The TextMesh Pro to be tested.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns>A list consisting of each line delimited by however many words can fit into the given TextMeshPro asset.</returns>
    public static List<string> MaxVerticalTextDisplay(TextMeshPro testText, string message)
    {
        return !message.Contains(" ") ? MaxCharDisplay(testText, message, testText.rectTransform.rect) : MaxVerticalWordDisplay(testText, message, testText.rectTransform.rect);
    }
   private static List<string> MaxVerticalWordDisplay(TextMeshPro testText, string message, Rect rect)
    {
        string currentString = "";
        string previousString = "";
        var strings = new List<string>(3);
        var rectHeight = rect.height;
        var rectWidth = rect.width;
        
        if (testText.GetPreferredValues("W", rectWidth,rectHeight).y > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        var words = HelperClass.WORD_MATCH.Matches(message).Select(m => m.Value).ToArray();

        //next we add the words to the strings

        for (var i = 0; i < words.Length; i++)
        {
            string curWord = words[i];
            float prefHeight;
            if (curWord.TryFindIndices(c => c is '\n' or '\r', out var indices))
            {
                int prevIndex = 0;
                foreach (int index in indices)
                {
                    currentString += curWord[prevIndex..index];
                    prefHeight = testText.GetPreferredValues(currentString, rectWidth, rectHeight).y;
                    if (prefHeight > rectHeight) //if the text cannot fit into the box
                    {
                        strings.Add(previousString);
                        currentString = previousString = curWord[prevIndex..index];
                    }
                    strings.Add(currentString);
                    prevIndex = index+1;//i think it needs +1 to ignore the \n or \r?
                    currentString = previousString = "";
                }

                currentString = previousString = curWord[prevIndex..];
            }
            else
            {
                currentString += curWord;
                prefHeight = testText.GetPreferredValues(currentString, rectWidth, rectHeight).y;
                if (prefHeight > rectHeight)
                {
                    strings.Add(previousString);
                    currentString = previousString = words[i];
                }
                else
                {
                    previousString = currentString;
                }
            }
        }

        if (Char.IsWhiteSpace(previousString[0]))
            previousString = previousString[1..];
        strings.Add(previousString);
        return strings;
    }

    private static List<string> MaxCharDisplay(TextMeshPro testText, string message, Rect rect)
    {
        var strings = new List<string>(3);
        var chars = message.ToCharArray();
        var rectHeight = rect.height;
        var rectWidth = rect.width;
        if (testText.GetPreferredValues("W", rectWidth,rectHeight).y > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        string s = "";
        foreach (char c in chars)
        {
            var prefHeight = testText.GetPreferredValues(s,rectWidth,rectHeight).y;
            if (prefHeight > rectHeight)
            {
                
                if (String.IsNullOrEmpty(s))
                {
                    throw new UnityException("Font size exceeds the maximum height the Text asset can display.");
                }
                if (Char.IsWhiteSpace(s[0]))
                    s = s[1..];
                strings.Add(s);
                s = c.ToString();
            }
            else
            {
                s += c;
            }
        }

        return strings;
    }
    
    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The TextMesh pro to be tested.</param>
    /// <param name="canvas">The Canvas <see cref="testText"/> is a child of.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns>A list consisting of each line delimited by however many words can fit into the given TextMeshPro asset.</returns>
    public static List<string> MaxVerticalTextDisplay(TextMeshProUGUI testText, Canvas canvas, string message)
    {
        return !message.Contains(" ") ? MaxCharDisplay(testText, message, testText.rectTransform.rect, canvas.scaleFactor) : MaxVerticalWordDisplay(testText, message, testText.rectTransform.rect, canvas.scaleFactor);
    }

    //punctuation is included in the strings (since ' ' is used as a delimiter).
 
    //punctuation is included in the strings (since ' ' is used as a delimiter).
    private static List<string> MaxVerticalWordDisplay(TextMeshProUGUI testText, string message, Rect rect,
        float canvasScaleFactor)
    {
        string currentString = "";
        string previousString = "";
        var rectHeight = rect.height * canvasScaleFactor;
        var rectWidth = rect.width * canvasScaleFactor;
        if (testText.GetPreferredValues("W", rectWidth,rectHeight).y > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        var strings = new List<string>(3);


        var words = HelperClass.WORD_MATCH.Matches(message).Select(m => m.Value).ToArray();

        //next we add the words to the strings

        for (var i = 0; i < words.Length; i++)
        {
            string curWord = words[i];
            float prefHeight;
            if (curWord.TryFindIndices(c => c is '\n' or '\r', out var indices))
            {
                int prevIndex = 0;
                foreach (int index in indices)
                {
                    currentString += curWord[prevIndex..index];
                    prefHeight = testText.GetPreferredValues(currentString, rectWidth, rectHeight).y;
                    if (prefHeight > rectHeight)
                    {
                        strings.Add(previousString);
                        currentString = previousString = curWord[prevIndex..index];
                    }
                    strings.Add(currentString);
                    prevIndex = index+1;
                    currentString = previousString = "";
                }
                
                currentString = previousString = curWord[prevIndex..];
            }
            else
            {
                currentString += curWord;
                prefHeight = testText.GetPreferredValues(currentString, rectWidth, rectHeight).y;
                if (prefHeight > rectHeight)
                {
                    strings.Add(previousString);
                    currentString = previousString = words[i];
                }
                else
                {
                    previousString = currentString;
                }
            }
        }
        strings.Add(previousString);
        return strings;
    }

    private static List<string> MaxCharDisplay(TextMeshProUGUI testText, string message, Rect rect,
        float canvasScaleFactor)
    {
        Debug.Log("MaxChar");
        var strings = new List<string>(3);
        var chars = message.ToCharArray();
        var rectHeight = rect.height * canvasScaleFactor;
        var rectWidth = rect.width * canvasScaleFactor;
        if (testText.GetPreferredValues("W", rectWidth,rectHeight).y > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        string s = "";
        foreach (char c in chars)
        {
            var prefHeight = testText.GetPreferredValues(s,rectWidth,rectHeight).y;
            if (prefHeight > rectHeight)
            {
                if (Char.IsWhiteSpace(s[0]))
                    s = s[1..];
                strings.Add(s);
                s = c.ToString();
            }
            else
            {
                s += c;
            }
        }

        return strings;
    }
    
    /*private static bool IsCnJp(char c)
    {
        //0x2E00-0x312F
        //0x3190-0x4DBF
        //0x4E00-0x9FFF

        if (c >= 0x2E00)
        {
            if (c <= 0x312F) return true;
            if (c >= 0x3190)
            {
                if (c <= 0x4DBF) return true;
                if (c >= 0x4E00 && c <= 0x9FFF)
                {
                    return true;
                }
            }
        }

        return false;
    }*/
}