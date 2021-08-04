using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardTurn
{
    public Player currentPlayer { get; }

    public Player adversaryPlayer { get; }
    private BoardMove[] _moves;
    public BoardMove[] moves { get => _moves; }
    private BoardCell[] _layout;
    public BoardCell[] layout { get => _layout; }
    private bool[] _damaLayout;
    public bool[] damaLayout { get => _damaLayout; }
    public bool completed { get => _moves.Length == 0; }

    public BoardTurn(Player currentPlayer, Player adversaryPlayer, BoardCell[] layout, bool[] damaLayout)
    {
        this.currentPlayer = currentPlayer;
        this.adversaryPlayer = adversaryPlayer;
        this._layout = layout;
        this._damaLayout = damaLayout;
        //this._layout.DebugString();
        // Construct Array
        BoardMove[] result = new BoardMove[0];
        int layoutSubsetLength = layout.Length / 2;
        int layoutSubsetOffset = (currentPlayer.color == PlayerColor.White) ? 0 : layoutSubsetLength;
        for (int i = 0; i < layoutSubsetLength; i++)
        {
            BoardMove[] partialResult = layout[i + layoutSubsetOffset].AvailableDestinations(layout, damaLayout);
            int partialResultLength = partialResult.Length;
            if (partialResultLength > 0)
            {
                int copyIndex = result.Length;
                Array.Resize(ref result, copyIndex + partialResultLength);
                partialResult.CopyTo(result, copyIndex);
            }
            
        }
        //result.DebugString(); // Fino a qui funziona bene
        // Evaluate Array 
        int maxValue = 0;
        BoardMove[] processedResult = new BoardMove[0];
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].value > maxValue)
            {
                maxValue = result[i].value;
                processedResult = new BoardMove[0];
            }
            if (result[i].value == maxValue)
            { 
                Array.Resize(ref processedResult, processedResult.Length + 1);
                processedResult[processedResult.Length - 1] = result[i];
            }
        }
        this._moves = processedResult;
        //this._moves.DebugString(); // Crasha qui
    }

    public int Check(BoardCell source, BoardCell destination)
    {
        for (int i = 0; i < _moves.Length; i++)
        {
            if(_moves[i].source == source && _moves[i].destination == destination)
                return i;
        }
        return -1;
    }
    // MoreMovesLeft, HasMoved, HasCaptured
    public (bool, bool, bool, bool) Procede(BoardMove move)
    {
        // Update Moves Array
        BoardMove[] newMoveSet = new BoardMove[0];
        for (int i = 0; i < _moves.Length; i++)
        {
            if(_moves[i].source == move.source && _moves[i].destination == move.destination && _moves[i].chain != null)
            {
                Array.Resize(ref newMoveSet, newMoveSet.Length + 1);
                newMoveSet[newMoveSet.Length - 1] = _moves[i].chain;
            }
        }
        _moves = newMoveSet;
        //this._moves.DebugString();
        // Update Layout
        bool turnNotOver = newMoveSet.Length > 0;
        (BoardCell[] newLayout, bool[] newDamaLayout, bool hasMoved, bool hasCaptured, bool hasGraduated) = move.ApplyTo(_layout, _damaLayout);
        this._layout = newLayout;
        this._damaLayout = newDamaLayout;
        currentPlayer.AddScore(move.moveScore);
        return (turnNotOver, hasMoved, hasCaptured, hasGraduated);
    }

    public (BoardTurn, Player) NextTurn(bool turnCamera = true)
    {
        BoardTurn nextTurn = new BoardTurn(adversaryPlayer, currentPlayer, _layout, _damaLayout);
        Player winner = nextTurn.CheckConclusion();
        if (turnCamera && (winner == null)) CameraManager.Shared.TurnCameraToDefault(180f * ((int) nextTurn.currentPlayer.color), true, 1.0f);
        return (nextTurn, winner);
    }

    public Player CheckConclusion()
    {
        // Check if enemy has pieces
        int enemyOffset = (adversaryPlayer.color == PlayerColor.White) ? 0 : 12;
        bool enemyHasPieces = false;
        for (int i = 0 + enemyOffset; i < 12 + enemyOffset; i++)
        {
            if (_layout[i].isValid)
            {
                enemyHasPieces = true;
                break;
            }
        }
        if (!enemyHasPieces) return currentPlayer; // You Won
        if (_moves.Length == 0) return adversaryPlayer; // You Lost
        return null; // Keep Going
    }
}