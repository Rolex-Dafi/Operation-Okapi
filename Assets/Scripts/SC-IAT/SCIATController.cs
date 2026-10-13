using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Pair
{
    public KeyCode key;
    public string stimulus;

    public Pair(KeyCode key, string stimulus)
    {
        this.key = key;
        this.stimulus = stimulus;
    }

    public override string ToString()
    {
        return (key == KeyCode.E ? "left + " : "right + ") + stimulus;
    }
}

public class SCIATController : MonoBehaviour
{
    [SerializeField] private DataSaver dataSaver;
    
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private CanvasGroup redCross;

    [SerializeField] private Button goToQst;
    
    private string[] instructionMessages =
    {
        // welcome message
        "\n" +
        "In this experiment you will be shown words on your screen. Your task is to assign these words " +
        "into different categories. If you want to assign the word to the category on the left, press (E), if you want to assign it to the " +
        "category on the right, press (I). There are three categories in total:\n" +
        "1. POSITIVE ADJECTIVES\n" +
        "2. NEGATIVE ADJECTIVES\n" +
        "3. LESBIAN AND BISEXUAL WOMEN\n" +
        "\n" +
        "Categories assigned to the (E) key will show in the upper left corner, categories assigned to the (I) key will show " +
        "in the upper right corner. The positions of the categories will change.\n" +
        "\n" +
        "Try to answer as fast as you can, with as little errors as possible. If you make an error a red cross will flash on the screen, " +
        "prompting you to answer correctly before proceeding.\n" +
        "\n" +
        "Press either the E or the I key to begin.",
        
        // instructions for 1st part
        "In this round you will be assigning words to categories LESBIAN AND BISEXUAL WOMEN, POSITIVE ADJECTIVES and NEGATIVE ADJECTIVES. \n" +
        "The categories LESBIAN AND BISEXUAL WOMEN and POSITIVE ADJECTIVES are on the left (E key), the category NEGATIVE ADJECTIVES is on the right (I key).\n" +
        "\n" +
        "Be aware that this test is not an expression of your personal opinion! Your task is only to assign the given words to the specific categories as fast as possible.\n" +
        "\n" +
        "Press either the E or the I key to continue.",
        
        // instructions for second part
        "In this round you will be assigning words to categories LESBIAN AND BISEXUAL WOMEN, POSITIVE ADJECTIVES and NEGATIVE ADJECTIVES. \n" +
        "The category POSITIVE ADJECTIVES is on the left (E key), the categories LESBIAN AND BISEXUAL WOMEN and NEGATIVE ADJECTIVES are on the right (I key).\n" +
        "\n" +
        "Press either the E or the I key to continue.",
        
        // outro
        "You have completed the reaction time task! \n" +
        "\n" +
        "Press either the E or the I key to continue."
    };

    private string goodWord = "good words";
    private string badWord = "bad words";
    private string gayShit = "lesbian or bisexual women";

    // TODO each category has 8 words
    private string[] goodWords =
    {
        "great",
        "amazing",
        "wonderful",
        "pleasant",
        "enjoyable",
        "lovely",
        "beautiful",
        "delightful"
    };
    
    private string[] badWords =
    {
        "awful",
        "disgusting",
        "immoral",
        "gross",
        "perverted",
        "evil",
        "shameful"
    };

    private string[] gayWords =
    {
        "lesbian",
        "bisexual",
        "gay",
        "LGBT",
        "queer",
        "homosexuality",
        "lesbianism",
        "bisexuality"
    };
    
    private List<Pair> stimuli = new List<Pair>();

    private const KeyCode leftKey = KeyCode.E;
    private const KeyCode rightKey = KeyCode.I;

    private KeyCode lastInput;

    private List<DataSet> allData = new List<DataSet>();
    private DataSet currentDataSet;
    private TestType currentTestType;

    private float lastStimulusTime;  // what time was the last stimulus shown
    
    private void Awake()
    {
        if (!Utility.secondSciat) 
        {
            dataSaver.SaveVersion(Utility.gayVersion);
        }
        
        StartCoroutine(TestLoop());
    }
    
    private IEnumerator TestLoop()
    {
        goToQst.gameObject.SetActive(false);
        
        // welcome message
        instructionsText.text = (Utility.secondSciat ? "Welcome back to the experiment\n" : "Welcome to the experiment\n") + instructionMessages[0];
        yield return new WaitUntil(GetAnyInput);
        yield return null;
        
        // --------------------------------------------
        // set instructions
        instructionsText.text = instructionMessages[1];
        
        // and corners
        leftText.text = goodWord + "\n OR\n" + gayShit;
        rightText.text = badWord;
        testText.text = "";

        yield return new WaitUntil(GetAnyInput);
        yield return null;

        // start test block
        instructionsText.text = "";

        //yield return new WaitForSeconds(.1f);
        
        // 1st block - 2 blocks as one continuous
        FillStimuliB1(1); // 1 repetition for practice
        currentTestType = TestType.Practice1;
        yield return RunTestBlock(true);
        dataSaver.SaveSciatData(currentDataSet);
        //dataSaver.SaveDataArch(currentDataSet, "Block1 - practice");
        
        FillStimuliB1(3); // 3 repetitions for test
        currentTestType = TestType.Test1;
        yield return RunTestBlock(false);
        dataSaver.SaveSciatData(currentDataSet);
        //dataSaver.SaveDataArch(currentDataSet, "Block1 - test");
        
        // --------------------------------------------
        // set instructions
        instructionsText.text = instructionMessages[2];
        
        // and corners
        leftText.text = goodWord;
        rightText.text = badWord + "\n OR\n" + gayShit;
        testText.text = "";
        
        yield return new WaitUntil(GetAnyInput);
        yield return null;
        
        // start test block
        instructionsText.text = "";

        // 2nd block - 2 blocks as one continuous
        FillStimuliB2(1); // 1 repetition for practice
        currentTestType = TestType.Practice2;
        yield return RunTestBlock(true);
        dataSaver.SaveSciatData(currentDataSet);
        //dataSaver.SaveDataArch(currentDataSet, "Block2 - practice");
        
        FillStimuliB2(3); // 3 repetitions for test
        currentTestType = TestType.Test2;
        yield return RunTestBlock(false);
        dataSaver.SaveSciatData(currentDataSet);
        //dataSaver.SaveDataArch(currentDataSet, "Block2 - test");
        
        // outro message
        instructionsText.text = instructionMessages[3];        
        testText.text = "";

        yield return new WaitUntil(GetAnyInput);
        yield return null;
        
        // show formr link
        instructionsText.text = Utility.secondSciat ? "Please go back to the questionnaire now" : "Please go to the following link to fill out a short questionnaire, then return here.";  // todo this depends if on 2nd go through
        goToQst.gameObject.SetActive(true);
        
        dataSaver.EndSciatBlock();
    }
    
    public void GoToQst()
    {
        Application.OpenURL("https://diana.ms.mff.cuni.cz/formr/OcapiRun");

        if (Utility.secondSciat)
        {
            Debug.Log("quitting game");
            Application.Quit();
        }
        else
        {
            // count the frame rate for first sciat
            Utility.avgFps = Time.frameCount / Time.time;
            
            GoToGame();
        }
    }
    
    private void GoToGame()
    {
        Debug.Log("switching scene");
        SceneManager.LoadScene("SceneBase");
    }
    
    /// <summary>
    /// Stimuli for 1st block.
    /// </summary>
    /// <param name="repetitions"></param>
    private void FillStimuliB1(int repetitions)
    {
        FillStimuli(repetitions, true);
    }

    /// <summary>
    /// Stimuli for 2nd block.
    /// </summary>
    /// <param name="repetitions"></param>
    private void FillStimuliB2(int repetitions)
    {
        FillStimuli(repetitions, false);
    }

    /// <summary>
    /// Fills the stimuli dictionary for the next block.
    /// </summary>
    /// <param name="repetitions">How many times the same stimuli appears in this block</param>
    /// <param name="t1"></param>
    private void FillStimuli(int repetitions, bool t1)
    {
        stimuli.Clear();
        for (int i = 0; i < repetitions; i++)
        {
            // left
            foreach (var word in goodWords)
            {
                stimuli.Add(new Pair(leftKey, word));
            }
            // left or right - depends on which block we're in
            foreach (var word in gayWords)
            {
                stimuli.Add(new Pair(t1 ? leftKey : rightKey, word));
            }
            // right
            foreach (var word in badWords)
            {
                stimuli.Add(new Pair(rightKey, word));
            }
        }
        
        ShuffleStimuli();
        
        // begin new dataset
        if (currentDataSet != null)
        {
            allData.Add(currentDataSet);
        }
        currentDataSet = new DataSet();
    }

    /// <summary>
    /// Runs one test block
    /// </summary>
    /// <param name="practice">practice needs to be evaluated separately</param>
    /// <returns></returns>
    private IEnumerator RunTestBlock(bool practice = false)
    {
        var index = 0;
        foreach (var pair in stimuli)
        {
            // show stimuli
            testText.text = pair.stimulus;
            lastStimulusTime = Time.time;
            
            yield return WaitForInput(pair, index);
            ++index;
        }
    }
    
    /// <summary>
    /// Fisher-Yates
    /// </summary>
    private void ShuffleStimuli()  
    {  
        var n = stimuli.Count;  
        while (n > 1) 
        {  
            n--;  
            var k = Random.Range(0, n + 1);  
            (stimuli[k], stimuli[n]) = (stimuli[n], stimuli[k]);
        }  
    }

    /// <summary>
    /// Returns after pressing the correct key.
    /// </summary>
    /// <param name="pair">A pair of the correct key code and the stimulus it belongs to</param>
    /// <param name="order">Order of the stimulus in the set</param>
    /// <returns></returns>
    private IEnumerator WaitForInput(Pair pair, int order)
    {
        var errorTime = 0f;
        var firstErrorTime = 0f;
        var numErrors = 0;
        while (true)
        {
            yield return new WaitUntil(GetInput);
            yield return null;

            if (lastInput == pair.key)
            {
                // save data
                //currentDataSet.RecordEntryArch(pair.stimulus, Time.time - lastStimulusTime, numErrors, errorTime);
                currentDataSet.RecordEntry(pair.stimulus, currentTestType, pair.key, GetWordCategory(pair.stimulus),
                    Time.time - lastStimulusTime, numErrors, 
                    numErrors > 0 ? Time.time - errorTime : 0, order, firstErrorTime);
                
                redCross.alpha = 0;
                
                break;
            }
            else if (lastInput == GetOppositeKey(pair.key))
            {
                // first error
                if (numErrors == 0)
                {
                    firstErrorTime = Time.time - lastStimulusTime;
                }

                errorTime = Time.time;
                ++numErrors;
                
                redCross.alpha = 1;
            }
            
        }
    }

    private KeyCode GetOppositeKey(KeyCode key)
    {
        return key == leftKey ? rightKey : leftKey;
    }

    /// <summary>
    /// Returns true if either the left or right key was pressed and caches the last key pressed.
    /// </summary>
    /// <returns></returns>
    private bool GetInput()
    {
        if (Input.GetKeyDown(leftKey))
        {
            lastInput = leftKey;
            return true;
        }
        if (Input.GetKeyDown(rightKey))
        {
            lastInput = rightKey;
            return true;
        }

        lastInput = KeyCode.None;
        return false;
    }
    
    /// <summary>
    /// Returns true for wither left or right key.
    /// </summary>
    /// <returns></returns>
    private static bool GetAnyInput()
    {
        var ret = Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey);
        
        if (ret) Debug.Log("pressing down correct key to continue");

        return ret;
    }

    private WordCategory GetWordCategory(string word)
    {
        if (goodWords.Contains(word)) return WordCategory.Good;
        if (badWords.Contains(word)) return WordCategory.Bad;
        if (gayWords.Contains(word)) return WordCategory.Gay;

        return WordCategory.NA;
    }
    
}
