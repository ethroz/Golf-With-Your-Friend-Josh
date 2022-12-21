using UnityEngine;
using UnityEngine.UI;

public class Menu
{
    public GameObject Self;
    public Text[] Texts;
    public Button[] Buttons;
    public Slider[] Sliders;

    public Menu(GameObject g)
    {
        Self = g;
    }
}