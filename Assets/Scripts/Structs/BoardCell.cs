using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardCell : System.IEquatable<BoardCell>, System.IComparable<BoardCell>
{
    // Internal Data
    private int _column;
    private static string[] _columnValues = new string[8] {"A", "B", "C", "D", "E", "F", "G", "H"};
    private int _row;
    private static string[] _rowValues = new string[8] {"1", "2", "3", "4", "5", "6", "7", "8"};

    private static float centerCorrection = 0.9990662932f;
    private static float lengthCorrection = 1.0209039548f;
    /* ********** */

    // External Data
    public string cell
    {
        get
        {
            if (!isValid)
            {
                return null;
            }
            else
            {
                return _columnValues[_column] + _rowValues[_row];
            }
        }
    }
    public (int, int) indices
    {
        get
        {
            return (_column, _row);
        }
    }
    public bool isValid
    {
        get
        {
            return !(_column < 0 || _column > 7 || _row < 0 || _row > 7);
        }
        set
        {
            if (!value)
            {
                _column = -1;
                _row = -1;
            }
            else
            {
                Debug.Log("An invalid cell cannot be brought back to validity");
            }
        }
    }
    /* ********** */

    // Helper conversions for Scene Instantiation
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
    public Vector3 ToGlobalPosition(Transform parent)
    {
        Vector3 local = localPosition;
        return new Vector3(local.x + parent.position.x, parent.position.y, local.z + parent.position.z);
    }
    /* ********** */

    // Gameplay-related methods
    public int GetIndex(BoardCell[] layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerColor GetOwner(BoardCell[] layout)
    {
        int i = GetIndex(layout);
        Debug.Log("this is in layout at " + i);
        if (i != -1)
            return (i < (layout.Length / 2)) ? PlayerColor.White : PlayerColor.Black;
        return (PlayerColor) (-1);
        /*
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                //Debug.Log("this is in layout at " + i);
                return (i < (layout.Length / 2)) ? PlayerColor.White : PlayerColor.Black;
            }
        }
        return (PlayerColor) (-1);
        */
    }
    public bool IsSibling(BoardCell[] layout, BoardCell sibling)
    {
        return sibling.GetOwner(layout) == this.GetOwner(layout);
    }
    public bool HasCapturedMovingTo(BoardCell destination)
    {
        return Mathf.Abs(destination.indices.Item1 - indices.Item1) == 2;
    }
    public BoardMove[] AvailableDestinations(BoardCell[] layout, bool[] damaLayout, bool requireCapturing = false, int maximumChain = 3)
    {
        if (!isValid) return new BoardMove[0];
        BoardMove[] result = new BoardMove[0];
        bool bidirectional = damaLayout[GetIndex(layout)];
        BoardCell[] searchScope = bidirectional
            ? new BoardCell[] { NE, NW, SE, SW }
            : GetOwner(layout) == PlayerColor.White ? new BoardCell[] { NE, NW } : new BoardCell[] { SE, SW };
        for (int i = 0; i < searchScope.Length; i++)
        {
            // Check if cell is inside the board
            if (!searchScope[i].isValid) continue;
            // Check if cell is not occupied and the player doesn't have to capture
            if (searchScope[i].CheckAvailability(layout) && !requireCapturing) 
            {
                Array.Resize(ref result, result.Length + 1);
                result[result.Length - 1] = new BoardMove(this, searchScope[i]);
                continue;
            }
            // Check if the cell is occupied... and NOT YOURS...
            if (!searchScope[i].CheckAvailability(layout) && !IsSibling(layout, searchScope[i]))
            {
                // ... and if it is dama and you are not...
                bool captureIsDama = damaLayout[searchScope[i].GetIndex(layout)];
                if (captureIsDama && !bidirectional) continue;
                // ...if so check if the one after that...
                (int rowDirection, int columnDirection) = (searchScope[i].indices.Item1 - indices.Item1, searchScope[i].indices.Item2 - indices.Item2);
                BoardCell nextCell = new BoardCell(searchScope[i].indices.Item1 + rowDirection, searchScope[i].indices.Item2 + columnDirection);
                // ...is inside the board...
                if (!nextCell.isValid) continue;
                // ...and not occupied
                if (nextCell.CheckAvailability(layout))
                {
                    BoardMove[] chainedMoves = new BoardMove[0];
                    if (maximumChain > 1 || bidirectional)
                    {
                        var applyChanges = new BoardMove(this, nextCell, searchScope[i]).ApplyTo(layout, damaLayout);
                        BoardCell[] alteredLayout = applyChanges.Item1;
                        bool[] alteredDamaLayout = applyChanges.Item2;
                        chainedMoves = nextCell.AvailableDestinations(alteredLayout, alteredDamaLayout, true, maximumChain - 1);
                    }
                    if (chainedMoves.Length > 0)
                    {
                        BoardCell sourceCell = this;
                        Array.ForEach(chainedMoves, chain => {
                            Array.Resize(ref result, result.Length + 1);
                            result[result.Length - 1] = new BoardMove(sourceCell, nextCell, searchScope[i], chain, (captureIsDama) ? 3 : 1);
                        });
                    }
                    else
                    {
                        // Assign
                        Array.Resize(ref result, result.Length + 1);
                        result[result.Length - 1] = new BoardMove(this, nextCell, searchScope[i], (captureIsDama) ? 3 : 1);
                    }
                }
            }
        }
        return result;
    }
    // Manipulate Array of BoardCell
    /*
    public bool Move(BoardCell[] layout, BoardCell destination)
    {
        if (destination.isValid) return false;
        if (!destination.CheckAvailability(layout)) return false;
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                layout[i] = destination;
                Debug.Log("Moved: " + this.cell + " to " + destination.cell);
                return true;
            }
        }
        return false;
    }
    public bool CaptureIn(BoardCell[] layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                layout[i] = BoardCell.invalidCell;
                Debug.Log("Captured: " + this.cell);
                return true;
            }
        }
        return false;
    }*/
    public bool CheckAvailability(BoardCell[] layout)
    {
        BoardCell thisOne = this;
        return !Array.Exists(layout, element => element == thisOne);
    }
    public bool playable
    {
        get
        {
            BoardCell thisOne = this;
            return Array.Exists(BoardCell.validCells, element => element == thisOne);   
        }
    }
    public BoardCell NE { get => new BoardCell(_column + 1, _row + 1); }
    public BoardCell NW { get => new BoardCell(_column - 1, _row + 1); }
    public BoardCell SE { get => new BoardCell(_column + 1, _row - 1); }
    public BoardCell SW { get => new BoardCell(_column - 1, _row - 1); }
    public BoardCell CellBetweenDiagonal(BoardCell other)
    {
        int offsetColumn = (other.indices.Item1 - indices.Item1) / 2;
        int offsetRow = (other.indices.Item2 - indices.Item2) / 2;
        if (Mathf.Abs(offsetColumn) != 1 && Mathf.Abs(offsetRow) != 1 )
            return BoardCell.invalidCell;
        return new BoardCell(_column + offsetColumn, _row + offsetRow);
    }
    /* ********** */

    // Standard methods override
    public override string ToString() => cell;
    public override bool Equals(object obj) => obj is BoardCell other && this.Equals(other);
    public bool Equals(BoardCell other) => cell == other.cell;
    public override int GetHashCode() => (_column, _row).GetHashCode();
    public static bool operator ==(BoardCell lhs, BoardCell rhs) => lhs.Equals(rhs);
    public static bool operator !=(BoardCell lhs, BoardCell rhs) => !(lhs == rhs);
    public int CompareTo(BoardCell other)
    {
        if (this.Equals(other)) return 0;
        if ((indices.Item2 > other.indices.Item2) || ((indices.Item2 == other.indices.Item2) && (indices.Item1 > other.indices.Item1)))
            return 1;
        return -1;
    }
    public static bool operator <=(BoardCell lhs, BoardCell rhs) => lhs.CompareTo(rhs) <= 0;
    public static bool operator >=(BoardCell lhs, BoardCell rhs) => lhs.CompareTo(rhs) >= 0;
    /* ********** */

    // Constructors
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
    /* ********** */

    // Common values
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
    public static BoardCell invalidCell
    {
        get
        {
            return new BoardCell(-1, -1);
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
    /* ********** */
}

public static class BoardCellExtensionMethods
{
    public static float AveragePlayerSpread(this BoardCell[] layout)
    {
        (float, float) whiteBalance = (0, 0);
        float whiteCounter = 0;
        (float, float) blackBalance = (0, 0);
        float blackCounter = 0;
        for (int i = 0; i < layout.Length; i++)
        {
            if (!layout[i].isValid) continue;
            if (i < layout.Length / 2)
            {
                whiteBalance = (whiteBalance.Item1 + layout[i].indices.Item1, whiteBalance.Item2 + layout[i].indices.Item2);
                whiteCounter++;
            }
            else
            {
                blackBalance = (blackBalance.Item1 + layout[i].indices.Item1, blackBalance.Item2 + layout[i].indices.Item2);
                blackCounter++;
            }
        }
        whiteBalance = (whiteBalance.Item1 / whiteCounter, whiteBalance.Item2 / whiteCounter);
        blackBalance = (blackBalance.Item1 / blackCounter, blackBalance.Item2 / blackCounter);
        return Mathf.Sqrt(Mathf.Pow(whiteBalance.Item1 - blackBalance.Item1,2) + Mathf.Pow(whiteBalance.Item2 - blackBalance.Item2,2));
    }
    public static bool Contains(this BoardCell[] layout, BoardCell cell)
    {
        return Array.Exists(layout, element => element == cell);
    }
    public static void DebugString(this BoardCell[] layout)
    {
        string debugLayout = "[";
        Array.ForEach(layout, element => debugLayout += element.cell + ", ");
        debugLayout = debugLayout.Remove(debugLayout.Length -2, 2) + "]";
        Debug.Log("layout = " + debugLayout);
    }
}