using System;
using System.Collections.Generic;

namespace RelativeControl.Avalonia;

public static class Splitters {
    public static string[] Split(string text, params char[] separators) {
        if (separators.Length == 0)
            return [text];

        string sep = string.Concat(separators);
        return [..SplitEnum(text, sep)];
    }

    public static string[] Split(string text, string separators) { return [..SplitEnum(text, separators)]; }

    public static IEnumerable<string> SplitEnum(string text, params char[] separators) {
        if (separators.Length == 0) {
            yield return text;
            yield break;
        }

        string sep = string.Concat(separators);
        foreach (string s in Split(text, sep))
            yield return s;
    }

    public static IEnumerable<string> SplitEnum(string text, string separators) {
        text = text.Trim();
        if (text.Length == 0)
            throw new FormatException("Value is empty.");

        if (separators.Contains(text[0]))
            throw new FormatException("Value begins with a separator.");

        string curr = string.Empty;
        foreach (char c in text) {
            if (!separators.Contains(c)) {
                curr += c;
                continue;
            }

            if (curr.Length == 0) {
                if (c != ' ')
                    throw new FormatException("An empty value was detected.");
                continue;
            }

            yield return curr;
            curr = string.Empty;
        }

        if (curr.Length <= 0)
            throw new FormatException("An empty value was detected.");
        yield return curr;
    }
}