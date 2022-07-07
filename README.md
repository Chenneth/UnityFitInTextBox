# UnityFitInTextBox

## Summary

Splits a given string into an array in which each element fits into a given Unity TextBox. Each new string is delimited so that only full words appear; the last word or phrase to appear in each string will not be cut off.

### Disclaimer

This code is a snippet from my DialogueSystem repository, but for better visibility (since this has a wider range of uses beyond that), this bit was extracted from that solution.

This code does **NOT** work well with languages that don't use spaces to delimit words (e.g. Japanese or Chinese). It will separate the strings based on however many characters it can fit into the textbox.
