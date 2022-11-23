using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Instance
{
    public int numErrors;
    public float responseTime;
    public float errorTime;

    public Instance(int numErrors, float responseTime, float errorTime)
    {
        this.numErrors = numErrors;
        this.responseTime = responseTime;
        this.errorTime = errorTime;
    }

    public override string ToString()
    {
        return "responseTime: " + responseTime + ", numErrors: " + numErrors + " errorTime: " + errorTime;
    }
}

public struct Word
{
    public string word;
    public List<Instance> instances;  // list count == num repetitions of this word in the dataset

    public Word(string word) : this()
    {
        this.word = word;
        instances = new List<Instance>();
    }

    public void AddInstance(Instance instance)
    {
        instances.Add(instance);
    }

    public override string ToString()
    {
        var str = "word: " + word;
        foreach (var instance in instances)
        {
            str += " {" + instance + "} ";
        }

        return str;
    }
}

public class DataSet
{
    private Dictionary<string, Word> words;

    public DataSet()
    {
        words = new Dictionary<string, Word>();
    }

    public void RecordEntry(string word, float time, int errors = 0, float errorTime = 0f)
    {
        if (words.ContainsKey(word))
        {
            var w = words[word];
            w.AddInstance(new Instance(errors, time, errorTime));
        }
        else
        {
            var w = new Word(word);
            w.AddInstance(new Instance(errors, time, errorTime));
            words.Add(word, w);
        }
    }

    public Dictionary<string, Word> GetWords() => words;
}

public class DataSaver : MonoBehaviour
{
    private string filename = "data.txt";

    public void SaveVersion(bool gay)
    {
        StreamWriter writer;
        
        if (File.Exists(filename))
        {
            Debug.Log("file " + filename + " already exists, appending");
            writer = new StreamWriter(filename, append: true);
        }
        else
        {
            Debug.Log("creating file " + filename);
            writer = new StreamWriter(filename);
        }
        
        writer.WriteLine(gay ? "gay version" : "str8 version");
        writer.Close();
    }
    
    public void SaveData(DataSet dataSet, string blockName)
    {
        StreamWriter writer;
        
        if (File.Exists(filename))
        {
            Debug.Log("file " + filename + " already exists, appending");
            writer = new StreamWriter(filename, append: true);
        }
        else
        {
            Debug.Log("creating file " + filename);
            writer = new StreamWriter(filename);
        }

        writer.WriteLine(blockName);
        foreach (var word in dataSet.GetWords())
        {
            writer.WriteLine(word);
        }
        writer.Close();
        
        
        // TODO should save to an excel table
    }

    public void EndSciatBlock()
    {
        var writer = new StreamWriter(filename, true);
        
        writer.WriteLine("SC-IAT Block end");
        writer.WriteLine("");
        writer.Close();
    }
    
}
