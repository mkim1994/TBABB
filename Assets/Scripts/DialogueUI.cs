﻿/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using Yarn.Unity;
using DG.Tweening;

/// Displays dialogue lines to the player, and sends
/// user choices back to the dialogue system.

/** Note that this is just one way of presenting the
 * dialogue to the user. The only hard requirement
 * is that you provide the RunLine, RunOptions, RunCommand
 * and DialogueComplete coroutines; what they do is up to you.
 */
public class DialogueUI : Yarn.Unity.DialogueUIBehaviour
{

    /// The object that contains the dialogue and the options.
    /** This object will be enabled when conversation starts, and 
     * disabled when it ends.
     */
    public GameObject dialogueContainer;

    /// The UI element that displays lines
    public Text lineText;

    /// A UI element that appears after lines have finished appearing
    public GameObject continuePrompt;

    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;

    /// How quickly to show the text, in seconds per character
    [Tooltip("How quickly to show the text, in seconds per character")]
    public float textSpeed = 0.025f;

    /// The buttons that let the user choose an option
    public List<Button> optionButtons;

    /// Make it possible to temporarily disable the controls when
    /// dialogue is active and to restore them when dialogue ends
    public RectTransform gameControlsContainer;

    private float textTime;
    private float currentTime;
    private float additionalTime;

    public bool debugMode;

    Sequence textAnim;

    void Awake()
    {
        DOTween.Init();
        // Start by hiding the container, line and option buttons
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

        lineText.gameObject.SetActive(false);

        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Hide the continue prompt if it exists
        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        textTime = -1f;
        additionalTime = 0.3f;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Return)){
            debugMode = !debugMode;
            Debug.Log("dialogue debug mode: " + debugMode);
        }
        /*if(debugMode && (int)textTime != -1){
            if(Input.GetKeyDown(KeyCode.Return)){
                textTime = -2f;
            }
        }*/
      /*  if ((int)textTime != -1 && textTime > -2f)
        {
            if (debugMode)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    textTime = -2f;
                }
            }

            if (Time.time - currentTime > textTime)
            {
                textTime = -2f;
            }

        }*/

    }

    public void ChooseOption(int o){
        if(optionButtons[o].gameObject.activeSelf){
            SetOption(o);
        }
    }

    /// Show a line of dialogue, gradually
    public override IEnumerator RunLine(Yarn.Line line)
    {
        // Show the text
        lineText.gameObject.SetActive(true);
        DOTween.Kill(lineText);
        lineText.color = new Color(1, 1, 1, 1);
        string reline = line.text;
        string result = reline.Replace("Customer: ", "");

        if (!debugMode)
        {

            if (textSpeed > 0.0f)
            {
                // Display the line one character at a time
                var stringBuilder = new StringBuilder();

                foreach (char c in result)
                {
                    stringBuilder.Append(c);
                    lineText.text = stringBuilder.ToString();
                    yield return new WaitForSeconds(textSpeed);
                }
                //determining the line duration on the screen
                textTime = result.Length * 0.05f;
                // currentTime = Time.time;
                yield return new WaitForSeconds(textTime);
                lineText.DOFade(0f, additionalTime).OnComplete(() => textTime = -2f);
            }
            else
            {
                // Display the line immediately if textSpeed == 0
                lineText.text = result;
            }
        } else{
            lineText.text = result;

            yield return new WaitForSeconds(0.0001f);
        }

        // Show the 'press any key' prompt when done, if we have one
        if (continuePrompt != null)
            continuePrompt.SetActive(true);

        // Wait for any user input
        /*while (Input.anyKeyDown == false)
        {*/
        //wait for time
        if (!debugMode)
        {
            while ((int)textTime != -2)
            {
                yield return null;
            }

        } else{
            while(!Input.GetKeyDown(KeyCode.Space)){
                yield return null;
            }
        }

        textTime = -1f;
        additionalTime = 1f;
        // Hide the text and prompt
        lineText.gameObject.SetActive(false);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

    }

    /// Show a list of options, and wait for the player to make a selection.
    public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                            Yarn.OptionChooser optionChooser)
    {
        // Do a little bit of safety checking
        if (optionsCollection.options.Count > optionButtons.Count)
        {
            Debug.LogWarning("There are more options to present than there are" +
                             "buttons to present them in. This will cause problems.");
        }

        // Display each option in a button, and make it visible
        int i = 0;
        foreach (var optionString in optionsCollection.options)
        {
            optionButtons[i].gameObject.SetActive(true);
            if (Services.GameManager.playerInput.isUsingController)
            {
                optionButtons[i].transform.GetChild(1).gameObject.SetActive(false);
                optionButtons[i].transform.GetChild(2).gameObject.SetActive(true);            
            }
            optionButtons[i].GetComponentInChildren<Text>().text = optionString;
            i++;
        }

        // Record that we're using it
        SetSelectedOption = optionChooser;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (SetSelectedOption != null)
        {
            yield return null;
        }

        // Hide all the buttons
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    /// Called by buttons to make a selection.
    public void SetOption(int selectedOption)
    {

        // Call the delegate to tell the dialogue system that we've
        // selected an option.
        SetSelectedOption(selectedOption);

        // Now remove the delegate so that the loop in RunOptions will exit
        SetSelectedOption = null;
    }

    /// Run an internal command.
    public override IEnumerator RunCommand(Yarn.Command command)
    {
        // "Perform" the command
        Debug.Log("Command: " + command.text);

        yield break;
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted()
    {
        Debug.Log("Dialogue starting!");

        // Enable the dialogue controls.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(true);

        // Hide the game controls.
        if (gameControlsContainer != null)
        {
            gameControlsContainer.gameObject.SetActive(false);
        }

        yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete()
    {
        Debug.Log("Complete!");

        // Hide the dialogue interface.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

        // Show the game controls.
        if (gameControlsContainer != null)
        {
            gameControlsContainer.gameObject.SetActive(true);
        }

        yield break;
    }
    [YarnCommand("settime")]
    public void DialogueTimeSet(float addtl)
    {
        additionalTime = addtl;
    }

}
