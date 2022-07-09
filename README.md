# UnityFitInTextBox

## Summary

Splits a given string into an array in which each element fits into a given Unity TextBox. Each new string is delimited so that only full words appear; the last word or phrase to appear in each string will not be cut off.

### Disclaimer

Oringinal code was from my DialogueSystem repository. The code in that one hasn't been updated to this one yet.

This code does **NOT** work well with languages that don't use spaces to delimit words (e.g. Japanese or Chinese). It will separate the strings based on however many characters it can fit into the textbox.

Additionally, this currently uses the UI Text feature instead of TextMesh Pro. Will work on one to use TMP later.
