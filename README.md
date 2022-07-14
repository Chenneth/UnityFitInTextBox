# UnityFitInTextBox

## Summary

Splits a given string into a list in which each element fits into a given Unity TextBox. Each new string is delimited so that only full words appear; the last word or phrase to appear in each string will not be cut off.

## Usage

Download ``TextFit.cs`` if you are using Unity Text, or download ``TextMeshProFit.cs`` if you are using Unity TextMesh Pro. Additionally, download ``HelperClass.cs`` as it's used by both scripts. HelperClass contains a static function and compiled regex, however you can also choose to place those in the same file as the TextFit functions if needed.

To use, use the function ``MaxVerticalTextDisplay()``. If the Text/TextMesh Pro is part of a Canvas, then use the function that has a Canvas parameter. This is to take into account the Canvas' scale factor. I think it's necessary? This function returns the list of strings. Example usage code is provided in ``Example.cs``.

### Very minor disclaimer

Original code was from my DialogueSystem repository. The code in that one hasn't been updated to this one yet (honestly probably never will since I hate that project + it serves no purpose). The reason I took this from the original is because it has use outside of that ~~garbage~~ niche project.

This code does **NOT** work well with languages that don't use spaces to delimit words (e.g. Japanese or Chinese). It will separate the strings based on however many characters it can fit into the textbox.
