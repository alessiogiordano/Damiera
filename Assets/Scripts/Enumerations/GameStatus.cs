using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus
{
    Uninitialized, Paused, Running
}

public static class GameStatusExtensionMethods
{
    public static bool ShouldAcceptGameInput(this GameStatus status)
    {
        switch(status) {
            case GameStatus.Running: return true;
            default: return false;
        }
    }
}