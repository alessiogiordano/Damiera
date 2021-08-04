using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMove
{
    public BoardCell source { get; }
    public BoardCell destination { get; }
    public BoardCell capture { get; }
    public BoardMove chain { get; }
    private int _value;
    public int value
    {
        get
        {
            if (chain != null)
                return (_value + chain.value);
            return _value;
        }
    }
    public int moveScore { get => _value; }
    public BoardMove(BoardCell source, BoardCell destination)
    {
        this.source = source;
        this.destination = destination;
        this.capture = BoardCell.invalidCell;
        this._value = 0;
        this.chain = null;
    }
    public BoardMove(BoardCell source, BoardCell destination, BoardCell capture, int value = 0)
    {
        this.source = source;
        this.destination = destination;
        this.capture = capture;
        this._value = value;
        this.chain = null;
    }
    public BoardMove(BoardCell source, BoardCell destination, BoardCell capture, BoardMove chain, int value = 0)
    {
        this.source = source;
        this.destination = destination;
        this.capture = capture;
        this._value = value;
        this.chain = chain;
    }

    public override string ToString()
    {
        return "{ source: " + source.cell + ", destination: " + destination.cell + ", capture: " + capture.cell + ", value: " + value + ", chain: " + (chain != null ? "true" : "false");
    }
    public (BoardCell[], bool, bool) ApplyTo(BoardCell[] layout)
    {
        var fullApplyResult = ApplyTo(layout, new bool[0]);
        return (fullApplyResult.Item1, fullApplyResult.Item3, fullApplyResult.Item4);
    }
    public (BoardCell[], bool[], bool, bool, bool) ApplyTo(BoardCell[] layout, bool[] damaLayout)
    {
        BoardCell[] newLayout = (BoardCell[]) layout.Clone();
        bool[] newDamaLayout = (bool[]) damaLayout.Clone();
        bool hasMoved = false;
        bool hasCaptured = false;
        bool hasGraduated = false;
        BoardCell capturedCell = this.capture;
        for (int i = 0; i < newLayout.Length; i++)
        {
            // Refresh layout
            if (newLayout[i] == this.source)
            {
                hasMoved = true;
                newLayout[i] = this.destination;
                // Refresh damaLayout
                if (layout.Length == damaLayout.Length)
                {
                    bool becameDama = (this.destination.GetOwner(newLayout) == PlayerColor.White) ? this.destination.indices.Item2 == 7 : this.destination.indices.Item2 == 0;
                    if (!newDamaLayout[this.destination.GetIndex(newLayout)] && becameDama) hasGraduated = true;
                    newDamaLayout[this.destination.GetIndex(newLayout)] = newDamaLayout[this.destination.GetIndex(newLayout)] || becameDama;
                }
            }
            // Disable captured piece
            if (this.capture.isValid && newLayout[i] == this.capture)
            {
                hasCaptured = true;
                newLayout[i] = BoardCell.invalidCell;
            }
        }
        return (newLayout, newDamaLayout, hasMoved, hasCaptured, hasGraduated);
    }

    public static bool[] UpdateDamaLayout(BoardCell[] layout, bool[] damaLayout)
    {
        // (GetOwner(layout) == PlayerColor.White) ? nextCell.indices.Item2 == 7 : nextCell.indices.Item2 == 0;
        if (layout.Length != damaLayout.Length) return damaLayout;
        bool[] alteredDamaLayout = (bool[]) damaLayout.Clone();
        for (int i = 0; i < layout.Length; i++)
        {
            if(i < (layout.Length / 2) && layout[i].indices.Item2 == 7)
                alteredDamaLayout[i] = true; // White Piece at top of board
            else if(i >= (layout.Length / 2) && layout[i].indices.Item2 == 0)
                alteredDamaLayout[i] = true; // Dark Piece at bottom of board
        }
        return alteredDamaLayout;
    }
}

public static class BoardMoveExtensionMethods
{
    public static void DebugString(this BoardMove[] moves)
    {
        string debugMoves = "{";
        Array.ForEach(moves, element => debugMoves += element.ToString() + ", ");
        if (debugMoves.Length > 2)
            debugMoves = debugMoves.Remove(debugMoves.Length -2, 2) + "}";
        else debugMoves += "}";
        Debug.Log("Moves = [" + debugMoves + "]");
    }
}