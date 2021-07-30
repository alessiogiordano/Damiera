using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public string name { get; }

    public HumanPlayer(PlayerColor color, string name) : base(color)
    {
        this.name = name;
    }
}
