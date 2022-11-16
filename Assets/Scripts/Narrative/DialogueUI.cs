using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

// should be on a canvas/ui panel
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public IEnumerator PlaySingleLine(string line)
    {
        yield return Open();
        yield return PlayLine(line);
        yield return new WaitUntil(() => Input.anyKeyDown);
        yield return Close();
    }
    
    public IEnumerator Open()
    {
        // pause the game
        gameManager.PauseGame(true);
        
        dialogueText.text = ""; // clear the text box
        
        var tween = canvasGroup.DOFade(1, .1f);
        yield return tween.WaitForCompletion();
    }
    
    public IEnumerator PlayLine(string line)
    {
        dialogueText.text = "";
        
        // write out the line - unskippable
        foreach (var t in line)
        {
            dialogueText.text += t;
            yield return new WaitForSeconds(.01f);
        }
        
        yield return null;
    }

    public IEnumerator Close()
    {
        // unpause the game
        gameManager.PauseGame(false);
        var tween = canvasGroup.DOFade(0, .1f);
        yield return tween.WaitForCompletion();
    }
}
