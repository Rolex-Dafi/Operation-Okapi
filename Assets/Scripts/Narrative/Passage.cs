using System;
using System.Collections.Generic;

/// <summary>
/// A dialogue passage which contains a list of dialogue lines for the passage.
/// </summary>
[Serializable]
public class Passage
{
    public string passageName;
    public List<string> lines;

    /// <summary>
    /// Constructs a new passage with the specified passage name.
    /// </summary>
    /// <param name="passageName">The name of the passage</param>
    public Passage(string passageName)
    {
        this.passageName = passageName;
        lines = new List<string>();
    }
}
