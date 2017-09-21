using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This is an Enum that contains the different possible types of Bombs/Power-ups
/// </summary>
public enum GameModes
{
    WordGuess,
    Relax
}

/// <summary>
/// GameController Class handles all of the starting and stopping of rounds.  It also constantly monitors the game
/// state so that as soon as a lose/time up conditions is met... it'll end the game.  This class also keeps the UI
/// text fields updated with the score.
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController GameControllerInstance;        //static reference to this game controller
    private CountdownTimer roundTimer;                          //a reference to the countdown timer(connected to same parent body)
    [Header("Select GameMode for Testing Purposes.")]
    public GameModes gameModes;                                 //our game controllers GameModes Enum variable... gameModes
    [Header("Drop the 'GameOverCanvas' Here")]
    public GameObject gameOverPanel;                            //the gameOverPanel that hold the Game Over UI Image
    public GameObject[] blueXClassicMode;                       //classic mode blue x's.  The 3 X's on the UI that you start with in Classic Mode
    public GameObject[] redXClassicMode;                        //Classic mode red x's.  The XXX on the UI (under the Blue X's), that are show when you miss fruit.
    public Text classicText;                                    //the txt that is the classic mode current store
    public Text WordGuessText;                                     //the txt that is the WordGuess mode current store
    public Text relaxText;                                      //the txt that is the relax mode current store
    public Text classicHighestText;                             //the txt that is the classic mode highest store
    public Text WordGuessHighestText;                              //the txt that is the WordGuess mode highest store
    public Text relaxHighestText;                               //the txt that is the relax mode highest store
    public Text wordText;
    public Text suggestion;
    public Text antidot;
    public GameObject slicerGO;                                 //a reference to our FNCTouchSlicer
    private bool gameHasStarted;                                //boolean game has started???
    public bool gameIsRunning;                                 //boolean game is running??
    public float waitForMenuAtEnd;                              //how long to wait before the settings/pause menu pops up?
    public int numAntidot;
    private Vector3 g = Physics.gravity;
    // Use this for pre-initialization
    void Awake()
    {
        //our instance reference to this GameController is assigned THIS.
        GameControllerInstance = this;
        //roundTimer reference is assigned the CountdownTimer that is connected to this gameobject.
        roundTimer = GetComponent<CountdownTimer>();
        //we make sure the gameOverPanel is inactive... (the game just started, son!)
        gameOverPanel.SetActive(false);
        //boolean gameIsRunning is True.
        gameIsRunning = true;
        wordText.text = "";
        suggestion.text = "Suggest: ";
        numAntidot = 0;
    }


    //OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {

        wordText.text = "";
        suggestion.text = "Suggest: ";
        numAntidot = 0;
        //we zero out all of our static variables dealing with score.
        GameVariables.FruitMissed = 0;
        GameVariables.ClassicModeScore = 0;
        GameVariables.WordGuessModeScore = 0;
        GameVariables.RelaxModeScore = 0;
        //then we RESET the splatterQuadSpawnDistance to 55f.  This static var is reset every round.  we increment it when we spawn a splatter, so that
        //they always spawn on top of the previous one... they stop around 45f worse case scenario, and in orthographic mode that is still acceptable.
        GameVariables.splatterQuadSpawnDistance = 55f;
    }

    // Use this for initialization
    void Start()
    {

        //start coroutine... ChooseGameModeAndCallRoundStart()... Long Name
        StartCoroutine(ChooseGameModeAndCallRoundStart());

        //and we start FNC_SlowUpdate.  We are trying to offload some secondary methods/workloads that don't need as frequent of updates.
        InvokeRepeating("FNC_SlowUpdate", 0.33f, 0.33f);

    }

    /// <summary>
    /// FNC_SlowUpdate Method again... Run Unimportant stuff here.  In this class at the least the UI updates will be in here.
    /// </summary>
    private void FNC_SlowUpdate()
    {
        //if game is running we will...
        if (gameIsRunning)
        {
            //call the updateUIText method.
            UpdateUIText();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if game is running we will...
        if (gameIsRunning)
        {
            MonitorGameState();
            //UpdateUIText();//moved to SlowUpdat()
        }

    }


    /// <summary>
    /// UpdateUIText does exactly what you would imagine.  It updates UI Text.
    /// </summary>
    private void UpdateUIText()
    {
        //we update all the current scores.
        WordGuessText.text = GameVariables.WordGuessModeScore.ToString();
        classicText.text = GameVariables.ClassicModeScore.ToString();
        relaxText.text = GameVariables.RelaxModeScore.ToString();
        //we update the highest scores.
        WordGuessHighestText.text = GameVariables.WordGuessModeHighestScore.ToString();
        classicHighestText.text = GameVariables.ClassicModeHighestScore.ToString();
        relaxHighestText.text = GameVariables.RelaxModeHighestScore.ToString();
        wordText.text = FNCTouchSlicer.getSlice();
        suggestion.text = "Suggest: " + LauncherController.suggestion;
        antidot.text = numAntidot.ToString();
    }


    /// <summary>
    /// The UpdatePlayerExperienceAndLevel method adds the destroyed fruit the players "Experience". and
    /// then Stores the new experience in PlayerPrefs/
    /// </summary>
    /// <param name="fruitDestroyed"></param>
    private void UpdatePlayerExperienceAndLevel(int fruitDestroyed)
    {
        //update Experience with FruitDestroyed value
        GameVariables.Experience += fruitDestroyed;
        //save Experience in PlayerPrefs
        PlayerPrefs.SetInt(Tags.experience, GameVariables.Experience);

    }


    /// <summary>
    /// This UpdateHighestScore takes in the ending round score(if it is higher than the current highest score) and then depending
    /// on the selected mode/variable It will write the update to PlayerPrefs.
    /// </summary>
    /// <param name="amt"></param>
    private void UpdateHighestScore(int amt)
    {
        switch (gameModes)
        {
            //if in WordGuess game mode...
            case GameModes.WordGuess:
                //ClassicModeHighestScore gets assigned "amt"
                GameVariables.WordGuessModeHighestScore = amt;

                //store the new amount in PlayerPrefs
                PlayerPrefs.SetInt(Tags.highestWordGuessScore, GameVariables.WordGuessModeScore);
                break;
            //if in Relax game mode...
            case GameModes.Relax:

                //ClassicModeHighestScore gets assigned "amt"
                GameVariables.RelaxModeHighestScore = amt;

                //store the new amount in PlayerPrefs
                PlayerPrefs.SetInt(Tags.highestRelaxScore, GameVariables.RelaxModeScore);
                break;

            default:
                //Debug.Log("Danger, WIll Robinson, Danger");
                break;

        }

    }

    /// <summary>
    /// This method stops the game time.
    /// </summary>
    private void StopTime()
    {
        //set Time.timeScale to 0f;
        Time.timeScale = 0.0f;
    }


    /// <summary>
    /// A redundant local private method to access our settings/menu instance var(and the method CallPauseAndMenu()).  So that we can invoke this
    /// Method about a half a second after round end. 
    /// </summary>
    private void LocalPauseAndSettingsMenuCall()
    {
        //SettingsAndPauseMenu.instance.CallPauseAndMenu();

        //call the settings menu onto the screen(without the pause).
        SettingsAndPauseMenu.instance.CallMenuOnly();
    }


    /// <summary>
    /// MonitorGameState is the main Method in the GameController Class.  The Method 
    /// </summary>
    private void MonitorGameState()
    {

        ////////////////////////////////
        ////_____WordGuess-MODE_______////
        ///////////////////////////////


        //if the current game mode is WordGuess...
        if (gameModes == GameModes.WordGuess)
        {
            //if roundTimer.timeLeft is less than or equal to 0.01 && gameHasStarted
            //Physics.gravity = g + new Vector3(0, -0.1f * GameVariables.WordGuessModeScore/3, 0);
            if (roundTimer.timeLeft <= 0.01 && gameHasStarted)
            {
                //Debug.Log("Game Over");

                //activate the gameOverPanel
                gameOverPanel.SetActive(true);

                //FNCTouchSlicer gets disabled
                slicerGO.SetActive(false);

                //invoke our settings/menu canvas to provoke a response...
                Invoke("LocalPauseAndSettingsMenuCall", waitForMenuAtEnd);

                //if GameVariables.WordGuessModeHighestScore is less than GameVariables.WordGuessModeScore then clearly we need to update the highest score
                if (GameVariables.WordGuessModeHighestScore < GameVariables.WordGuessModeScore)
                {
                    //we update the highest score by calling UpdateHighestScore(GameVariables.WordGuessModeScore(WordGuessModeScore))
                    UpdateHighestScore(GameVariables.WordGuessModeScore);
                }

                //We also need to update player experience... we pass in our new score to do that.
                UpdatePlayerExperienceAndLevel(GameVariables.WordGuessModeScore);

                //and we set gameIsRunning to false
                gameIsRunning = false;

            }
            //else... game clock is still running, and we should still be launching...
            else
            {
                //Access the LaunchControllers static Instance and call ReduceLaunchTimersAndLaunchObjects...
                LauncherController.LaunchControllerInstance.ReduceLaunchTimersAndLaunchObjects();
            }
        }


        ////////////////////////////////
        ////______RELAX-MODE_______////
        ///////////////////////////////


        //if the current game mode is WordGuess...
        if (gameModes == GameModes.Relax)
        {
            //if roundTimer.timeLeft is less than or equal to 0.01 && gameHasStarted
            if (roundTimer.timeLeft <= 0.01 && gameHasStarted)
            {
                //Debug.Log("Game Over");

                //activate the gameOverPanel
                gameOverPanel.SetActive(true);

                //FNCTouchSlicer gets disabled
                slicerGO.SetActive(false);

                //invoke our settings/menu canvas to provoke a response...
                Invoke("LocalPauseAndSettingsMenuCall", waitForMenuAtEnd);

                //if GameVariables.WordGuessModeHighestScore is less than GameVariables.WordGuessModeScore then clearly we need to update the highest score
                if (GameVariables.RelaxModeHighestScore < GameVariables.RelaxModeScore)
                {
                    //we update the highest score by calling UpdateHighestScore(GameVariables.WordGuessModeScore(WordGuessModeScore))
                    UpdateHighestScore(GameVariables.RelaxModeScore);
                }

                //We also need to update player experience... we pass in our new score to do that.
                UpdatePlayerExperienceAndLevel(GameVariables.RelaxModeScore);

                //and we set gameIsRunning to false
                gameIsRunning = false;

            }
            //else... game clock is still running, and we should still be launching...
            else
            {
                //Access the LaunchControllers static Instance and call ReduceLaunchTimersAndLaunchObjects...
                LauncherController.LaunchControllerInstance.ReduceLaunchTimersAndLaunchObjects();

            }
        }
    }


    /// <summary>
    /// Method called by Coroutine to start a Classic Mode round.  If hasTimer is true then we call StartTimer on our CountdownTimer Class, and we
    /// pass the amount of time that should be on the clock.  Then we change the boolean "gameHasStarted" which will remain true until round end.
    /// </summary>
    /// <param name="hasTimer"></param>
    /// <param name="gameTime"></param>
    private void StartClassicModeGame(bool hasTimer, float gameTime)
    {
        //if hasTimer is true... (in classic mode it should be false, so the following code block will not be run... (it'll pick up at "roundTimer.DisableTimerText();") **(classic mode does not)
        if (hasTimer)
        {
            //this should not happen in classic mode....
            roundTimer.StartTimer(gameTime);
        }
        //now we call roundTimer.DisableTimerText (Note:This should only remove timer in classic mode (because timer is not set(it should remove the timer display text)
        roundTimer.DisableTimerText();

        //now we set the gameHasStarted boolean to true! Game is Starting!!!
        gameHasStarted = true;

    }

    /// <summary>
    /// Method called by Coroutine to start a WordGuess Mode round.  If hasTimer is true then we call StartTimer on our CountdownTimer Class, and we
    /// pass the amount of time that should be on the clock.  Then we change the boolean "gameHasStarted" which will remain true until round end.
    /// </summary>
    /// <param name="hasTimer"></param>
    /// <param name="gameTime"></param>
    private void StartWordGuessModeGame(bool hasTimer, float gameTime)
    {
        //if has timer is true then we...  **(WordGuess mode does)
        if (hasTimer)
        {
            //will call StartTimer and pass in the gameTime(60f) for WordGuess Mode...
            roundTimer.StartTimer(gameTime);

        }

        //then we set gameHasStarted to true, and start slicing!
        gameHasStarted = true;
    }

    /// <summary>
    /// Method called by Coroutine to start a Relax Mode round.  If hasTimer is true then we call StartTimer on our CountdownTimer Class, and we
    /// pass the amount of time that should be on the clock.  Then we change the boolean "gameHasStarted" which will remain true until round end.
    /// </summary>
    /// <param name="hasTimer"></param>
    /// <param name="gameTime"></param>
    private void StartRelaxModeGame(bool hasTimer, float gameTime)
    {
        //if has timer is true then we...  **(relax mode does)
        if (hasTimer)
        {
            //will call StartTimer and pass in the gameTime(60f) for WordGuess Mode...
            roundTimer.StartTimer(gameTime);

        }

        //then we set gameHasStarted to true, and start slicing!
        gameHasStarted = true;

    }

    /// <summary>
    /// This Method Zeros out the "timeLeft" and disables the FNCTouchSlicer.
    /// </summary>
    private void ZeroGameTimeAndEndGame()
    {
        //assign a value of 0f to roundTimer.timeLeft.
        roundTimer.timeLeft = 0f;

        //disable FNC.touchSlicer GO
        slicerGO.SetActive(false);

    }


    ///// <summary>
    ///// This method Ends the Game. It activates all the Red X's, Disables the Blue X's, and sets FruitMissed to 5.
    ///// </summary>
    //public void TakeFruitAndEndGame()
    //{
    //    //set our static var FruitMissed to 5
    //    GameVariables.FruitMissed = 5;

    //    //if the gameModes is equal to GameModes.Classic
    //    if (gameModes == GameModes.Classic)
    //    {
    //        //deactivate the slicer Game Object
    //        slicerGO.SetActive(false);

    //        //use a for loop to go through all of the Blue x's...
    //        for (int i = 0; i < blueXClassicMode.Length; i++)
    //        {
    //            ///if we find a blue X that is active then we...
    //            if (blueXClassicMode[i].activeInHierarchy)
    //            {
    //                //disable it..
    //                blueXClassicMode[i].SetActive(false);
    //            }
    //        }
    //        //use for loop to go through all of the Red x's...
    //        for (int i = 0; i < redXClassicMode.Length; i++)
    //        {
    //            //if we find a Red X that is active then we...
    //            if (!redXClassicMode[i].activeInHierarchy)
    //            {
    //                //disable it...
    //                redXClassicMode[i].SetActive(true);
    //            }
    //        }
    //    }
    //}



    /// <summary>
    /// This Method is called at the start of the scene... It starts the game with the appropriate gameObjects/Settings
    /// </summary>
    /// <returns></returns>
    IEnumerator ChooseGameModeAndCallRoundStart()
    {
        switch (gameModes)
        {


            //if we are in GameModes.Classic then we need to...
            //case GameModes.Classic:

            //    //set the current classic score to zero...
            //    GameVariables.ClassicModeScore = 0;

            //    //call the method StartClassicModeGame with these variables... false(hasTimer), and 0f(time to set Timer for).
            //    StartClassicModeGame(false, 0f);

            //    //loop through the number of x's in the array...
            //    for (int i = 0; i < blueXClassicMode.Length; i++)
            //    {
            //        //for each blue x we set it active...
            //        blueXClassicMode[i].SetActive(true);
            //    }

            //    //then we are done...
            //    break;



            //if we are in GameModes.WordGuess then we need to...
            case GameModes.WordGuess:

                //wait for a few seconds while "60 seconds" and "Go!!" text slide on screen and then fade.
                yield return new WaitForSeconds(3.5f);

                //our Static variable GameVariables.WordGuessModeScore needs to be set to zero (to make sure it is cleared)
                GameVariables.WordGuessModeScore = 0;

                //then we call StartWordGuessModeGame() and pass in true(hasTimer), and 60f(TimerTime)
                StartWordGuessModeGame(true, 240f);

                //break... we are done.
                break;



            //if we are in GameModes.Relax then we need to...
            case GameModes.Relax:

                //wait for a few seconds while "60 seconds" and "Go!!" text slide on screen and then fade.
                yield return new WaitForSeconds(3.5f);

                //our Static variable GameVariables.RelaxModeScore needs to be set to zero (to make sure it is cleared)
                GameVariables.RelaxModeScore = 0;

                //then we call StartRelaxModeGame() and pass in true(hasTimer), and 90f(TimerTime)
                StartRelaxModeGame(true, 90f);

                //break... we are done.
                break;



            default:
                //do nothing, because I cant think of anything to do but scream, ERROR!!!!
                break;


        }

        //yield return null... this coroutine is over!
        yield return null;
    }

}
