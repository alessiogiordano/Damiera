using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int _score;
    public int score { get => _score; }
    public void AddScore(int ammount) { _score += Mathf.Abs(ammount); }
    public PlayerColor color { get; }

    public Player(PlayerColor color)
    {
        this._score = 0;
        this.color = color;
    }
}
