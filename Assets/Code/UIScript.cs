using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    private static GameObject Clouds;
    private static GameObject[] Water;
    private static Menu[] Menus { get; set; }
    private string[] MenuNames = new string[]
    { 
        "Main Menu",
        "Pause Menu",
        "Settings Menu",
        "Controls Menu",
        "Level Select Menu",
        "Game Menu"
    };
    private string[] SettingsTexts = new string[]
    {
        "Tier",
        "Value"
    };
    private string[] GameTexts = new string[]
    {
        "Goal",
        "Score"
    };
    private static GameObject continueObject;
    private static GameObject powerBar;
    private static PowerIndicatorScript powerIndicator;
    private static float offTime = float.PositiveInfinity;

    private void Awake()
    {
        Clouds = GameObject.FindGameObjectWithTag("Clouds");
        Water = GameObject.FindGameObjectsWithTag("Water");
        //if (Water[0].name == "High Water")
        //{
        //    GameObject water = Water[0];
        //    Water[0] = Water[1];
        //    Water[1] = water;
        //}
        //Water[0].SetActive(false);
        GameObject[] ms = GameObject.FindGameObjectsWithTag("Menu");
        GameObject[] ts = GameObject.FindGameObjectsWithTag("Text");
        GameObject[] bs = GameObject.FindGameObjectsWithTag("Button");
        GameObject[] ss = GameObject.FindGameObjectsWithTag("Slider");
        Menu[] menus = new Menu[ms.Length];
        for (int i = 0; i < ms.Length; i++)
        {
            menus[i] = new Menu(ms[i]);
            List<Text> texts = new List<Text>();
            for (int j = 0; j < ts.Length; j++)
            {
                if (ts[j].GetComponentInParent<MenuTag>().gameObject.name == menus[i].Self.name)
                    texts.Add(ts[j].GetComponent<Text>());
            }
            menus[i].Texts = texts.ToArray();
            List<Button> buttons = new List<Button>();
            for (int j = 0; j < bs.Length; j++)
            {
                if (bs[j].GetComponentInParent<MenuTag>().gameObject.name == menus[i].Self.name)
                    buttons.Add(bs[j].GetComponent<Button>());
            }
            menus[i].Buttons = buttons.ToArray();
            List<Slider> sliders = new List<Slider>();
            for (int j = 0; j < ss.Length; j++)
            {
                if (ss[j].GetComponentInParent<MenuTag>().gameObject.name == menus[i].Self.name)
                    sliders.Add(ss[j].GetComponent<Slider>());
            }
            menus[i].Sliders = sliders.ToArray();
        }
        Slider[] slids = menus[2].Sliders;
        menus[2].Sliders = new Slider[2];
        for (int i = 0; i < menus[2].Sliders.Length; i++)
        {
            for (int j = 0; j < slids.Length; j++)
            {
                if ("Slider " + (i + 1) == slids[j].gameObject.name)
                    menus[2].Sliders[i] = slids[j];
            }
        }
        Text[] texs = menus[2].Texts;
        menus[2].Texts = new Text[SettingsTexts.Length];
        for (int i = 0; i < menus[2].Texts.Length; i++)
        {
            for (int j = 0; j < texs.Length; j++)
            {
                if (SettingsTexts[i] == texs[j].gameObject.name)
                    menus[2].Texts[i] = texs[j];
            }
        }
        texs = menus[5].Texts;
        menus[5].Texts = new Text[GameTexts.Length];
        for (int i = 0; i < menus[5].Texts.Length; i++)
        {
            for (int j = 0; j < texs.Length; j++)
            {
                if (GameTexts[i] == texs[j].gameObject.name)
                    menus[5].Texts[i] = texs[j];
            }
        }
        Menus = new Menu[MenuNames.Length];
        for (int i = 0; i < Menus.Length; i++)
        {
            for (int j = 0; j < menus.Length; j++)
            {
                if (MenuNames[i] == menus[j].Self.name)
                    Menus[i] = menus[j];
            }
        }
        for (int i = 0; i < Menus.Length; i++)
        {
            Menus[i].Self.SetActive(false);
        }
        Menus[2].Sliders[0].onValueChanged.AddListener(SetQuality);
        Menus[2].Sliders[1].onValueChanged.AddListener(SetSensitivity);
        powerBar = GameObject.FindGameObjectWithTag("Bar");
        powerIndicator = powerBar.GetComponentInChildren<PowerIndicatorScript>();
        continueObject = GameObject.FindGameObjectWithTag("ContinueText");
    }
    
    public static void EnableMenu(int index)
    {
        for (int i = 0; i < Menus.Length; i++)
        {
            Menus[i].Self.SetActive(i == index);
        }
    }

    public static void ActivatePowerBar()
    {
        powerBar.SetActive(true);
    }

    public static void CheckOff()
    {
        if (Time.time >= offTime)
        {
            DeactivatePowerBar();
        }
    }

    public static void DeactivatePowerBar()
    {
        offTime = float.PositiveInfinity;
        powerBar.SetActive(false);
    }

    public static void DeactivatePowerBar(float time)
    {
        offTime = Time.time + time;
    }

    public static void SetPowerLevel(float amount)
    {
        powerIndicator.SetBar(amount);
    }

    public static void SetContinueObject(bool b)
    {
        continueObject.SetActive(b);
    }

    public static void SetPar()
    {
        Menus[5].Texts[0].text = "Par: " + GameManagerScript.CurrentLevel.Par;
    }

    public static void SetScore()
    {
        Menus[5].Texts[1].text = "Strokes: " + GameManagerScript.Players[0].Score;
    }

    public static void IncrementScore()
    {
        GameManagerScript.Players[0].Score++;
        SetScore();
    }

    public static void SetQuality(float setting)
    {
        QualitySettings.SetQualityLevel((int)setting);
        Menus[2].Texts[0].text = QualitySettings.names[(int)setting];
        //switch (setting)
        //{
        //    case 0:
        //        for (int i = 0; i < Profile.components.Count; i++)
        //            Profile.components[i].active = false;
        //        Clouds.SetActive(false);
        //        Water[0].SetActive(true);
        //        Water[1].SetActive(false);
        //        break;
        //    case 1:
        //        Profile.components[0].active = false;
        //        Profile.components[1].active = true;
        //        Profile.components[2].active = true;
        //        Profile.components[3].active = false;
        //        Clouds.SetActive(true);
        //        Water[0].SetActive(true);
        //        Water[1].SetActive(false);
        //        break;
        //    case 2:
        //        for (int i = 0; i < Profile.components.Count; i++)
        //            Profile.components[i].active = true;
        //        Clouds.SetActive(true);
        //        Water[0].SetActive(false);
        //        Water[1].SetActive(true);
        //        break;
        //}
    }

    public static void SetSensitivity(float setting)
    {
        GameManagerScript.Sensitivity = setting;
        Menus[2].Texts[1].text = GameManagerScript.Sensitivity.ToString("0.000");
    }

    public static void StartLocalSolo()
    {
        if (GameManagerScript.LastMenuStates.Peek() != GameManagerScript.MenuState.Pause)
        {
            GameManagerScript.SetLevel(GameManagerScript.LevelIndex);
            GameManagerScript.SetGameState(GameManagerScript.GameState.LocalSolo);
        }
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Game);
    }
    
    public static void ControlsButton()
    {
        GameManagerScript.LastMenuStates.Push(GameManagerScript.CurrentMenuState);
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Controls);
    }

    public static void SettingsButton()
    {
        GameManagerScript.LastMenuStates.Push(GameManagerScript.CurrentMenuState);
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Settings);
    }

    public static void ResumeButton()
    {
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Game);
        GameManagerScript.SetCurrentPlayerState(GameManagerScript.CurrentPlayer.PreviousState);
    }

    public static void RestartButton()
    {
        StartLocalSolo();
    }

    public static void ExitButton()
    {
        GameManagerScript.ExitGame();
    }

    public static void PauseButton()
    {
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Pause);
    }

    public static void BackButton()
    {
        GameManagerScript.SetMenuState(GameManagerScript.LastMenuStates.Pop());
    }

    public static void MainMenuButton()
    {
        GameManagerScript.SetGameState(GameManagerScript.GameState.Menu);
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.Main);
    }

    public static void LevelSelectButton()
    {
        GameManagerScript.LastMenuStates.Push(GameManagerScript.CurrentMenuState);
        GameManagerScript.SetMenuState(GameManagerScript.MenuState.LevelSelect);
    }

    public static void Level1Button()
    {
        GameManagerScript.LevelIndex = 0;
        ControlsButton();
    }

    public static void Level2Button()
    {
        GameManagerScript.LevelIndex = 1;
        ControlsButton();
    }

    public static void Level3Button()
    {
        GameManagerScript.LevelIndex = 2;
        ControlsButton();
    }

    public static void Level4Button()
    {
        GameManagerScript.LevelIndex = 3;
        ControlsButton();
    }

    public static void Level5Button()
    {
        GameManagerScript.LevelIndex = 4;
        ControlsButton();
    }

    public static void Level6Button()
    {
        GameManagerScript.LevelIndex = 5;
        ControlsButton();
    }

    public static void Level7Button()
    {
        GameManagerScript.LevelIndex = 6;
        ControlsButton();
    }

    public static void Level8Button()
    {
        GameManagerScript.LevelIndex = 7;
        ControlsButton();
    }

    public static void Level9Button()
    {
        GameManagerScript.LevelIndex = 8;
        ControlsButton();
    }

    public static void Level10Button()
    {
        GameManagerScript.LevelIndex = 9;
        ControlsButton();
    }

    public static void Level11Button()
    {
        GameManagerScript.LevelIndex = 10;
        ControlsButton();
    }

    public static void Level12Button()
    {
        GameManagerScript.LevelIndex = 11;
        ControlsButton();
    }

    public static void Level13Button()
    {
        GameManagerScript.LevelIndex = 12;
        ControlsButton();
    }

    public static void Level14Button()
    {
        GameManagerScript.LevelIndex = 13;
        ControlsButton();
    }

    public static void Level15Button()
    {
        GameManagerScript.LevelIndex = 14;
        ControlsButton();
    }

    public static void Level16Button()
    {
        GameManagerScript.LevelIndex = 15;
        ControlsButton();
    }

    public static void Level17Button()
    {
        GameManagerScript.LevelIndex = 16;
        ControlsButton();
    }

    public static void Level18Button()
    {
        GameManagerScript.LevelIndex = 17;
        ControlsButton();
    }
}