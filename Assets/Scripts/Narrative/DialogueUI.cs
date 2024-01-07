using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the dialogue UI. Should be on a canvas or UI panel.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Color merchantColor;
    [SerializeField] private Color exColor;
    
    [SerializeField] private Image avatar;
    [SerializeField] private Image textBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private Sprite merchantAvatar;
    [SerializeField] private Sprite exAvatarGay;
    [SerializeField] private Sprite exAvatarStraight;
    
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    
    /// <summary>
    /// Initializes the class with a game manager.
    /// </summary>
    /// <param name="gameManager"></param>
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Changes the character avatar next to the dialogue bubble.
    /// </summary>
    /// <param name="merchant"></param>
    public void ChangeAvatar(bool merchant = true)
    {
        avatar.sprite = merchant ? merchantAvatar : Utility.gayVersion ? exAvatarGay : exAvatarStraight;
        textBox.color = merchant ? merchantColor : exColor;
    }
    
    /// <summary>
    /// Plays a single line of dialogue and waits for player input.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public IEnumerator PlaySingleLine(string line)
    {
        yield return Open();
        yield return PlayLine(line);
        yield return new WaitUntil(() => Input.anyKeyDown);
        yield return Close();
    }
    
    /// <summary>
    /// Pauses the game and opens the dialogue UI.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Open()
    {
        // pause the game
        gameManager.PauseGame(true);
        
        dialogueText.text = ""; // clear the text box
        
        var tween = canvasGroup.DOFade(1, .1f);
        yield return tween.WaitForCompletion();
    }
    
    /// <summary>
    /// Displays a line of dialogue.
    /// </summary>
    /// <param name="line">The dialogue line to display</param>
    /// <returns></returns>
    public IEnumerator PlayLine(string line)
    {
        dialogueText.text = "";
        
        // write out the line - unskippable
        foreach (var t in line)
        {
            dialogueText.text += t;
            yield return new WaitForSeconds(.04f);
        }
        
        yield return new WaitForSeconds(.04f);;
    }

    /// <summary>
    /// Closes the dialogue UI.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Close()
    {
        // unpause the game
        gameManager.PauseGame(false);
        var tween = canvasGroup.DOFade(0, .1f);
        yield return tween.WaitForCompletion();
    }
}
