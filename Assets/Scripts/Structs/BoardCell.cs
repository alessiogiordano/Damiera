using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardCell
{
    private int _column;
    private static string[] _columnValues = new string[8] {"A", "B", "C", "D", "E", "F", "G", "H"};
    private int _row;
    private static string[] _rowValues = new string[8] {"1", "2", "3", "4", "5", "6", "7", "8"};

    private static float centerCorrection = 0.9990662932f;
    private static float lengthCorrection = 1.0209039548f;

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
    public bool isActive
    {
        get
        {
            return cell != null;
        }
        // Isn't this potentially dangerous? A deactivate method would be better...
        set
        {
            if (value && (_row < 0 && _column < 0)) {
                _row = _row * -1;
                _column = _column * -1;
            }
            else if (!value && (_row > 0 && _column > 0))
            {
                _row = _row * -1;
                _column = _column * -1;
            }
        }
    }
    public Vector3 localPosition
    {
        get
        {
            // Universal Values
            float cellDimension = 0.03f * lengthCorrection;
            float startingPoint = cellDimension * 3.5f;
            // Cell-specific values
            float startingX = startingPoint - (cellDimension * _column);
            float startingZ = startingPoint - (cellDimension * _row);
            return new Vector3(startingX, 0.0f, startingZ);
        }
    }

    public Vector3 toGlobalPosition(Transform parent)
    {
        Vector3 local = localPosition;
        return new Vector3(local.x + parent.position.x, parent.position.y, local.z + parent.position.z);
    }

    public override string ToString() => cell;

    public BoardCell(Vector3 relativePosition) {
        // Adjust relativePosition to account for human error in the texture
        float rootOffset = localCenterOffset.x;
        float cellLength = 3 * lengthCorrection;
        float groupLength = cellLength * 4;
        // Coordinates in Centimeters starting from the center of the board
        float xCM = (relativePosition.x + rootOffset) * 100;
        float yCM = (relativePosition.z - rootOffset) * 100;
        // Reject overflowing input
        if ((xCM > groupLength) || (xCM < -groupLength) || (yCM > groupLength) || (yCM < -groupLength))
        {
            _column = -1;
            _row = -1;
            return;
        }
        // Coordinates in Centimeters starting from A1
        xCM = Mathf.Abs(xCM - groupLength);
        yCM = Mathf.Abs(yCM - groupLength);
        // Return indices
        _column = (int) (xCM / cellLength);
        _row = (int) (yCM / cellLength);
        return;
    }
    public BoardCell(int x, int y)
    {
        _column = x;
        _row = y;
        return;
    }
    public static BoardCell[] validCells
    {
        get
        {
            BoardCell[] result = new BoardCell[32];
            for (int i_row = 0; i_row < 8; i_row++)
            {
                for (int i_column = 0; i_column < 4; i_column++)
                {
                    int row_value = i_row;
                    int column_value = (i_row % 2 == 0) ? i_column * 2: (i_column * 2) + 1;
                    result[i_column + (4*i_row)] = new BoardCell(column_value, row_value);
                }
            }
            return result;
        }
    }
    public static BoardCell[] defaultLayout
    {
        get
        {
            BoardCell[] valid = validCells;
            BoardCell[] result = new BoardCell[24];
            for (int i = 0; i < 24; i++)
            {
                bool isWhite = i < 12;
                int validCellIndex = (isWhite) ? i : (validCells.Length - 1 - (i - 12));
                result[i] = valid[validCellIndex];
            }
            return result;
        }
    }
    public static Vector3 localCenterOffset
    {
        get
        {
            float rootOffset = (0.272f / 2) * (1.0f - centerCorrection);
            return new Vector3(rootOffset, 0.0f, -rootOffset);
        }
    }
}
