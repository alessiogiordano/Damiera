using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Neutral, Top, Bottom, Left, Right
}
public static class DirectionExtensionMethods
{
    public static Direction ToDirection(this KeyCode code)
    {
        switch(code) {
            case KeyCode.LeftArrow: return Direction.Left;
            case KeyCode.RightArrow: return Direction.Right;
            case KeyCode.UpArrow: return Direction.Top;
            case KeyCode.DownArrow: return Direction.Bottom;
            default: return Direction.Neutral;
        }
    }
    public static KeyCode ToKeyCode(this Direction dir)
    {
        switch(dir) {
            case Direction.Left: return KeyCode.LeftArrow;
            case Direction.Right: return KeyCode.RightArrow;
            case Direction.Top: return KeyCode.UpArrow;
            case Direction.Bottom: return KeyCode.DownArrow;
            default: return KeyCode.None;
        }
    }
    public static Direction ToDirectionStartingFrom(this Vector3 endPosition, Vector3 startPosition)
    {
        float deltaX = startPosition.x - endPosition.x;
        float deltaY = startPosition.y - endPosition.y;
        // Choose Axis of Rotation
        bool chooseRow = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);
        // Choose Quadrant
        if (deltaX > 0 && deltaY > 0) {
            return chooseRow ? Direction.Left : Direction.Bottom;
        } else if (deltaX > 0 && deltaY < 0) {
            return chooseRow ? Direction.Left : Direction.Top;
        } else if (deltaX < 0 && deltaY < 0) {
            return chooseRow ? Direction.Right : Direction.Top;
        } else if (deltaX < 0 && deltaY > 0) {
            return chooseRow ? Direction.Right : Direction.Bottom;
        } else {
            return Direction.Neutral;
        }
    }
}
