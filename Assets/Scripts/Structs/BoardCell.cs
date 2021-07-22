using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardCell
{
    private int _column;
    private static string[] _columnValues = new string[8] {"A", "B", "C", "D", "E", "F", "G", "H"};
    private int _row;
    private static string[] _rowValues = new string[8] {"1", "2", "3", "4", "5", "6", "7", "8"};

    public string cell
    {
        get
        {
            if (_column < 0 || _column > 7 || _row < 0 || _row > 7)
            {
                return null;
            }
            else
            {
                return _columnValues[_column] + _rowValues[_row];
            }
        }
    }

    public override string ToString() => cell;

    public BoardCell(Vector3 relativePosition) {
        // Coordinates in Centimeters starting from the center of the board
        int xCM = (int) (relativePosition.x * 100);
        int yCM = (int) (relativePosition.z * 100);
        // Reject overflowing input
        if ((xCM > 12) || (xCM < -12) || (yCM > 12) || (yCM < -12))
        {
            _column = -1;
            _row = -1;
            return;
        }
        // Or adjust it to make it valid
        /*
        {
            xCM = (xCM > 12) ? 12 : xCM;
            xCM = (xCM < -12) ? -12 : xCM;
            yCM = (yCM > 12) ? 12 : yCM;
            yCM = (yCM < -12) ? -12 : yCM;
        }
        */
        // Coordinates in Centimeters starting from A1
        xCM = Mathf.Abs(xCM - 12);
        yCM = Mathf.Abs(yCM - 12);
        // Return indices
        _column = xCM / 3;
        _row = yCM / 3;
        return;
    }
    public BoardCell(int x, int y)
    {
        _column = x;
        _row = y;
        return;
    }
}
