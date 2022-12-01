using System;
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

public enum WordCategory
{
    Good,
    Bad,
    Gay,
    NA
}

public enum TestType
{
    Practice1,
    Test1,
    Practice2,
    Test2
}

/// <summary>
/// One row of the sc-iat table.
/// </summary>
public class Entry
{
    public string word;
    public TestType testType;
    public KeyCode correctResponse;
    public WordCategory wordCategory;
    public float responseTime;
    public int numErrors;
    public float errorTime;
    public int orderInSet;

    public Entry(string word, TestType testType, KeyCode correctResponse, WordCategory wordCategory, float responseTime, int numErrors, float errorTime, int orderInSet)
    {
        this.word = word;
        this.testType = testType;
        this.correctResponse = correctResponse;
        this.wordCategory = wordCategory;
        this.responseTime = responseTime;
        this.numErrors = numErrors;
        this.errorTime = errorTime;
        this.orderInSet = orderInSet;
    }

    /// <summary>
    /// Returns entry as a string suitable for saving as a line in csv format. 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return word + "," + testType + "," + correctResponse + "," + wordCategory + "," + responseTime + "," +
               numErrors + "," + errorTime + "," + orderInSet;
    }
}

public class DataSet
{
    private Dictionary<string, Word> words;
    private List<Entry> entries;

    public DataSet()
    {
        words = new Dictionary<string, Word>();
        entries = new List<Entry>();
    }

    public void RecordEntryArch(string word, float time, int errors = 0, float errorTime = 0f)
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

    public void RecordEntry(string word, TestType testType, KeyCode correctResponse, WordCategory wordCategory, 
        float responseTime, int numErrors, float errorTime, int orderInSet)
    {
        var entry = new Entry(word, testType, correctResponse, wordCategory, 
            responseTime, numErrors, errorTime, orderInSet);
        entries.Add(entry);
    }
    
    public Dictionary<string, Word> GetWords() => words;

    public List<Entry> GetEntries() => entries;
}

public class DataSaver : MonoBehaviour
{
    private string filename = "data.csv";

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
        
        writer.WriteLine("Version," +  (gay ? "Gay" : "Straight"));
        writer.WriteLine("");
        writer.Close();
    }
    
    public void SaveDataArch(DataSet dataSet, string blockName)
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
    
    public void SaveSciatData(DataSet dataSet)
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
        
        // header
        writer.WriteLine("word,testType,correctResponse,wordCategory,responseTime,numErrors,errorTime,orderInSet");
        
        foreach (var entry in dataSet.GetEntries())
        {
            writer.WriteLine(entry);
        }
        
        writer.WriteLine("");
        writer.Close();
    }

    public void EndSciatBlock()
    {
        var writer = new StreamWriter(filename, true);
        
        writer.WriteLine("");
        writer.WriteLine("");
        writer.Close();
    }

    public void SaveAnalytics()
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
        
        // analytics
        writer.WriteLine("Version," +  (Utility.gayVersion ? "Gay" : "Straight"));
        writer.WriteLine("Game won," + (Utility.gameWon ? "Yes" : "No"));
        writer.WriteLine("Date," +  DateTime.Today);
        
        writer.Close();
    }
    
}
