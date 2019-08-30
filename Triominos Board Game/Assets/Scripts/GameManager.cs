using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardManager;

    private PlayerCode playerCode = PlayerCode.None;
    private int numbDraws = 0;
    private readonly int[] playerPoints;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        boardManager = GetComponent<BoardManager>();
        this.InitGame();
    }

    public void InitGame()
    {
        boardManager.InitBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
