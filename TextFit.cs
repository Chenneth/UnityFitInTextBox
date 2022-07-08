using System.Collections.Generic;
using System.Linq;
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
    /// <param name="testText">The text box to be tested.</param>
    /// <param name="canvas">The Canvas that contains <see cref="testText" />/></param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns></returns>
    public static List<string> MaxVerticalWordDisplay(Text testText, Canvas canvas, string message)
    {
        var previousString = "";
        var currentString = "";

        var rect = testText.rectTransform.rect;
        var generationSettings = testText.GetGenerationSettings(rect.size);
        var rectHeight = rect.height * canvas.scaleFactor;

        var textGenerator = testText.cachedTextGeneratorForLayout;

        
        
        //if the given message does not contain any spaces todo: check for JP/CN chars (since I don't know anything about KR/AR etc.) to delimit it in its own algorithm, 
        if (!message.Contains(" "))
        {
            //then we will run through each character instead and return the string which has as many characters within it as possible
            //Without overflowing the textbox height
            var chars = message.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                currentString += chars[i].ToString();
                
                if (textGenerator.GetPreferredHeight(currentString, generationSettings) > rectHeight)
                    return new List<string> { previousString };

                previousString = currentString;
            }

            return new List<string> { currentString };
        }

        var strings = new List<string>(3);


        var words = message.Split(' ').ToList();

        //next we add the words to the strings

        for (var i = 0; i < words.Count; i++)
        {
            currentString += $" {words[i]}";

            //supposedly this if statement always fails... however this function... well, functioned, fine so maybe i forgot (as usual) to do some code cleanup
            //in any case, i'm not saying this code is efficient, but it does the stuff properly so who cares? not like unity provides a better option (yet?)
            var prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
            if (prefHeight > rectHeight)
            {
                //removes a random space that would sometimes appear at the start of a message
                if (previousString.StartsWith(" "))
                    previousString = previousString.Substring(1, previousString.Length - 1);
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

        if (previousString.StartsWith(" "))
            previousString = previousString.Substring(1, previousString.Length - 1);
        strings.Add(previousString);
        return strings;
    }
    
    /// <summary>
    ///     Returns a list of strings delimited by the maximum amount of words that can fit inside <see cref="testText" />.
    ///     This stops vertical overflow of text. This does not stop horizontal overflow.
    ///     This only works for languages that use spaces to delimit words (this may result in unnatural splicing of East Asian
    ///     words)
    /// </summary>
    /// <param name="testText">The text box to be tested.</param>
    /// <param name="message">The message to be inputted into <see cref="testText" /></param>
    /// <returns></returns>
    public static List<string> MaxVerticalWordDisplay(Text testText, string message)
    {
        var previousString = "";
        var currentString = "";

        var rect = testText.rectTransform.rect;
        var generationSettings = testText.GetGenerationSettings(rect.size);
        var rectHeight = rect.height;

        var textGenerator = testText.cachedTextGeneratorForLayout;

        //if the given message does not contain any spaces todo: check for JP/CN chars (since I don't know anything about KR/AR etc.) to delimit it in its own algorithm, 
        if (!message.Contains(" "))
        {
            //then we will run through each character instead and return the string which has as many characters within it as possible
            //Without overflowing the textbox height
            var chars = message.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                currentString += chars[i].ToString();
                
                if (textGenerator.GetPreferredHeight(currentString, generationSettings) > rectHeight)
                    return new List<string> { previousString };

                previousString = currentString;
            }

            return new List<string> { currentString };
        }

        var strings = new List<string>(3);


        var words = message.Split(' ').ToList();

        //next we add the words to the strings

        for (var i = 0; i < words.Count; i++)
        {
            currentString += $" {words[i]}";

            //supposedly this if statement always fails... however this function... well, functioned, fine so maybe i forgot (as usual) to do some code cleanup
            //in any case, i'm not saying this code is efficient, but it does the stuff properly so who cares? not like unity provides a better option (yet?)
            var prefHeight = textGenerator.GetPreferredHeight(currentString, generationSettings);
            if (prefHeight > rectHeight)
            {
                //removes a random space that would sometimes appear at the start of a message
                if (previousString.StartsWith(" "))
                    previousString = previousString.Substring(1, previousString.Length - 1);
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

        if (previousString.StartsWith(" "))
            previousString = previousString.Substring(1, previousString.Length - 1);
        strings.Add(previousString);
        return strings;
    }
}