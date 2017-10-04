using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// FruitLauncherController Class handles the Launching of the gameObjects.  Fruit,Bombs, and PowerUps.
/// </summary>
public class LauncherController : MonoBehaviour
{
    public static LauncherController LaunchControllerInstance;                  // our static reference to this LaunchController
    [Header("Bottom Fruit Launchers, and how many to Launch")]
    public GameObject[] bottomFruitLaunchers;                                   // an array of the fruit launchers at the bottom of the dojo
    public int bottomLauncherSalvoAmount;                                       // the salvo amt (an int that determines how many fruit are fired
    [Header("Side Fruit Launchers(Frenzy), and how many to Launch")]
    //public GameObject[] sideFruitLaunchers;                                     // an array of the fruit launchers at the side of the dojo (for Frenzy PowerUp)
    //public int sideLauncherSalvoAmount;                                         // the salvo amt (an int that determines how many fruit are fired during a frenzy
    [Header("How Long To Wait For Fruit to Spawn?(MIN)")]
    public float minWaitTime;                                                   // Min time to wait before fruit are spawned (when requested)
    [Header("How Long To Wait For Fruit to Spawn?(MAX)")]
    public float maxWaitTime;                                                   // Max time to wait before fruit are spawned (when requested)
    [Header("How Long Between Regular Bottom Fruit Launches?")]
    public float timeBetweenLaunches;                                           // the time between salvos (there is a little break between them)
    //private float basedTimeBetweenLaunches;
    public float timeBetweenRandomLaunches;                                     // time between random launches (there are some salvos that come at random intervals too(breaks up the interval launches)
    public float timeBetweenSpecialLaunches;                                    // the time between special object's launches (power ups)
    public float timeBetweenBombLaunches;                                       // the time between bomb object's launches
    [Header("What should the Max Number of Fruit Launches be?")]
    public int maximumSimultaneousFruitLaunches;                                // the max number of simultaneous fruit launches (max salvo amt)
    [Tooltip("This is the number that after the 'maxSimultaneousFruitLaunches' is reached, that the launcher will start over at")]
    public int resetFruitLauncherAmount;                                        // if we reach the max number of launches, what number should we drop it back down to?
    [Header("How Many More Fruit In Relax Mode?")]
    public int RelaxModeExtraFruit;                                             // How many extra fruit would we like launched in relax mode?  there are no fruit and bombs, so why not add fruit!?
    [Header("Amount of Objects that are launched")]
    [Header(" **goes up each loop** ")]
    public int BombSalvoAmount;                                                 // the amount of bombs that will be launched during bomb salvos
    public int maxBombSalvoAmt;                                                 // the max number of bombs that can be launched at one time
    public int powerUpSalvoAmount;                                              // the amount of power-ups that will be launched when called for(one usually)
    public int freezeBananaPowerUpCount;                                        // the count of how many Freeze Bananas have launched in the round (ClassicMode)
    public int frenzyBananaPowerUpCount;                                        // the count of how many Frenzy Bananas have launched in the round (ClassicMode)
    public int twoTimesScoreBananaPowerUpCount;                                 // the count of how many 2xScore Bananas have launched in the round (ClassicMode)
    private float initialTimeBetweenLaunches;                                   // the initial time between the fruit launches
    private float initialTimeBetweenBombLaunches;
    private List<FruitLauncher> bottomLaunchersScriptReference;                 // a List that contains all of the Bottom FruitLaunchers (the gameObject the "FruitLauncher" script is attached to)
    //private List<FruitLauncher> sideLaunchersScriptReference;                   // a List that contains all of the Side FruitLaunchers (the gameObject the "FruitLauncher" script is attached to)
    private CountdownTimer timer;                                               // a reference to our "CountdownTimer". GameController,DojoBoundary,LauncherContoller(this), and CountdownTimer are all on GameController gameObject
    //private FreezeEffect freezeEffectReference;                                 // a reference to the FreezeEffect GameObject (in the scene.. the one that activates the effect)
    //private FrenzyEffect frenzyEffectReference;                                 // a reference to the FreezeEffect GameObject (in the scene.. the one that activates the effect)
    //private TwoTimesScoreEffect twoTimesScoreEffectReference;                   // a reference to the 2xScore GameObject (in the scene.. the one that activates the effect)
    //private int cycleThruSideLaunchersForFrenzy;                                // the int that is used for looping through the side launchers
    //private int numOfTimesFreezeBananaLaunched;                                 // the number of times that the Freeze Banana has been launched
    //private int numOfTimesFrenzyBananaLaunched;                                 // the number of times that the Freeze Banana has been launched
    //private int numOfTimesScoreTimesTwoLaunched;                                // the number of times that the Freeze Banana has been launched
    public static ArrayList listWords;                                          // a list of words randomed from dictionary
    public static ArrayList dictionary;                                         // words database
    private List<string> prepairWords;                                          // words randomed from listwords to throw up
    public static string suggestion = "";                                       // suggestion
    public static int numWords;                                                 // the number of words in listWords
    public static ArrayList listMultiWord;
    public static Dictionary<string, ArrayList> multiDic;
    public static List<Dictionary<char, int>> listCounting;
    public int kindLine;
    public string lastWord = "";

    //public static ArrayList listChars = new ArrayList();
    // Use this for pre-initialization
    void Awake()
    {
        //make sure our static LaunchController Instance is set to THIS gameObject
        LaunchControllerInstance = this;

        // Read database
        dictionary = new ArrayList();
        TextAsset text = Resources.Load("dictionary") as TextAsset;
        string[] dic = Regex.Split(text.text, "\n");
        foreach(string s in dic) {
            string s1 = s;
            for (int i = s1.Length - 1; i >= 0; i--) {
                if (s1[i] > 'z' || s1[i] < 'a') {
                    s1 = s1.Remove(i, 1);
                }
            }
            if(s1.Length > 0)
                dictionary.Add(s1);
        }
        ////System.IO.StreamReader file = new System.IO.StreamReader(Application.persistentDataPath + "Resources\\words.txt");
        //string line;
        //while((line = file.ReadLine()) != null) {
        //    dictionary.Add(line);
        //}

        listMultiWord = GetMultiWords();

        // random 200 words
        listWords = GetAllWord();
        numWords = listWords.Count;

        //our timer reference by calling GetComponent on THIS gameObject
        timer = GetComponent<CountdownTimer>();

        //Use GameObjects FindObjectWithTag method to setup our references to the freeze,frenzy, and 2xScore objects/classes.  Using our Const Strings in "Tags"
        //freezeEffectReference = GameObject.FindGameObjectWithTag(Tags.freezeEffectGameObjectTag).GetComponent<FreezeEffect>();
        //frenzyEffectReference = GameObject.FindGameObjectWithTag(Tags.frenzyEffectGameObjectTag).GetComponent<FrenzyEffect>();
        //twoTimesScoreEffectReference = GameObject.FindGameObjectWithTag(Tags.twoTimesScoreEffectGameObjectTag).GetComponent<TwoTimesScoreEffect>();

        //Use GameObjects FindObjectWithTag method to setup our references to the bottom and side fruitLaunchers.
        bottomFruitLaunchers = GameObject.FindGameObjectsWithTag(Tags.bottomFruitLaunchers);
        //sideFruitLaunchers = GameObject.FindGameObjectsWithTag(Tags.sideFruitLaunchers);

        //Initialize the lists  "bottomLaunchersScriptReference" && "sideLaunchersScriptReference"
        bottomLaunchersScriptReference = new List<FruitLauncher>();
        //sideLaunchersScriptReference = new List<FruitLauncher>();

        //loop through all of the bottom fruit launchers
        for (int i = 0; i < bottomFruitLaunchers.Length; i++)
        {
            //now add each bottom fruit launcher to our List ("bottomLaunchersScriptReference")
            bottomLaunchersScriptReference.Add(bottomFruitLaunchers[i].GetComponent<FruitLauncher>());
        }

        //basedTimeBetweenLaunches = timeBetweenLaunches;

        prepairWords = new List<string>();
        
        //loop through all of the side fruit launchers
        //for (int j = 0; j < sideFruitLaunchers.Length; j++)
        //{
        //    //now add each side fruit launcher to our List ("sideLaunchersScriptReference")
        //    sideLaunchersScriptReference.Add(sideFruitLaunchers[j].GetComponent<FruitLauncher>());
        //}
    }

    // Get 200 words randomly

    public static ArrayList GetAllWord()
    {
        listWords = new ArrayList();
        for (int i = 1; i <= 20; i++) {
            int r = Random.Range(0, listMultiWord.Count);
            //Debug.Log(dictionary[r]);
            //Debug.Log(dictionary[r].ToString().Length);
            listWords.Add(listMultiWord[r]);
        }


        return listWords;
    }

    public static ArrayList GetMultiWords() {
        listMultiWord = new ArrayList();
        multiDic = new Dictionary<string, ArrayList>();

        TextAsset text = Resources.Load("Word") as TextAsset;
        string[] dic = Regex.Split(text.text, "\n");
        foreach(string s in dic) {
            string s1 = s;
            for(int i = s1.Length - 1; i >= 0; i--) {
                if(s1[i] > 'z' || s1[i] < 'a') {
                    s1 = s1.Remove(i, 1);
                }
            }
            if(s1.Length > 0)
                listMultiWord.Add(s1);
        }
        //listMultiWord.Sort();

        //listCounting = new List<Dictionary<char, int>>();
        //foreach (string s in listMultiWord) {
        //    Dictionary<char, int> a = new Dictionary<char, int>();
        //    foreach (char c in s.ToCharArray()) {
        //        if(!a.ContainsKey(c)) {
        //            a[c] = 1;
        //        } else {
        //            a[c] += 1;
        //        }
        //    }
        //}
        //foreach(string word in listMultiWord) {
        //    //string[] w = s;
        //    for(int i = 1; i < word.Length - 1; i++) {
        //        if(dictionary.Contains(word.Substring(0, i)) && dictionary.Contains(word.Substring(i))) {
        //            if(!multiDic.ContainsKey(word.Substring(0, i)))
        //                multiDic[word.Substring(0, i)] = new ArrayList();
        //            multiDic[word.Substring(0, i)].Add(word.Substring(i));
        //            break;
        //        }
        //    }
        //}
        //string[] str = new string[10000];
        //int n = 0;
        //foreach(string s in listMultiWord) {
        //    str[n] = s;
        //    n += 1;
        //}
        //System.IO.File.WriteAllLines("multiword.txt", str);

        return listMultiWord;
    }

    // Use this for initialization
    void Start()
    {
        //we store our "timeBetweenLaunches" in the InitialTimeBetweenLaunches" variable. (so we have a back up of the original value)
        initialTimeBetweenLaunches = timeBetweenLaunches;
        // random time between bomb launches
        initialTimeBetweenBombLaunches = Random.Range(timeBetweenBombLaunches - 2, timeBetweenBombLaunches + 2);
        //StartCoroutine(FirstFruitLaunch(bottomLauncherSalvoAmount));
    }


    private void Update() {
        //initialTimeBetweenLaunches = Mathf.Max(basedTimeBetweenLaunches - GameVariables.ClassicModeScore / 100 * 2, 5);

    }

    /// <summary>
    /// This Method calls for the First Fruit Launch.
    /// </summary>
    /// <param name="amt"></param>
    /// <returns></returns>
    private IEnumerator FirstFruitLaunch(int amt)
    {
        //we wait for 1f seconds
        yield return new WaitForSeconds(4f);
        //Then we "RequestFruitSalvoFromBottom(and pass in the "amt" that was passed with the method call)"
        //RequestFruitSalvoFromBottom(amt);
    }


    /// <summary>
    /// This method adjusts some of the launcher settings/times when relax is the gameMode.
    /// </summary>
    private void ChangeToRelaxSettings()
    {
        //we assign timeBetweenLaunches 4.
        timeBetweenLaunches = 4;
        //we assign initialTimeBetweenLaunches 4 as well.
        initialTimeBetweenLaunches = 4;
        //then we set the "maximumSimultaneousFruitLaunches" to 9;
        maximumSimultaneousFruitLaunches = 4;

    }


    /// <summary>
    /// This Method is where all of the "Launching" happens.  This method is pretty long compared to the others, so sit down, and strap in.
    /// We monitor the "GameMode" we are in based on a Switch that compares our gameModes var to the GameModes Enum.
    /// </summary>
    public void ReduceLaunchTimersAndLaunchObjects()
    {

        //before we get into gameMode specific code... timeBetweenLaunches gets decremented by Time.deltaTime;
        timeBetweenLaunches -= Time.deltaTime;
        timeBetweenBombLaunches -= Time.deltaTime;

        // if timeBetweenBombLaunches < 0 we ....
        if (timeBetweenBombLaunches <= 0f && timeBetweenLaunches > 0f) {
           
            int powerUpTypeRandom = Random.Range(0, 4);



            //randomize a poison
            if(powerUpTypeRandom == 0) {
                RequestOtherObjectSalvoFromBottomLauncher(1, 2);

            }



            //randomize a antidot
            if(powerUpTypeRandom == 1) {
                RequestOtherObjectSalvoFromBottomLauncher(1, 1);
            }



            //randomize a x2 score
            if(powerUpTypeRandom == 2) {
                RequestOtherObjectSalvoFromBottomLauncher(1, 0);
            }

            //randomize a gameover bomb
            if(powerUpTypeRandom == 3) {
                RequestOtherObjectSalvoFromBottomLauncher(1, 4);
            }
            
            timeBetweenBombLaunches = initialTimeBetweenBombLaunches;
        }


        //random words
        if (timeBetweenLaunches <= 0f)
        {
            //create a new int named "r" and give it a random value between 0 and 3 (inclusive, exclusive... so values can be 0,1,2)
            int r = Random.Range(0, 2);
            //now for the gameMode that we are in.
            switch (GameController.GameControllerInstance.gameModes)
            {


                
                /////////////////////////////////////
                ////////_____Classic-MODE_____////////
                /////////////////////////////////////


                //if we are in GameMode Classic
                case GameModes.Classic:


                    //we check to see what random value was generated...

                    //if r equals 0, we....  ( we just launch fruit )
                    if(timer.timeLeft > 1.5f) {
                        //launch fruit
                        RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                        //increment salvo amount
                        //time between launches gets initialTimeBetweenLaunches value
                        timeBetweenLaunches = initialTimeBetweenLaunches;
                    }


                    //if r equals 1, we...  ( we launch fruit and bombs )
                    //if (r == 1)
                    //{

                    //    //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen)
                    //    if (timer.timeLeft > 1.5f)
                    //    {
                    //        //launch fruit (lesser amount because we are also launching bombs...)
                    //        RequestFruitSalvoFromBottom(BombSalvoAmount + 2);
                    //        //launch bombs
                    //        RequestOtherObjectSalvoFromBottomLauncher(BombSalvoAmount, 3);
                    //        //increment bomb salvo amount
                    //        BombSalvoAmount++;
                    //        //time between launches gets initialTimeBetweenLaunches value
                    //        timeBetweenLaunches = initialTimeBetweenLaunches;
                    //        //if the BombSalvoAmount is greater than maxBombSalveAmt then....
                    //        if (BombSalvoAmount > maxBombSalvoAmt)
                    //        {
                    //            //we reset the bomb salvo amount to some other value between 1 and the maxBombSalvoAmt
                    //            BombSalvoAmount = Random.Range(1, maxBombSalvoAmt);
                    //        }
                    //    }

                    //}



                    //if the "bottomLauncherSalvoAmount" is at the "maximumSimultaneousFruitLaunches"
                    if (bottomLauncherSalvoAmount > maximumSimultaneousFruitLaunches)
                    {
                        //then we assign "resetFruitLauncherAmount" to "bottomLauncherSalvoAmount"
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }



                    break;


                /////////////////////////////////////
                ////////______RELAX-MODE_____////////
                /////////////////////////////////////


                //if we are in GameMode Relax
                case GameModes.Relax:

                    //RelaxMode is for fruit only... no matter what r is assigned
                    if (r == 0 || r == 1 || r == 2)
                    {
                        //if the time is above 1.5 second(changed from zero to prevent a launch right after GameOver Screen
                        if (timer.timeLeft > 1.5f)
                        {
                            //launch fruit
                            RequestFruitSalvoFromBottom(bottomLauncherSalvoAmount);
                            //increment the salvo amount.
                            bottomLauncherSalvoAmount++;
                            //we reset the time between launches...
                            timeBetweenLaunches = initialTimeBetweenLaunches;
                        }

                    }

                    //if the "bottomLauncherSalvoAmount" is at the "maximumSimultaneousFruitLaunches"
                    if (bottomLauncherSalvoAmount >= maximumSimultaneousFruitLaunches)
                    {
                        //then we assign "resetFruitLauncherAmount" to "bottomLauncherSalvoAmount"
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }


                    break;
                default:

                    if(bottomLauncherSalvoAmount >= maximumSimultaneousFruitLaunches) {
                        //then we assign "resetFruitLauncherAmount" to "bottomLauncherSalvoAmount"
                        bottomLauncherSalvoAmount = resetFruitLauncherAmount;
                    }

                    break;
                
            }
            if(bottomLauncherSalvoAmount > maximumSimultaneousFruitLaunches) {
                //we need to assign the "resetFruitLauncherAmount" to our "bottomLauncherSalvoAmount" variable.
                bottomLauncherSalvoAmount = resetFruitLauncherAmount;
            }
        }

    }


    /// <summary>
    /// This Method Stops all of the Object Launchers. 
    /// </summary>
    private void CancelAllObjectLaunchers()
    {
        //set the timer to 0
        timer.timeLeft = 0;
        //call StopAllCoroutines.
        StopAllCoroutines();
    }


    /// <summary>
    /// This Coroutine is called by "WaitToLaunchBottomFruit".  It fulfills the launching of the fruit.  The Coroutine does the "actual firing".
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToLaunchBottomFruit(int index, char c)
    {
        //we wait a random interval between minWaitTime and maxWaitTime
        yield return new WaitForSeconds(0.5f);
        //create a random variable from 0 to the length of the bottomeFruitLaunchers array.
        //int r = Random.Range(0, bottomFruitLaunchers.Length);
        //the we call LoadAndFireRandomFruit on the bottomLauncherScriptReference element at position r.
        bottomLaunchersScriptReference[index].LoadAndFireRandomFruit(c);

    }

    private IEnumerator WaitToLaunchBottomFruit(List<int> line, char[] chars, int index, float time) {
        //we wait a random interval between minWaitTime and maxWaitTime
        if(index >= chars.Length)
            yield return null;
        if (index != 0 && line[index] == line[index - 1]) {
            yield return new WaitForSeconds(time);
        } else {
            yield return new WaitForSeconds(time);

        }
        //create a random variable from 0 to the length of the bottomeFruitLaunchers array.
        //int r = Random.Range(0, bottomFruitLaunchers.Length);
        //the we call LoadAndFireRandomFruit on the bottomLauncherScriptReference element at position r.
        bottomLaunchersScriptReference[line[index]].LoadAndFireRandomFruit(chars[index]);

    }

    private IEnumerator WaitToNextLaunchInARound() {
        Debug.Log(1);
        yield return new WaitForSeconds(1);
    }





    /// <summary>
    /// This Coroutine is called by "RequestOtherObjectSalvoFromBottomLauncher".  It fulfills the launching of the fruit.  The Coroutine does the "actual firing".
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    private IEnumerator WaitToLaunchBottomOtherObject(int objectType)
    {
        //we wait a random interval between minWaitTime and maxWaitTime
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        //create a random variable from 0 to the length of the bottomeFruitLaunchers array.
        int r = Random.Range(0, bottomFruitLaunchers.Length);
        //the we call LoadAndFireOtherObject on the bottomLauncherScriptReference element at position r, and pass the objectType.
        bottomLaunchersScriptReference[r].LoadAndFireOtherObject(objectType);
    }




    /// <summary>
    /// This Coroutine is called by "RequestFruitSalvoFromSide".  It fulfills the launching of the fruit from the side(frenzy).  The Coroutine does the "actual firing".
    /// </summary>
    /// <returns></returns>


    public IEnumerator Shoot(char[] chars, int start, int end, float t) {

        yield return new WaitForSeconds(t);
        int h = chars.Length;
        kindLine = Random.Range(0, 2);
        List<int> line = getLine(h, start, end, kindLine);
        chars = ResuffleByLine(chars, kindLine);
        float time = 0;
        for (int i = 0; i < chars.Length; i++) {
            if(i != 0 && line[i] == line[i - 1]) {
                time += 0.5f;
            } else {
                time += Random.Range(0f, 0.2f);
            }
            StartCoroutine(WaitToLaunchBottomFruit(line, chars, i, time));
        }
    }

    /// <summary>
    /// This Method is called When fruit should be launched from the bottom.  We pass the number of fruit that we want launched.  This Method start the actual coroutines that call the 
    /// fire method on the "Fruit Launcher"
    /// </summary>
    /// <param name="numOfFruit"></param>
    public void RequestFruitSalvoFromBottom(int numOfFruit)
    {
        //loop through  "numOfFruit"
        GenerateLetter();
        if (prepairWords.Count == 1) {
            StartCoroutine(Shoot(prepairWords[0].ToCharArray(), Random.Range(0, 3), maximumSimultaneousFruitLaunches - 1, 0f));
            prepairWords.RemoveAt(0);
        } else {
            int start = 0;
            for (int i = 0; i < prepairWords.Count; i++) {
                //Debug.Log(maximumSimultaneousFruitLaunches);
                //Debug.Log(prepairWords.Count);
                if(prepairWords[i].Length > maximumSimultaneousFruitLaunches / prepairWords.Count) {
                    StartCoroutine(Shoot(prepairWords[i].ToCharArray(), 0, maximumSimultaneousFruitLaunches - 1, 1f * i));
                } else {
                    StartCoroutine(Shoot(prepairWords[i].ToCharArray(), Random.Range(0, maximumSimultaneousFruitLaunches/2), maximumSimultaneousFruitLaunches - 2, 1f * i));
                }
            }
            prepairWords.Clear();
        }
        //foreach (char c in chars) {
        //    Debug.Log(line[index]);
        //    StartCoroutine(WaitToLaunchBottomFruit(line[index], c));
        //    index++;
        //}
        //List<int> fightedSalvo = new List<int>();
        //if(h <= 6) {
        //    foreach(char c in chars) {
        //        int r = Random.Range(0, numOfFruit);
        //        while(fightedSalvo.Contains(r)) {
        //            r = Random.Range(0, numOfFruit);
        //        }
        //        fightedSalvo.Add(r);
        //        StartCoroutine(WaitToLaunchBottomFruit(r, c));
        //    }
        //} else {
        //    foreach(char c in chars) {
        //        int r = Random.Range(0, numOfFruit);
        //        StartCoroutine(WaitToLaunchBottomFruit(r, c));
        //    }
        //}
        //for (int i = 1; i <= 1; i++) {
        //    int c1 = Random.Range(0, 25) + 97;
        //    int r1 = Random.Range(0, numOfFruit);
        //    while(fightedSalvo.Contains(r1)) {
        //        r1 = Random.Range(0, numOfFruit);
        //    }
        //    StartCoroutine(WaitToLaunchBottomFruit(r1, (char)c1));
        //}
    }

    public char[] ResuffleByLine(char[] chars, int r) {
        if (r == 0) {
            return chars;
        }
        if (r == 1) {
            return chars;
        }
        return chars;
    }

    public List<int> getLine(int length, int start, int finish, int r) {
        List<int> line = new List<int>();
        if (r == 0) {
            int i = start;
            int k = 1;
            for (int j = 0;  j < length + 2; j++) {
                line.Add(i);
                if (i + k > finish || i + k < start) {
                    line.Add(i);
                    j += 1;
                    k *= -1;
                }
                i += k;
            }
        }
        if(r == 1) {
            int i = finish;
            int k = -1;
            for(int j = 0; j < length; j++) {
                line.Add(i);
                if(i + k > finish || i + k < start) {
                    line.Add(i);
                    j += 1;
                    k *= -1;
                }
                i += k;
            }
        }
        return line;
    }

    public void GenerateLetter()
    {
        if (prepairWords.Count == 0 && numWords > 0) {
            numWords -= 1;
            int index = 0;
            string word = "";
            if (GameVariables.longerWord != "") {
                Debug.Log(GameVariables.longerWord);
                string longer = GameVariables.longerWord.ToLower();
                int start = listMultiWord.BinarySearch(longer);
                if (start < 0) {
                    start = ~start;
                }
                int end = listMultiWord.BinarySearch(longer + "z");
                if (end < 0) {
                    end = ~end;
                }
                Debug.Log(start);
                Debug.Log(end);
                if (start < end) {
                    for(int i = 0; i <= Mathf.Min(end - start, 10); i++) {
                        int r = Random.Range(start, end);
                        string last = listMultiWord[r].ToString().Substring(longer.Length);
                        if(word == "" && last != "") {
                            foreach(string s in listMultiWord) {
                                if(s.Contains(last) && !s.StartsWith(longer) && !s.Equals(lastWord) ) {
                                    Debug.Log(last);
                                    word = s;
                                    break;
                                }
                            }
                        }
                    }
                }
                //if(multiDic.ContainsKey(longer)) {
                //    int i = Random.Range(0, multiDic[longer].Count);
                //    string start = multiDic[longer][i].ToString();
                //    Debug.Log(start);
                //    if(multiDic.ContainsKey(start)) {
                //        i = Random.Range(0, multiDic[start].Count);
                //        word = start + multiDic[start][i].ToString();
                //    }
                //}

                //bool ok = false;
                //for (index = 0; index < listMultiWord.Count; index++) {
                //    string s = listMultiWord[index].ToString();
                //    if(s.StartsWith(longer)) {
                //        if(dictionary.Contains(s.Substring(longer.Length))) {
                //            word = s;
                //            ok = true;
                //            break;
                //        }
                //    }
                //}
                //if(!ok) {
                //    index = Random.Range(0, listWords.Count);
                //    word = (string)listWords[index];
                //    listWords.RemoveAt(index);
                //}
                
            } 
            if (word == "") {
                do {
                    index = Random.Range(0, listMultiWord.Count);
                    word = (string)listMultiWord[index];
                } while(word.Length > 100);
                //listWords.RemoveAt(index);
            }
            lastWord = word;
            Debug.Log(word);
            Debug.Log(word.Length);
            bool ok = false;
            for(int i = 1; i < word.Length - 1; i++) {
                if(dictionary.Contains(word.Substring(0, i)) && dictionary.Contains(word.Substring(i))) {
                    prepairWords.Add(word.Substring(0, i));
                    prepairWords.Add(word.Substring(i));
                    ok = true;
                    break;
                }
            }
            if(!ok) {
                prepairWords.Add(word);
            }
        } else if(prepairWords.Count == 0 && numWords == 0) {
            GameObject[] fruit = GameObject.FindGameObjectsWithTag(Tags.fruitTag);
            if(fruit == null || fruit.Length == 0) {
                CountdownTimer.countdown.timeLeft = 0;
            }
            
        }
        //string w = prepairWords[0];
        //prepairWords.RemoveAt(0);
        //Debug.Log(w);
        return;

        //if (prepairWords.Count == 0 && listWords.Count > 0) {
        //    // randomize 2 words
        //    suggestion = "";
        //    List<string> l = new List<string>();
        //    int range = Mathf.Min(2, listWords.Count);
        //    for(int i = 1; i <= range; i++) {
        //        int index = Random.Range(0, listWords.Count);
        //        string word = (string)listWords[index];
        //        listWords.Remove(word);
        //        suggestion += word + " ";
        //        l.Add(word);
        //    }
        //    // separate and add to prepair
        //    int half1 = l[0].Length / 2;
        //    int half2 = l[1].Length / 2;

        //    prepairWords.Add(l[0].Substring(0, half1));
        //    prepairWords.Add(l[1].Substring(0, half2));
        //    prepairWords.Add(l[0].Substring(half1));
        //    prepairWords.Add(l[1].Substring(half2));
        //    numWords -= 2;

        //} else if (prepairWords.Count == 0 && listWords.Count == 0) {
        //    // no word game over
        //    if(numWords == 0) {
        //        GameObject[] fruit = GameObject.FindGameObjectsWithTag(Tags.fruitTag);
        //        if(fruit == null  || fruit.Length == 0) {
        //            CountdownTimer.countdown.timeLeft = 0;
        //        }
        //    }
        //}

        //// choose which part to throw

        //string w = prepairWords[0];
        //prepairWords.RemoveAt(0);
        //return w.ToCharArray();
    }

    public Dictionary<char, int> TransformLetter(string s) {
        Dictionary<char, int> d = new Dictionary<char, int>();
        foreach (char i in s.ToCharArray()) {
            d[i] += 1;
        }

        return d;
    }

    /// <summary>
    /// This Method is called When other objects should be launched from the bottom.  We pass "numOfOtherProjectiles" for the AMT of other objects we want.  Then We pass the number for the TYPE we 
    /// want launched. This Method starts the actual coroutines that call the fire method on the "Fruit Launcher" 0 = 2X Score , 1 = Frenzy , 2 = Freeze , 3 = Minus10Bomb , and 4 = GameOverBomb.
    /// </summary>
    /// <param name="numOfOtherProjectiles"></param>
    /// <param name="objectType"></param>
    public void RequestOtherObjectSalvoFromBottomLauncher(int numOfOtherProjectiles, int objectType)
    {
        //loop through  "numOfFruit"
        for (int i = 0; i < numOfOtherProjectiles; i++)
        {
            //and call startCoroutine (passing the objectType that we want launched) as a parameter, and we do that for the number iterations in the loop
            StartCoroutine(WaitToLaunchBottomOtherObject(objectType));
        }
    }



    /// <summary>
    /// This Method is called When fruit should be launched from the side.  We pass the number of fruit that we want launched.  This Method starts the actual coroutines that call the 
    /// fire method on the "Fruit Launcher"
    /// </summary>
    /// <param name="numOfFruit"></param>
    //public void RequestFruitSalvoFromSide(int numOfFruit)
    //{
    //    //loop through  "numOfFruit"
    //    //for (int i = 0; i < numOfFruit; i++)
    //    //{
    //    //    //and call startCoroutine for the number iterations in the loop
    //    //    StartCoroutine(WaitToLaunchSideFruit());
    //    //}
    //}


}
