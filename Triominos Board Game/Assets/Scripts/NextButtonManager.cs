using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButtonManager : MonoBehaviour
{
    public void NextTurn()
    {
        GameManager.instance.NextTurn();
    }
}
