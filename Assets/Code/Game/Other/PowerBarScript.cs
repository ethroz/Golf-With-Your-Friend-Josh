using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Extensions;

[RequireComponent(typeof(Image))]
public class PowerBarScript : MonoBehaviour
{
    private Image bar;

    private void Awake()
    {
        this.GetComponentOrThrow(out bar);
        SetBar(0.0f);
    }

    public void SetBar(float frac)
    {
        bar.color = Color.Lerp(Color.green, Color.red, frac);
        transform.localScale = new Vector3(frac, 1.0f, 1.0f);
        transform.localPosition = new Vector3((1 - frac) * -250.0f, 54.0f, 0.0f);
    }
}
