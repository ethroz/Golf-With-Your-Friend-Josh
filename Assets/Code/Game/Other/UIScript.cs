using States;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Extensions;

public class UIScript : MonoBehaviour, IGameListener
{
    public GameManagerScript Game { get; private set; }

    private MenuState state;
    private Dictionary<string, Menu> menus;
    private Stack<MenuState> menuStack;
    private PowerBarScript powerBar;
    private Dictionary<Sliders, Slider> settingsSliders;

    private enum Sliders : int { Quality, Sensitivity };
    private enum Texts { Par, Score };
    private Action continueButtonCallback;

    private void Awake()
    {
        GameObject[] menuObjs = GameObject.FindGameObjectsWithTag("Menu");
        Text[] texts = FindObjectsOfType<Text>();
        Button[] buttons = FindObjectsOfType<Button>();
        Slider[] sliders = FindObjectsOfType<Slider>();
        menus = new();
        foreach (var menuObj in menuObjs)
        {
            menus.Add(menuObj.name, new Menu(menuObj));
        }
        foreach (var menu in Enum.GetNames(typeof(MenuState)))
        {
            if (!menus.ContainsKey(menu)) throw new KeyNotFoundException(menu);
        }
        AddToMenus(texts);
        AddToMenus(buttons);
        AddToMenus(sliders);
        foreach (var slider in Enum.GetValues(typeof(Sliders)))
        {
            if (((Sliders)slider).ToString() != ((Settings)slider).ToString() || (int)(Sliders)slider != (int)(Settings)slider)
            {
                throw new NotSupportedException("No match for " + ((Sliders)slider).ToString());
            }
        }
        settingsSliders = new();
        foreach (var slider in menus[nameof(MenuState.Settings)].Sliders)
        {
            settingsSliders.Add(Enum.Parse<Sliders>(slider.Key), slider.Value);
        }

        settingsSliders[Sliders.Quality].maxValue = QualitySettings.names.Length - 1;
        Game.Settings.SetCallback<int>(Settings.Quality, OnQualityChanged);
        var quality = Game.Settings.Get<int>(Settings.Quality);
        settingsSliders[Sliders.Quality].SetValueWithoutNotify(quality);
        var sensitivity = Game.Settings.Get<float>(Settings.Sensitivity);
        settingsSliders[Sliders.Sensitivity].SetValueWithoutNotify(sensitivity);

        menuStack = new();

        powerBar = GetOrThrow<PowerBarScript>();
    }

    private void OnBack(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            switch (state)
            {
                case MenuState.Main:
                    ExitButton();
                    break;
                case MenuState.Game:
                case MenuState.Continue:
                case MenuState.PowerBar:
                    PauseButton();
                    break;
                case MenuState.Pause:
                    MainMenuButton();
                    break;
                default:
                    BackButton();
                    break;
            }
        }
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }

    public void SetState(MenuState state)
    {
        this.state = state;
        foreach (var menu in menus)
        {
            menu.Value.Self.SetActive(menu.Key == state.ToString());
        }
        switch (state)
        {
            case MenuState.Main:
                menuStack.Clear();
                goto case MenuState.Pause;
            case MenuState.Pause:
                Game.StopFlashPole();
                break;
            case MenuState.Game:
            case MenuState.Continue:
            case MenuState.PowerBar:
                menuStack.Clear();
                break;
        }
    }

    public void SetPowerLevel(float amount)
    {
        powerBar.SetBar(amount);
    }

    public void SetPar()
    {
        menus[nameof(MenuState.Game)].Texts[nameof(Texts.Par)].text = string.Format("Par: {0}", Game.CurrentLevel.Par);
    }

    public void SetScore(int score)
    {
        menus[nameof(MenuState.Game)].Texts[nameof(Texts.Score)].text = string.Format("Strokes: {0}", score);
    }

    public void SliderChangedCallback(Settings setting, UnityAction<float> callback)
    {
        settingsSliders[(Sliders)(int)setting].onValueChanged.AddListener(callback);
    }

    private void OnQualityChanged(int value)
    {
        menus[nameof(MenuState.Settings)].Texts[nameof(Sliders.Quality)].text = ((Quality)value).ToString();
    }

    private void AddToMenus<T>(T[] objs) where T : Component
    {
        foreach (T obj in objs)
        {
            obj.GetComponentInParentOrThrow(out MenuTag menuTag);
            if (!menus.ContainsKey(menuTag.name)) throw new KeyNotFoundException(menuTag.name);
            menus[menuTag.name].Add(obj.name, obj);
        }
    }

    private void EnterMenu(MenuState state)
    {
        menuStack.Push(this.state);
        SetState(state);
    }

    // Buttons

    public void PlayButton()
    {
        continueButtonCallback = () =>
        {
            Game.SetState(GameState.LocalSolo);
            Game.ResetLevel(0); 
        };
        EnterMenu(MenuState.Controls);
    }
    
    public void ControlsButton()
    {
        continueButtonCallback = () => { BackButton(); };
        EnterMenu(MenuState.Controls);
    }

    public void SettingsButton()
    {
        EnterMenu(MenuState.Settings);
    }

    public void ResumeButton()
    {
        Game.SetPrevState();
        SetState(menuStack.Pop());
    }

    public void RestartButton()
    {
        Game.SetPrevState();
        Game.ResetLevel(Game.LevelIndex); // Sets the menu state.
    }

    public void ExitButton()
    {
        QuitGame();
    }

    public void PauseButton()
    {
        menuStack.Push(state);
        Game.SetState(GameState.Paused); // Sets the menu state.
    }

    public void MainMenuButton()
    {
        Game.SetState(GameState.Main); // Sets the menu state.
    }

    public void BackButton()
    {
        SetState(menuStack.Pop());
    }

    public void ContinueButton()
    {
        continueButtonCallback();
        continueButtonCallback = null;
    }

    public void LevelSelectButton()
    {
        EnterMenu(MenuState.LevelSelect);
    }

    public void LevelButton(int level)
    {
        continueButtonCallback = () => 
        {
            Game.SetState(GameState.LocalSolo);
            Game.ResetLevel(level - 1); 
        };
        EnterMenu(MenuState.Controls);
    }
}
