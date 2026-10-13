using System;
using System.Collections.Generic;

[Serializable]
public class Passage
{
    public string passageName;
    public List<string> lines;

    public Passage(string passageName)
    {
        this.passageName = passageName;
        lines = new List<string>();
    }
}
