﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GrammerLevelController : MonoBehaviour
{
    private string _Keyresponse = "";
    private string _response = "";
    private PauseManager pauseMenu;
    private VolumeController setVolume; 
    private GrammarRecognizer gr;

    private void Start() {
        _response = "";
        _Keyresponse = "";
        // Load in the XML file
        gr = new GrammarRecognizer(Path.Combine(Application.streamingAssetsPath,  
        "Level.xml"), ConfidenceLevel.Low); 
        
        // Begin the Grammar Recogniser
        gr.OnPhraseRecognized += GR_OnPhraseRecognized;
        gr.Start();
        Debug.Log("Grammer Load and Recogniser Started!");

        pauseMenu = FindObjectOfType<PauseManager>();
        setVolume = FindObjectOfType<VolumeController>();
    }
    private void Update() {
        //Switch between the Words Spoken
        RestartCommand();
        PauseGameCommands();
        VolumeCommands();
        PauseCommands();
        
    }
    // Checking that game is not paused and Command to pause game
    private void PauseGameCommands(){
        // Each Key has an action assign and is called to determine the different phrases said
        if(_Keyresponse == "pause" && !PauseManager.IsPaused){
            switch (_response)
            {
                // Pause the Game Rule
                case "pause the game":
                    pauseMenu.Pause();
                    break;
            }
            _response = "";
        }
    }
    // Volume Commands for Sound of App 
    private void VolumeCommands(){
        if(_Keyresponse == "volume" && PauseManager.IsPaused){
            switch (_response)
            {
                // Volume Rules
                case "decrease":
                    setVolume.Volume(-0.1f);   
                    break;
                case "increase":
                    setVolume.Volume(0.1f);   
                    break;
                default:
                    _response = "";
                    break;
            }
            _response = "";
        }
    }
    // Voice Control for Pause Menu
    private void PauseCommands(){
        if(_Keyresponse == "pause" && PauseManager.IsPaused ){
            switch (_response)
            {
                // Pause Menu Rules
                case "continue":
                    pauseMenu.Resume();
                    break;
                case "restart":
                    pauseMenu.RestartLevel();
                    break;
                case "quit":
                    pauseMenu.BackToMenu(); 
                    break;
                default:
                    _response = "";
                    break;
            }
            _response = "";
        }
        
    }
    // Voice Control for when the game is over and gameover  
    // screen is active
    private void RestartCommand(){
        if(_Keyresponse == "pause" && HealthController.active){
            switch (_response)
            {
                case "restart":
                    pauseMenu.RestartLevel();
                    break;
                case "quit":
                    pauseMenu.BackToMenu(); 
                    break;
                default:
                    _response = "";
                    break;
            }
            _response = "";
        }
    }
    // Menu Comand when in Main Menu
    private void MenuCommands(){
        if(_Keyresponse == "menu"){
            switch (_response)
            {
                // Menu Rules
                case "start":
                    SceneManager.LoadScene(1);
                    break;
                case "exit":
                    Application.Quit();
                    Debug.Log("Game is exiting");
                    break;
                default:
                    _response = "";
                    break;
            }
            _response = "";
        }
    }

    // Determine the phrases match the user input from the XML file.  
    private void GR_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder message = new StringBuilder();
        //Read the semantic meanings from the args passed in
        SemanticMeaning[] meanings = args.semanticMeanings;
        //use foreach to get all the meanings.
        foreach (SemanticMeaning meaning in meanings)
        {
            // Get the Key and Value Strings from Input if matches.
            string keyString = meaning.key.Trim();
            var valueString = meaning.values[0].Trim();

            message.Append("Key " + keyString + "Value " + valueString + " ");

            // Assign the Inputs to new Values for future use.
            _response = valueString;
            _Keyresponse = keyString;
        }
        //use a string builder to create the string and out to the user.
        Debug.Log(message);
    }
    // Stops the Grammer Recognizer
    private void OnApplicationQuit()
    {
        if (gr == null || !gr.IsRunning) return;
        gr.OnPhraseRecognized -= GR_OnPhraseRecognized;
        gr.Stop();
    }
    // When Destroy then stop Grammer Recognizer
    private void OnDestroy()
    {
        if (gr != null)
        {
            gr.Stop();
            gr.OnPhraseRecognized -= GR_OnPhraseRecognized;
            gr.Dispose();
            gr = null; 
        }
    }
}
