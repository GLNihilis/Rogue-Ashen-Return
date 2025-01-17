using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public GameObject window;
    public GameObject interact;
    public TMP_Text dialogueText;
    public List<string> dialogues;
    public float writingSpeed;
    private int index; // Index on dialogues
    private int characterIndex;
    public bool isStarted;
    public bool waitForNext;
    public bool skipTyping;

    private void ToggleWindow(bool show)
    {
        window.SetActive(show);
    }

    public void ToggleInteract(bool show)
    {
        interact.SetActive(show);
    }

    private void Awake()
    {
        ToggleWindow(false);
        ToggleInteract(false);
    }

    public void StartDialogue()
    {
        if (isStarted) return;

        isStarted = true;

        // Show the window
        ToggleWindow(true);

        // Hide the interact
        ToggleInteract(false);

        GetDialogue(0);
    }

    private void GetDialogue(int i)
    {
        // Start index at zero
        index = i;

        // Reset the character index
        characterIndex = 0;

        // Clear the dialogue
        dialogueText.text = string.Empty;

        StartCoroutine(Writing());
    }

    public void EndDialogue()
    {
        // Hide the window
        StopCoroutine(Writing());
        ToggleWindow(false);
        isStarted = false;
        waitForNext = false;
    }

    public IEnumerator Writing()
    {
        string currentDialogue = dialogues[index];

        #region
        // Write the character
        //dialogueText.text += currentDialogue[characterIndex];

        //// Increase the character index
        //characterIndex++;

        // The end of sentences
        //if (characterIndex < currentDialogue.Length)
        //{
        //    // Wait for seconds to finish the dialogue
        //    yield return new WaitForSeconds(writingSpeed);

        //    // Restart the same progress
        //    StartCoroutine(Writing());
        //}
        //else
        //{
        //    waitForNext = true;
        //    ToggleInteract(true);
        //}
        #endregion
        #region
        //int charactersToShow = 3;
        //int remainingCharacters = currentDialogue.Length - characterIndex;

        //for (int i = 0; i < Mathf.Min(charactersToShow, remainingCharacters); i++)
        //{
        //    dialogueText.text += currentDialogue[characterIndex];
        //    characterIndex++;
        //}
        #endregion
        #region
        while (characterIndex < currentDialogue.Length)
        {
            if (skipTyping)
            {
                dialogueText.text = currentDialogue;
                skipTyping = false;
                break;
            }

            dialogueText.text += currentDialogue[characterIndex];
            characterIndex++;
            ToggleInteract(false);
            yield return new WaitForSeconds(writingSpeed);
            ToggleInteract(true);
        }

        waitForNext = true;
        #endregion
    }

    private void Update()
    {
        // If it not started ignore the remaining code
        if (!isStarted) return;

        if (waitForNext && Input.GetKeyDown(KeyCode.E))
        {
            waitForNext = false;
            index++;

            if (index < dialogues.Count)
            {
                GetDialogue(index);
            }
            else
            {
                EndDialogue();
            }
        }

        #region
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skipTyping = true;
        }
        #endregion
    }
}
