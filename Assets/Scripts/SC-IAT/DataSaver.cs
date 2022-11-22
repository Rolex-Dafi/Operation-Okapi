using System.Collections.Generic;
using UnityEngine;

public struct Instance
{
    public int numErrors;
    public float responseTime;

    public Instance(int numErrors, float responseTime)
    {
        this.numErrors = numErrors;
        this.responseTime = responseTime;
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
}

public class DataSet
{
    private Dictionary<string, Word> words;

    public DataSet()
    {
        words = new Dictionary<string, Word>();
    }

    public void RecordEntry(string word, int errors, float time)
    {
        if (words.ContainsKey(word))
        {
            var w = words[word];
            w.AddInstance(new Instance(errors, time));
        }
        else
        {
            var w = new Word(word);
            w.AddInstance(new Instance(errors, time));
            words.Add(word, w);
        }
    }
}

public class DataSaver : MonoBehaviour
{

    public void SaveData(DataSet dataSet, string fileName)
    {
        // should save to an excel table
    }
    
}
