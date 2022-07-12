using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextFit : MonoBehaviour
{
    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The Unity Text to be tested.</param>
    /// <param name="canvas">The Canvas that contains <see cref="testText" />/></param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <param name="newlineSeparator">If true, then newline and carriage returns pushes the following characters into a new index.</param>
    /// <returns></returns>
    public static List<string> MaxVerticalTextDisplay(Text testText, Canvas canvas, string message, bool newlineSeparator = false)
    {
        
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("The string was either null or empty.", nameof(message));
        var rect = testText.rectTransform.rect;
        var generationSettings = testText.GetGenerationSettings(rect.size);
        var rectHeight = rect.height * canvas.scaleFactor;

        var textGenerator = testText.cachedTextGeneratorForLayout;


        /*
        if (message.All(IsCnJp)) //if all the chars are cn/jp
        {
            //then we do diff algorithm (yay)
            
        }
        */
        
        //if there's whitespace, then it will fill the textbox normally based on words/newlines, otherwise it will try to fill the text box with as many chars as possible
        return message.All(char.IsWhiteSpace) ? MaxVerticalWordDisplay(message, textGenerator, generationSettings, rectHeight) : MaxCharDisplay(message, textGenerator, generationSettings, rectHeight);
    }

    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The Unity Text to be tested.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns></returns>
    public static List<string> MaxVerticalTextDisplay(Text testText, string message)
    {
        
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("The string was either null or empty.", nameof(message));
        
        var rect = testText.rectTransform.rect;
        var generationSettings = testText.GetGenerationSettings(rect.size);
        var rectHeight = rect.height;

        var textGenerator = testText.cachedTextGeneratorForLayout;



        /*
        if (message.All(IsCnJp)) //if all the chars are cn/jp
        {
            //then we do diff algorithm (yay)
            //MaxCnJpDisplay(message, textGenerator, generationSettings, rectHeight);
        }
        */
        
        return message.All(char.IsWhiteSpace) ? MaxVerticalWordDisplay(message, textGenerator, generationSettings, rectHeight) : MaxCharDisplay(message, textGenerator, generationSettings, rectHeight);
    }

    private static readonly Regex _regex = new(@"\b(\S+\s*)");
    
    //punctuation is included in the strings (since ' ' is used as a delimiter).
    private static List<string> MaxVerticalWordDisplay(string message, TextGenerator textGenerator,
        TextGenerationSettings generationSettings, float rectHeight)
    {
        if (textGenerator.GetPreferredHeight("W", generationSettings) > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        string currentString = "";
        string previousString = "";
        var strings = new List<string>(3);
        //var words = _regex.Split(message).Where(s => !string.IsNullOrEmpty(s)).ToList();
        string[] words = _regex.Matches(message).Select(m => m.Value).ToArray();
        //next we add the words to the strings

        for (var i = 0; i < words.Length; i++)
        {
            currentString += words[i];
            /*currentString += $" {words[i]}";*/

            //supposedly this if statement always fails... however this function... well, functioned, fine so maybe i forgot (as usual) to do some code cleanup
            //in any case, i'm not saying this code is efficient, but it does the stuff properly so who cares? not like unity provides a better option (yet?)
            var prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
            if (prefHeight > rectHeight)
            {
                //unknown if this is needed due to now using Regex.Matches instead of string.split
                /*//removes a random space that would sometimes appear at the start of a message
                if (Char.IsWhiteSpace(previousString[0]))
                    previousString = previousString[1..];*/
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

    private static List<string> MaxCharDisplay(string message, TextGenerator textGenerator,
        TextGenerationSettings generationSettings, float rectHeight)
    {
        if (textGenerator.GetPreferredHeight("W", generationSettings) > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        var strings = new List<string>(3);
        var chars = message.ToCharArray();
        string s = "";
        foreach (char c in chars)
        {
            var prefHeight = textGenerator.GetPreferredHeight(s+c, generationSettings);
            if (prefHeight > rectHeight)
            {
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
                if (c >= 0x4E00 && c<=0x9FFF)
                {
                    return true;
                }
            }
        }
        
        return false;
    }*/
}