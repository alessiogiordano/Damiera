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
    public PlayerColor GetOwner(BoardCell[] layout)
    {
        //string debugLayout = "[";
        //Array.ForEach(layout, element => debugLayout += element.cell + ", ");
        //debugLayout = debugLayout.Remove(debugLayout.Length -2, 2) + "]";
        //Debug.Log("layout = " + debugLayout);
        //Debug.Log("this = " + this.cell);
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                Debug.Log("this is in layout at " + i);
                return (i < (layout.Length / 2)) ? PlayerColor.White : PlayerColor.Black;
            }
        }
        Debug.Log("this is not in layout");
        return (PlayerColor) (-1);
    }
    public bool IsSibling(BoardCell[] layout, BoardCell sibling)
    {
        return sibling.GetOwner(layout) == this.GetOwner(layout);
    }
    public bool HasCapturedMovingTo(BoardCell destination)
    {
        return Mathf.Abs(destination.indices.Item1 - indices.Item1) == 2;
    }
    public BoardCell[] AvailableDestinations(BoardCell[] layout, bool bidirectional = false, bool requireCapturing = false)
    {
    //Debug.Log("AvailableDestinations(" + layout + ", " + bidirectional + ", " + requireCapturing + ");");
        if (!isValid) return new BoardCell[0];
        BoardCell[] result = new BoardCell[0];
        bool mustCapture = requireCapturing;
        BoardCell[] searchScope = bidirectional
            ? new BoardCell[] { NE, NW, SE, SW }
            : GetOwner(layout) == PlayerColor.White ? new BoardCell[] { NE, NW } : new BoardCell[] { SE, SW };
    //Debug.Log("GetOwner() = " + GetOwner(layout));
    //Debug.Log("searchScope[" + searchScope.Length + "]");
        for (int i = 0; i < searchScope.Length; i++)
        {
            // Check if cell is inside the board
            if (!searchScope[i].isValid) continue;
            // Check if cell is not occupied and the player doesn't have to capture
            if (searchScope[i].CheckAvailability(layout) && !mustCapture) 
            {
                Array.Resize(ref result, result.Length + 1);
                result[result.Length - 1] = searchScope[i];
                continue;
            }
            // Check if the cell is occupied... and NOT YOURS...
            if (!searchScope[i].CheckAvailability(layout) && !IsSibling(layout, searchScope[i]))
            {
                // ...if so check if the one after that...
                (int rowDirection, int columnDirection) = (searchScope[i].indices.Item1 - indices.Item1, searchScope[i].indices.Item2 - indices.Item2);
                BoardCell nextCell = new BoardCell(searchScope[i].indices.Item1 + rowDirection, searchScope[i].indices.Item2 + columnDirection);
                // ...is inside the board...
                if (!nextCell.isValid) continue;
                // ...and not occupied
                if (nextCell.CheckAvailability(layout))
                {
                    if (!mustCapture)
                    {
                        result = new BoardCell[0];
                        mustCapture = true;
                    }
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = nextCell;
                }
            }
        }
        Debug.Log(requireCapturing ? "Mangiate a Catena" : "Prima mossa");
        Debug.Log(mustCapture ? "Obbligo di Mangiata" : "Scelta Libera");
        Array.ForEach(result, element => Debug.Log(element.cell));
        Debug.Log("Fine");
        return result;
    }
    public bool Move(BoardCell[] layout, BoardCell destination)
    {
        if (destination.isValid) return false;
        if (!destination.CheckAvailability(layout)) return false;
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == this)
            {
                layout[i] = destination;
                return true;
            }
        }
        return false;
    }
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
    public static BoardCell[] AvailableDestinations(this BoardCell[] layout, BoardCell source, bool bidirectional = false, bool requireCapturing = false)
    {
        return source.AvailableDestinations(layout, bidirectional, requireCapturing);
    }
    public static bool Move(this BoardCell[] layout, BoardCell source, BoardCell destination)
    {
        return source.Move(layout, destination);
    }
    public static bool CheckCellValidity(this BoardCell[] layout, BoardCell tester)
    {
        return tester.isValid && tester.playable && tester.CheckAvailability(layout);
    }
    public static PlayerColor GetOwnerOf(this BoardCell[] layout, BoardCell cell)
    {
        return cell.GetOwner(layout);
    }
    public static bool Siblings(this BoardCell[] layout, BoardCell siblingOne, BoardCell siblingTwo)
    {
        return siblingOne.GetOwner(layout) == siblingTwo.GetOwner(layout);
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