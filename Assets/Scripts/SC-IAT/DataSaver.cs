using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// The category of the stimuli words. Positive words, negative words, and words pertaining to the attitude object.
/// </summary>
public enum WordCategory
{
    Good,
    Bad,
    Gay,
    NA
}

/// <summary>
/// The type of the test block in the SC-IAT.
/// </summary>
public enum TestType
{
    Practice1,
    Test1,
    Practice2,
    Test2
}

/// <summary>
/// One row of the SC-IAT table.
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
    public float firstErrorTime;

    /// <summary>
    /// Creates a new entry according to the specified parameters.
    /// </summary>
    /// <param name="word">The word shown on the screen</param>
    /// <param name="testType">The type of test block</param>
    /// <param name="correctResponse">The correct response key</param>
    /// <param name="wordCategory">The word category</param>
    /// <param name="responseTime">Response time</param>
    /// <param name="numErrors">Number of errors</param>
    /// <param name="errorTime">Error time</param>
    /// <param name="orderInSet">The order of the stimulus in the set</param>
    /// <param name="firstErrorTime">The time of the first error</param>
    public Entry(string word, TestType testType, KeyCode correctResponse, WordCategory wordCategory, float responseTime, 
        int numErrors, float errorTime, int orderInSet, float firstErrorTime)
    {
        this.word = word;
        this.testType = testType;
        this.correctResponse = correctResponse;
        this.wordCategory = wordCategory;
        this.responseTime = responseTime;
        this.numErrors = numErrors;
        this.errorTime = errorTime;
        this.orderInSet = orderInSet;
        this.firstErrorTime = firstErrorTime;
    }

    /// <summary>
    /// Returns entry as a string suitable for saving as a line in .csv format. 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return (Utility.secondSciat ? "sciat2" : "sciat1") + "," +
               word + "," +
               testType + "," +
               correctResponse + "," +
               wordCategory + "," +
               responseTime.ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "," +
               numErrors + "," +
               errorTime.ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "," +
               orderInSet + "," +
               firstErrorTime.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
    }
}

/// <summary>
/// Wrapper for one SC-IAT dataset.
/// </summary>
public class DataSet
{
    private List<Entry> entries;

    /// <summary>
    /// Constructs a new dataset.
    /// </summary>
    public DataSet()
    {
        entries = new List<Entry>();
    }

    /// <summary>
    /// Adds a new entry according to the specified parameters to the dataset.
    /// </summary>
    /// <param name="word">The word shown on the screen</param>
    /// <param name="testType">The type of test block</param>
    /// <param name="correctResponse">The correct response key</param>
    /// <param name="wordCategory">The word category</param>
    /// <param name="responseTime">Response time</param>
    /// <param name="numErrors">Number of errors</param>
    /// <param name="errorTime">Error time</param>
    /// <param name="orderInSet">The order of the stimulus in the set</param>
    /// <param name="firstErrorTime">The time of the first error</param>
    public void RecordEntry(string word, TestType testType, KeyCode correctResponse, WordCategory wordCategory, 
        float responseTime, int numErrors, float errorTime, int orderInSet, float firstErrorTime)
    {
        var entry = new Entry(word, testType, correctResponse, wordCategory, 
            responseTime, numErrors, errorTime, orderInSet, firstErrorTime);
        entries.Add(entry);
    }
    
    /// <summary>
    /// Returns the list of entries for this dataset.
    /// </summary>
    /// <returns></returns>
    public List<Entry> GetEntries() => entries;
}

/// <summary>
/// Responsible for saving the game analytics and the data from the SC-IAT into a .csv file.
/// </summary>
public class DataSaver : MonoBehaviour
{
    private string filename = "data.csv";

    /// <summary>
    /// Saves which version of the game the player is using (experimental or control).
    /// </summary>
    /// <param name="gay">The version of the game, true means experimental, false means control</param>
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
        
        writer.WriteLine("Version," +  (gay ? "Gay" : "Straight") + ",2");
        writer.WriteLine("");
        writer.Close();
    }
    
    /// <summary>
    /// Appends the provided dataset to the .csv file.
    /// </summary>
    /// <param name="dataSet">The dataset to save</param>
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
        writer.WriteLine("testNumber,word,testType,correctResponse,wordCategory,responseTime,numErrors,errorTime,orderInSet,firstErrorTime");
        
        foreach (var entry in dataSet.GetEntries())
        {
            writer.WriteLine(entry);
        }
        
        writer.WriteLine("");
        writer.Close();
    }

    /// <summary>
    /// Signals the .csv file to end the SC-IAT block.
    /// </summary>
    public void EndSciatBlock()
    {
        var writer = new StreamWriter(filename, true);
        
        writer.WriteLine("");
        writer.Close();
    }

    /// <summary>
    /// Saves the game analytics to the .csv file. The analytics include the version of the game, FPS, whether
    /// the player won the game, game duration, and date.
    /// </summary>
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
        
        writer.WriteLine("Version," +  (Utility.gayVersion ? "Gay" : "Straight"));
        writer.WriteLine("Fps," + Utility.avgFps.ToString(CultureInfo.InvariantCulture).Replace(",", "."));
        writer.WriteLine("Game won," + (Utility.gameWon ? "Yes" : "No"));
        writer.WriteLine("Game duration," + Utility.gameDuration.ToString(CultureInfo.InvariantCulture).Replace(",", "."));
        writer.WriteLine("Date," +  DateTime.Today);
        
        writer.WriteLine("");
        writer.WriteLine("");
        
        writer.Close();
    }
    
}
