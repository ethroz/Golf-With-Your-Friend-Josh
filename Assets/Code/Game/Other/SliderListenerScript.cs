using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class SliderListenerScript : MonoBehaviour
{
    public string Format;

    private Text text;
    private Slider parent;

    private void Awake()
    {
        this.GetComponentOrThrow(out text);
        this.GetComponentInParentOrThrow(out parent);
        Extensions.AssertTrue(Format.Length > 0);
        if (Format.Contains('{'))
        {
            parent.onValueChanged.AddListener(StringFormat);
            StringFormat(parent.value);
        }
        else
        {
            parent.onValueChanged.AddListener(FloatFormat);
            FloatFormat(parent.value);
        }
    }

    private void StringFormat(float value)
    {
        text.text = string.Format(Format, value);
    }

    private void FloatFormat(float value)
    {
        text.text = value.ToString(Format);
    }
}
