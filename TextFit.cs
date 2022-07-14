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
        //if there's whitespace, then it will fill the textbox normally based on words/newlines, otherwise it will try to fill the text box with as many chars as possible
        if (message.Any(char.IsWhiteSpace))
            if (newlineSeparator)
                return MaxVerticalWordDisplayAlt(message, textGenerator, generationSettings, rectHeight);
            else
                return MaxVerticalWordDisplay(message, textGenerator, generationSettings, rectHeight);
        else
            return MaxCharDisplay(message, textGenerator, generationSettings, rectHeight);
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
    public static List<string> MaxVerticalTextDisplay(Text testText, string message, bool newlineSeparator = false)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("The string was either null or empty.", nameof(message));
        
        var rect = testText.rectTransform.rect;
        var generationSettings = testText.GetGenerationSettings(rect.size);
        var rectHeight = rect.height;

        var textGenerator = testText.cachedTextGeneratorForLayout;

        if (message.Any(char.IsWhiteSpace))
            if (newlineSeparator)
                return MaxVerticalWordDisplayAlt(message, textGenerator, generationSettings, rectHeight);
            else
                return MaxVerticalWordDisplay(message, textGenerator, generationSettings, rectHeight);
        else
            return MaxCharDisplay(message, textGenerator, generationSettings, rectHeight);
    }

    private static readonly Regex WORD_MATCH = new(@"\b(\S+\s*)");
    
    //punctuation is included in the strings (since ' ' is used as a delimiter).
    private static List<string> MaxVerticalWordDisplay(string message, TextGenerator textGenerator,
        TextGenerationSettings generationSettings, float rectHeight)
    {
        if (textGenerator.GetPreferredHeight("W", generationSettings) > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        string currentString = "";
        string previousString = ""; //prev string exists to prevent the need to backtrack/try to subtract a string (although I suppose I could use words[i].Length and some substring nonsense)
        var strings = new List<string>(3);
        //var words = _regex.Split(message).Where(s => !string.IsNullOrEmpty(s)).ToList();
        string[] words = WORD_MATCH.Matches(message).Select(m => m.Value).ToArray();
        //next we add the words to the strings

        for (var i = 0; i < words.Length; i++)
        {
            currentString += words[i];
            float prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
            if (prefHeight > rectHeight) //if the text cannot fit into the box
            {
                strings.Add(previousString);
                currentString = previousString = words[i];
            }
            else //if it can fit then update previousString
            {
                previousString = currentString;
            }
        }
        strings.Add(previousString);
        return strings;
    }
    
    //this one sets interprets newlines and carriage returns as separating the message to a new element in the array
    private static List<string> MaxVerticalWordDisplayAlt(string message, TextGenerator textGenerator,
        TextGenerationSettings generationSettings, float rectHeight)
    {
        if (textGenerator.GetPreferredHeight("W", generationSettings) > rectHeight)
        {
            throw new UnityException("The width or height of the Text GameObject is too small to fit any text.");
        }
        string currentString = "";
        string previousString = ""; //prev string exists to prevent the need to backtrack/try to subtract a string (although I suppose I could use words[i].Length and some substring nonsense)
        var strings = new List<string>(3);
        //var words = _regex.Split(message).Where(s => !string.IsNullOrEmpty(s)).ToList();
        string[] words = WORD_MATCH.Matches(message).Select(m => m.Value).ToArray();
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
                    prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
                    if (prefHeight > rectHeight) //if the text cannot fit into the box
                    {
                        strings.Add(previousString);
                        Debug.Log(strings[^1]);
                        currentString = previousString = curWord[prevIndex..index];
                        Debug.Log(currentString);
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

                prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
                if (prefHeight > rectHeight) //if the text cannot fit into the box
                {
                    strings.Add(previousString);
                    currentString = previousString = curWord;
                }
                else //otherwise we add stuff to prev string
                {
                    previousString = currentString;
                }
            }
        }
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

public static class TestClass
{
    public static bool TryFindIndices<T>(this IEnumerable<T> items, Func<T, bool> predicate, out IEnumerable<int> indices) 
    {
        int i = 0;
        List<int> indicesList = new List<int>(1);
        foreach (var item in items) 
        {
            if (predicate(item))
            {
                indicesList.Add(i);
            }

            i++;
        }

        indices = indicesList.AsEnumerable();
        return indicesList.Count > 0;
    } //modified from sta ckover flow.c om/questions/14476162/14476244#14476244
}