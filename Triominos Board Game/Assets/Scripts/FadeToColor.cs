using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToColor : MonoBehaviour
{
    [Range(1, 10)]
    public int fadingSpeed = 1;

    public Color32 startingColor = new Color32(255, 0, 0, 255);
    public Color32 targetColor = new Color32(255, 252, 225, 255);
    public bool fadeOnChannelR = true;
    public bool fadeOnChannelG = false;
    public bool fadeOnChannelB = true;

    private SpriteRenderer renderer;
    private Color originColor;
    private int fadeAmountR;
    private int fadeAmountG;
    private int fadeAmountB;
    private int fadeAmount = 1;

    private void Awake()
    {
        originColor = this.GetComponent<Renderer>().material.color;
    }

    IEnumerator FadeToOrign()
    {
        bool hasChanged = true;
        int speedCount = 0;
        while(hasChanged)
        {
            Color32 actualColor = this.renderer.material.color;
            hasChanged = false;
            speedCount++;

            if (actualColor.r != targetColor.r)
            {
                actualColor = new Color32((byte)(actualColor.r + fadeAmountR), actualColor.g, actualColor.b, targetColor.a);
                hasChanged = true;
            }

            if (actualColor.g != targetColor.g)
            {
                actualColor = new Color32(actualColor.r, (byte)(actualColor.g + fadeAmountG), actualColor.b, targetColor.a);
                hasChanged = true;
            }

            if (actualColor.b != targetColor.b)
            {
                actualColor = new Color32(actualColor.r, actualColor.g, (byte)(actualColor.b + fadeAmountB), targetColor.a);
                hasChanged = true;
            }

            this.renderer.material.color = actualColor;

            if (speedCount == this.fadingSpeed)
            {
                speedCount = 0;
                yield return new WaitForSeconds(0.01f);
            }
        }

        this.renderer.material.color = originColor;
    }

    public void StartFadeToOrigin()
    {
        this.renderer = GetComponent<SpriteRenderer>();
        fadeAmountR = 0;
        fadeAmountG = 0;
        fadeAmountB = 0;

        if (fadeOnChannelR && startingColor.r != targetColor.r)
        {
            fadeAmountB = fadeAmount;
            if (startingColor.r > targetColor.r)
            {
                fadeAmountR = -fadeAmountR;
            }
        }

        if (fadeOnChannelG && startingColor.g != targetColor.g)
        {
            fadeAmountG = fadeAmount;
            if (startingColor.g > targetColor.g)
            {
                fadeAmountG = -fadeAmountG;
            }
        }

        if (fadeOnChannelB && startingColor.b != targetColor.b)
        {
            fadeAmountB = fadeAmount;
            if (startingColor.b > targetColor.b)
            {
                fadeAmountB = -fadeAmountB;
            }
        }

        Color32 colorToStart = this.startingColor;
        if (!fadeOnChannelR)
        {
            colorToStart = new Color32(targetColor.r, colorToStart.g, colorToStart.b, targetColor.a);
        }

        if (!fadeOnChannelG)
        {
            colorToStart = new Color32(colorToStart.r, targetColor.g, colorToStart.b, targetColor.a);
        }

        if (!fadeOnChannelB)
        {
            colorToStart = new Color32(colorToStart.r, colorToStart.g, targetColor.b, targetColor.a);
        }

        this.renderer.material.color = colorToStart;

        StartCoroutine(this.FadeToOrign());
    }
}
