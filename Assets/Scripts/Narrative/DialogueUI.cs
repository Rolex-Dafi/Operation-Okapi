using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// should be on a canvas/ui panel
public class DialogueUI : MonoBehaviour
{

    public void Open()
    {
        
    }
    
    public IEnumerator PlayLine(string line)
    {
        // write out the line - unskippable
        
        yield return null;
    }
}
