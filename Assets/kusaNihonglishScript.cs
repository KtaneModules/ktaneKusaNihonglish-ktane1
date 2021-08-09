using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class kusaNihonglishScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] keyboard;
    public KMSelectable spacebar;
    public KMSelectable play;
    public KMSelectable red;
    public KMSelectable green;
    public TextMesh word;

    int chosenWord = 0;
    bool typing = false;
    public List<string> wordList = new List<string> { "yametekudastop", "sorrymasen", "gomenasorry", "sore wa chigawrong", "wakarunderstand", "tadaim home", "chotto a minute", "arigathanks", "arigathanks gozaimuch", "eatadakimasu", "subaramazing", "daijouokay", "seeyanara", "dont itashimashite", "urushaddup", "buenos oyasleep", "gebokawa", "konnichiwassup", "omedetulations", "konnichiwas geht ab", "gomennatschuldigung", "arigadanke", "willst du stress mann", "i missamisii you", "quantum chicken soup big chungus" };
    string alphabet = "qwertyuiopasdfghjklzxcvbnm";
    string currentText = "";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    //Thank you for everything, Coco.
    void Awake () {
        moduleId = moduleIdCounter++;
        Debug.LogFormat("[Kusa Nihonglish #{0}] Module initiated! OKITE OKITE OKITE!", moduleId);
        foreach (KMSelectable key in keyboard) {
            KMSelectable pressedKey = key;
            key.OnInteract += delegate () { keyPress(pressedKey); return false; };
        }

        spacebar.OnInteract += delegate () { Spacebar(); return false; };
        play.OnInteract += delegate () { PlayButton(); return false; };
        green.OnInteract += delegate () { GreenButton(); return false; };
        red.OnInteract += delegate () { RedButton(); return false; };

    }

    // Use this for initialization
    void Start () {
        int FUN = UnityEngine.Random.Range(0, 100);
        if (FUN == 42)
        {
            chosenWord = 24;
        }
        else
        {
            chosenWord = UnityEngine.Random.Range(0, 24);
        }
        currentText = "";
        word.text = currentText;
    }
	

    void keyPress(KMSelectable pressedKey)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            for (int i = 0; i < 26; i++)
            {
                if (pressedKey == keyboard[i])
                {
                    float width = 0;
                    foreach (char symbol in word.text)
                    {
                        CharacterInfo info;
                        if (word.font.GetCharacterInfo(symbol, out info, word.fontSize, word.fontStyle))
                        {
                            width += info.advance;
                        }
                    }
                    width = width * word.characterSize;
                    while (width > 1200)
                    {
                        word.characterSize -= 0.1f;
                        width = width * word.characterSize;
                    }
                    currentText += alphabet[i];
                    word.text = currentText;
                }
            }
        }
    }

    void Spacebar()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            float width = 0;
            foreach (char symbol in word.text)
            {
                CharacterInfo info;
                if (word.font.GetCharacterInfo(symbol, out info, word.fontSize, word.fontStyle))
                {
                    width += info.advance;
                }
            }
            width = width * word.characterSize;
            while (width > 1200)
            {
                word.characterSize -= 0.1f;
                width = width * word.characterSize;
            }
            currentText += " ";
            word.text = currentText;
        }
    }

    void PlayButton()
    {
        if (moduleSolved == false)
        {
            play.AddInteractionPunch();
            Audio.PlaySoundAtTransform(wordList[chosenWord], transform);
            if (typing == false)
            {
                typing = true;
                currentText = "";
                word.text = currentText;
                if (chosenWord == 24)
                {
                    Debug.LogFormat("[Kusa Nihonglish #{0}] Play button pressed! You just got blessed by quantum chicken soup grass big chungus. Submit literally anything to solve.", moduleId);
                }
                else
                {
                    Debug.LogFormat("[Kusa Nihonglish #{0}] Play button pressed! Your word is {1}.", moduleId, wordList[chosenWord]);
                }
            }
        }
    }

    void GreenButton()
    {
        green.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            word.characterSize = 1;
            if (chosenWord == 24)
            {
                Debug.LogFormat("[Kusa Nihonglish #{0}] You submitted something. Module solved anyways.", moduleId);
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
                currentText = "GOODBYE MFS";
                word.text = currentText;
            }
            else if (currentText == wordList[chosenWord])
            {
                Debug.LogFormat("[Kusa Nihonglish #{0}] You submitted the correct word. Module solved.", moduleId);
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
                currentText = "GOODBYE MFS";
                word.text = currentText;
            }
            else
            {
                Debug.LogFormat("[Kusa Nihonglish #{0}] The word you submitted is incorrect. Module strike and reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                int FUN = UnityEngine.Random.Range(0, 100);
                if (FUN == 42)
                {
                    chosenWord = 24;
                }
                else
                {
                    chosenWord = UnityEngine.Random.Range(0, 24);
                }
                currentText = "";
                word.text = currentText;
            }
            typing = false;
        }
    }

    void RedButton()
    {
        red.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (typing == true)
        {
            currentText = "";
            word.text = currentText;
            word.characterSize = 1;
        }
    }

    //twitch plays

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} play [Presses the play button] | !{0} submit <word> [Submits the specified word]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*press play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            play.OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!typing)
            {
                yield return "sendtochaterror The play button must be pressed before a submittion can be made!";
                yield break;
            }
            red.OnInteract();
            for (int j = 1; j < parameters.Length; j++)
            {
                for (int i = 0; i < parameters[j].Length; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    string comparer = parameters[j].ElementAt(i) + "";
                    comparer = comparer.ToLower();
                    if (comparer.Equals("q"))
                    {
                        keyboard[0].OnInteract();
                    }
                    else if (comparer.Equals("w"))
                    {
                        keyboard[1].OnInteract();
                    }
                    else if (comparer.Equals("e"))
                    {
                        keyboard[2].OnInteract();
                    }
                    else if (comparer.Equals("r"))
                    {
                        keyboard[3].OnInteract();
                    }
                    else if (comparer.Equals("t"))
                    {
                        keyboard[4].OnInteract();
                    }
                    else if (comparer.Equals("y"))
                    {
                        keyboard[5].OnInteract();
                    }
                    else if (comparer.Equals("u"))
                    {
                        keyboard[6].OnInteract();
                    }
                    else if (comparer.Equals("i"))
                    {
                        keyboard[7].OnInteract();
                    }
                    else if (comparer.Equals("o"))
                    {
                        keyboard[8].OnInteract();
                    }
                    else if (comparer.Equals("p"))
                    {
                        keyboard[9].OnInteract();
                    }
                    else if (comparer.Equals("a"))
                    {
                        keyboard[10].OnInteract();
                    }
                    else if (comparer.Equals("s"))
                    {
                        keyboard[11].OnInteract();
                    }
                    else if (comparer.Equals("d"))
                    {
                        keyboard[12].OnInteract();
                    }
                    else if (comparer.Equals("f"))
                    {
                        keyboard[13].OnInteract();
                    }
                    else if (comparer.Equals("g"))
                    {
                        keyboard[14].OnInteract();
                    }
                    else if (comparer.Equals("h"))
                    {
                        keyboard[15].OnInteract();
                    }
                    else if (comparer.Equals("j"))
                    {
                        keyboard[16].OnInteract();
                    }
                    else if (comparer.Equals("k"))
                    {
                        keyboard[17].OnInteract();
                    }
                    else if (comparer.Equals("l"))
                    {
                        keyboard[18].OnInteract();
                    }
                    else if (comparer.Equals("z"))
                    {
                        keyboard[19].OnInteract();
                    }
                    else if (comparer.Equals("x"))
                    {
                        keyboard[20].OnInteract();
                    }
                    else if (comparer.Equals("c"))
                    {
                        keyboard[21].OnInteract();
                    }
                    else if (comparer.Equals("v"))
                    {
                        keyboard[22].OnInteract();
                    }
                    else if (comparer.Equals("b"))
                    {
                        keyboard[23].OnInteract();
                    }
                    else if (comparer.Equals("n"))
                    {
                        keyboard[24].OnInteract();
                    }
                    else if (comparer.Equals("m"))
                    {
                        keyboard[25].OnInteract();
                    }
                    else
                    {
                        yield return "sendtochaterror Typing stopped due to a character not typable in the keyboard!";
                        yield break;
                    }
                }
                if (!(j+1 == parameters.Length))
                {
                    spacebar.OnInteract();
                }

            }
            yield return new WaitForSeconds(0.1f);
            green.OnInteract();

        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!typing)
        {
            yield return ProcessTwitchCommand("play");
        }
        yield return ProcessTwitchCommand("submit " + wordList[chosenWord]);
    }
}
