using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawButtonManager : MonoBehaviour
{
    private Button DrawButton;
    private bool isActive = true;

    public void Awake()
    {
        this.DrawButton = this.gameObject.GetComponent<Button>();
    }

    public void DrawButtonTurnRed()
    {
        ColorBlock colors = this.DrawButton.colors;
        colors.normalColor = Color.red;
        colors.highlightedColor = new Color32(255, 100, 100, 255);
        this.DrawButton.colors = colors;
    }

    public void DrawButtonTurnWhite()
    {
        ColorBlock colors = this.DrawButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(255, 255, 255, 255);
        this.DrawButton.colors = colors;
    }

    public void Deactivate()
    {
        this.DrawButtonTurnRed();
        this.isActive = false;
    }

    public void Activate()
    {
        this.DrawButtonTurnWhite();
        this.isActive = true;
    }

    public void DrawTile()
    {
        if (this.isActive)
        {
            UnityGameManager.instance.DrawTile();
        }
    }
}
