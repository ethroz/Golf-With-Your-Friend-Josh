using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Menu
{
    public GameObject Self;
    public Dictionary<string, Text> Texts;
    public Dictionary<string, Button> Buttons;
    public Dictionary<string, Slider> Sliders;

    public Menu(GameObject self)
    {
        Self = self;
        Texts = new();
        Buttons = new();
        Sliders = new();
    }

    public void Add<T>(string name, T obj)
    {
        if (typeof(T) == typeof(Text)) Texts.Add(name, (Text)(object)obj);
        else if (typeof(T) == typeof(Button)) Buttons.Add(name, (Button)(object)obj);
        else if (typeof(T) == typeof(Slider)) Sliders.Add(name, (Slider)(object)obj);
        else throw new TypeAccessException();
    }
}
