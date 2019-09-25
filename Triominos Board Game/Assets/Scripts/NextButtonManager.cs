using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButtonManager : MonoBehaviour
{
    public bool isDebug;

    private Button NextButton;

    private void Awake()
    {
        this.NextButton = this.gameObject.GetComponent<Button>();
    }

    public void NextTurn()
    {
        if (this.isDebug)
        {
            UnityGameManager.instance.GameManager.NextTurnForDebug();
        }

        if (!UnityGameManager.instance.GameManager.TryNextTurn())
        {
            this.NextButtonTurnRed();
            new WaitForSeconds(2.0f);
            this.NextButtonTurnWhite();
        }        
    }

    public void NextButtonTurnRed()
    {
        ColorBlock colors = this.NextButton.colors;
        colors.normalColor = Color.red;
        colors.highlightedColor = new Color32(255, 100, 100, 255);
        this.NextButton.colors = colors;
    }

    public void NextButtonTurnWhite()
    {
        ColorBlock colors = this.NextButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(255, 255, 255, 255);
        this.NextButton.colors = colors;
    }
}
