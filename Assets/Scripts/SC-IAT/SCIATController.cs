using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private Image redCross;
    
    private string[] instructionMessages =
    {
        // welcome message
        "Welcome to the experiment\n" +
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
        "good",
        "amazing"
    };
    
    private string[] badWords =
    {
        "bad",
        "awful"
    };

    private string[] gayWords =
    {
        "lesbian",
        "bisexual",
        "gay",
        "LGBT"
    };
    
    private List<Pair> stimuli = new List<Pair>();

    private const KeyCode leftKey = KeyCode.E;
    private const KeyCode rightKey = KeyCode.I;

    private KeyCode lastInput;
    
    private void Awake()
    {
        StartCoroutine(TestLoop());
    }

    private IEnumerator TestLoop()
    {
        // welcome message
        instructionsText.text = instructionMessages[0];
        yield return new WaitUntil(GetAnyInput);
        yield return new WaitForEndOfFrame();
        
        // --------------------------------------------
        // set instructions
        instructionsText.text = instructionMessages[1];
        
        // and corners
        leftText.text = goodWord + "\n OR\n" + gayShit;
        rightText.text = badWord;
        testText.text = "";

        yield return new WaitUntil(GetAnyInput);
        yield return new WaitForEndOfFrame();

        // start test block
        instructionsText.text = "";

        //yield return new WaitForSeconds(.1f);
        
        // 1st block - 2 blocks as one continuous
        FillStimuliB1(1); // 1 repetition for practice
        yield return RunTestBlock(true);
        
        FillStimuliB1(3); // 3 repetitions for test
        yield return RunTestBlock(false);
        
        // --------------------------------------------
        // set instructions
        instructionsText.text = instructionMessages[2];
        
        // and corners
        leftText.text = goodWord;
        rightText.text = badWord + "\n OR\n" + gayShit;
        testText.text = "";
        
        yield return new WaitUntil(GetAnyInput);
        yield return new WaitForEndOfFrame();
        
        // start test block
        instructionsText.text = "";

        // 2nd block - 2 blocks as one continuous
        FillStimuliB2(1); // 1 repetition for practice
        yield return RunTestBlock(true);
        
        FillStimuliB2(3); // 3 repetitions for test
        yield return RunTestBlock(false);
        
        // outro message
        instructionsText.text = instructionMessages[3];
        yield return new WaitUntil(GetAnyInput);
        yield return new WaitForEndOfFrame();
        
        // TODO got to different scene/show formr link
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
    }

    /// <summary>
    /// Runs one test block
    /// </summary>
    /// <param name="practice">practice needs to be evaluated separately</param>
    /// <returns></returns>
    private IEnumerator RunTestBlock(bool practice = false)
    {
        foreach (var pair in stimuli)
        {
            // show stimuli
            testText.text = pair.stimulus;
            
            yield return WaitForInput(pair); 
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
    /// Only returns after pressing the correct key
    /// </summary>
    /// <param name="pair">A pair of the correct key code and the stimulus it belongs to</param>
    /// <returns></returns>
    private IEnumerator WaitForInput(Pair pair)
    {
        while (true)
        {
            yield return new WaitUntil(GetInput);
            yield return new WaitForEndOfFrame();

            if (lastInput == pair.key)
            {
                // TODO save to data - time
                
                Debug.Log("correct answer for " + pair);
                
                break;
            }
            else
            {
                // incorrect answer 
                
                // TODO save to data - time
        
                Debug.Log("!!! incorrect answer for " + pair);
                
                // flash red cross
                StartCoroutine(FlashRed());
            }

        }
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

    /// <summary>
    /// Flashes the red cross graphics.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashRed()
    {
        redCross.DOKill();
        var tween = redCross.DOFade(1, .1f);
        yield return tween.WaitForCompletion();
        yield return new WaitForSeconds(.2f);
        tween = redCross.DOFade(0, .1f);
        yield return tween.WaitForCompletion();
    }
    
}
