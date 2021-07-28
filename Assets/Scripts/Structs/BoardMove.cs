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
        BoardCell[] newLayout = (BoardCell[]) layout.Clone();
        bool hasMoved = false;
        bool hasCaptured = false;
        BoardCell capturedCell = this.capture;
        for (int i = 0; i < newLayout.Length; i++)
        {
            // Refresh layout
            if (newLayout[i] == this.source)
            {
                hasMoved = true;
                newLayout[i] = this.destination;
            }
            // Disable captured piece
            if (this.capture.isValid && newLayout[i] == this.capture)
            {
                hasCaptured = true;
                //pedinaPool[i].GetComponent<Pedina>().captured = true;
                newLayout[i] = BoardCell.invalidCell;
            }
        }
        return (newLayout, hasMoved, hasCaptured);
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