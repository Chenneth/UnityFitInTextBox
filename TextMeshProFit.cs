using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextMeshProFit : MonoBehaviour
{
    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The text box to be tested.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns>A list consisting of each line delimited by however many words can fit into the given TextMeshPro asset.</returns>
    public static List<string> MaxVerticalTextDisplay(TextMeshPro testText, string message)
    {
        var rect = testText.rectTransform.rect;
        var rectHeight = rect.height;

        if (message.All(IsCnJp)) //if all the chars are cn/jp
        {
            //then we do diff algorithm (yay)
            //MaxCnJpDisplay(message, textGenerator, generationSettings, rectHeight);
        }

        //if the given message does not contain any spaces todo: check for JP/CN chars (since I don't know anything about KR/AR etc.) to delimit it in its own algorithm, 
        if (!message.Contains(" "))
        {
            //then we will run through each character instead and return the string which has as many characters within it as possible
            //Without overflowing the textbox height
            return MaxCharDisplay(testText, message, rect);
        }

        return MaxVerticalWordDisplay(testText, message, rect);
    }

    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The text box to be tested.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns>A list consisting of each line delimited by however many words can fit into the given TextMeshPro asset.</returns>
    public static List<string> MaxVerticalTextDisplay(TextMeshProUGUI testText, Canvas canvas, string message)
    {
        var rect = testText.rectTransform.rect;

        if (message.All(IsCnJp)) //if all the chars are cn/jp
        {
            //then we do diff algorithm (yay)
        }

        //if the given message does not contain any spaces 
        if (!message.Contains(" "))
        {
            //then we will run through each character instead and return the string which has as many characters within it as possible
            //Without overflowing the textbox height
            return MaxCharDisplay(testText, message, rect, canvas.scaleFactor);
        }

        return MaxVerticalWordDisplay(testText, message, rect, canvas.scaleFactor);
    }

    //punctuation is included in the strings (since ' ' is used as a delimiter).
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
        var words = message.Split(' ').ToList();

        //next we add the words to the strings

        for (var i = 0; i < words.Count; i++)
        {
            currentString += $" {words[i]}";
            //supposedly this if statement always fails... however this function... well, functioned, fine so maybe i forgot (as usual) to do some code cleanup
            //in any case, i'm not saying this code is efficient, but it does the stuff properly so who cares? not like unity provides a better option (yet?)
            var prefHeight = testText.GetPreferredValues(currentString,rectWidth,rectHeight).y;
            if (prefHeight > rectHeight)
            {
                //removes a random space that would sometimes appear at the start of a message
                if (Char.IsWhiteSpace(
                        previousString[0])) //i guess we could also do a while loop in case multiple show up buy meh
                    previousString = previousString[1..];
                strings.Add(previousString);
                previousString = "";
                currentString = previousString = words[i];
            }
            //this is kinda useless since currentString would be set blank in the if statement...
            else
            {
                previousString = currentString;
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


    /*private static List<string> MaxCnJpDisplay(string message, TextGenerator textGenerator, TextGenerationSettings generationSettings, float rectHeight)
    {
        
        
    }*/
    
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


        var words = message.Split(' ').ToList();

        //next we add the words to the strings

        for (var i = 0; i < words.Count; i++)
        {
            currentString += $" {words[i]}";

            //supposedly this if statement always fails... however this function... well, functioned, fine so maybe i forgot (as usual) to do some code cleanup
            //in any case, i'm not saying this code is efficient, but it does the stuff properly so who cares? not like unity provides a better option (yet?)
            var prefHeight = testText.GetPreferredValues(currentString, rectWidth, rectHeight).y;
            if (prefHeight > rectHeight)
            {
                //removes a random space that would sometimes appear at the start of a message
                if (Char.IsWhiteSpace(previousString[0]))
                    previousString = previousString[1..];
                strings.Add(previousString);
                previousString = "";
                currentString = previousString = words[i];
            }
            //this is kinda useless since currentString would be set blank in the if statement...
            else
            {
                previousString = currentString;
            }
        }

        if (Char.IsWhiteSpace(previousString[0]))
            previousString = previousString[1..];
        strings.Add(previousString);
        return strings;
    }

    private static List<string> MaxCharDisplay(TextMeshProUGUI testText, string message, Rect rect,
        float canvasScaleFactor)
    {
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
            //this is kinda useless since currentString would be set blank in the if statement...
            else
            {
                s += c;
            }
        }

        return strings;
    }


    /*private static List<string> MaxCnJpDisplay(string message, TextGenerator textGenerator, TextGenerationSettings generationSettings, float rectHeight)
    {
        
        
    }*/
    private static bool IsCnJp(char c)
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
    }
}