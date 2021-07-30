using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : Player
{
    public int movesAhead { get; }

    public ComputerPlayer(PlayerColor color, int movesAhead): base(color)
    {
        this.movesAhead = Mathf.Abs(movesAhead);
    }

    public BoardMove ChooseMove(BoardTurn turn)
    {
        // Setup
        int levelsLeft = movesAhead / 2; // Io considero a coppie
        (BoardMove, float, BoardTurn[], Player)[] movesEvaluation = new (BoardMove, float, BoardTurn[], Player)[turn.moves.Length];
        for (int i = 0; i < turn.moves.Length; i++) movesEvaluation[i] = (turn.moves[i], 0, new BoardTurn[1] { turn }, null);
        // Collect Evaluation Values
        while (levelsLeft > 0)
        {
            int winningMoves = 0;
            int losingMoves = 0;
            int neutralMoves = 0;

            for (int i = 0; i < movesEvaluation.Length; i++)
            {
                float previousValue = movesEvaluation[i].Item2;
                movesEvaluation[i] = EvaluateMoveTuple(movesEvaluation[i]);   // Max
                float maxValue = movesEvaluation[i].Item2;
                if (levelsLeft > 1 || movesAhead % 2 == 0)
                {
                    if (movesEvaluation[i].Item4 != turn.currentPlayer)
                        movesEvaluation[i] = EvaluateMoveTuple(movesEvaluation[i]);  // Min
                    // Evaluate value of Move Pair
                    movesEvaluation[i].Item2 = previousValue + ((previousValue + maxValue) / 2) - ((previousValue + movesEvaluation[i].Item2) / 2);
                } else movesEvaluation[i].Item2 = previousValue + ((previousValue + maxValue) / 2);
                if (movesEvaluation[i].Item4 != null)
                {
                    movesEvaluation[i] = (movesEvaluation[i].Item1, movesEvaluation[i].Item2, new BoardTurn[0], movesEvaluation[i].Item4);
                    if (movesEvaluation[i].Item4 == turn.currentPlayer) winningMoves++;
                    else losingMoves++;
                }
                else neutralMoves++;
            }
            // Check which moves led you to win or lose, if (1) break out of loop and evaluate those only, otherwise remove bad ones?
            if (winningMoves > 0)
            {
                int winningIterator = 0;
                (BoardMove, float, BoardTurn[], Player)[] winningSubset = new (BoardMove, float, BoardTurn[], Player)[winningMoves];
                Array.ForEach(movesEvaluation, item => { if(item.Item4 == turn.currentPlayer) { winningSubset[winningIterator] = item; winningIterator++; }});
                movesEvaluation = winningSubset;
                levelsLeft = 0;
            }
            else if (neutralMoves > 0 && losingMoves > 0)
            {
                int losingIterator = 0;
                (BoardMove, float, BoardTurn[], Player)[] losingSubset = new (BoardMove, float, BoardTurn[], Player)[losingMoves];
                Array.ForEach(movesEvaluation, item => { if(item.Item4 == turn.adversaryPlayer) { losingSubset[losingIterator] = item; losingIterator++; }});
                movesEvaluation = losingSubset;
            }
            levelsLeft--;
        }
        // Pick the highest value, eventually randomly picking one
        if (movesEvaluation.Length > 0)
        {
            (BoardMove, float, BoardTurn[], Player) bestMove = movesEvaluation[0];
            Array.ForEach(movesEvaluation, item => {
                if (bestMove.Item2 < item.Item2) bestMove = item;
                else if (bestMove.Item2 == item.Item2) 
                {
                    int bestDestinationRow = bestMove.Item1.destination.indices.Item2;
                    int candidateDestinationRow = item.Item1.destination.indices.Item2;
                    if (
                        (turn.currentPlayer.color == PlayerColor.White && candidateDestinationRow > bestDestinationRow)
                        ||
                        (turn.currentPlayer.color == PlayerColor.Black && candidateDestinationRow < bestDestinationRow)
                    ) bestMove = item;
                    else bestMove = (new System.Random().NextDouble() > 0.5) ? bestMove : item;
                }
            });
            return bestMove.Item1;
        }
        else return null;
    }

    public (BoardMove, float, BoardTurn[], Player) EvaluateMoveTuple((BoardMove move, float value, BoardTurn[] turns, Player winner) moveTuple)
    {
        float newValue = 0;
        BoardTurn[] nextTurns = new BoardTurn[0];
        for (int i = 0; i < moveTuple.turns.Length; i++)
        {
            float turnValue = 0;
            for (int j = 0; j < moveTuple.turns[i].moves.Length; j++)
            {
                BoardTurn copyOfTurn = moveTuple.turns[i];
                (bool turnNotOver, bool hasMoved, bool hasCaptured, bool hasGraduated) = copyOfTurn.Procede(copyOfTurn.moves[j]);
                while (turnNotOver)
                {
                    (turnNotOver, hasMoved, hasCaptured, hasGraduated) = copyOfTurn.Procede(copyOfTurn.moves[0]);
                }
                // Evaluate Turn
                float valueComponent = 0;
                for(int k = 0; k < copyOfTurn.layout.Length; k++)
                {
                    if (copyOfTurn.layout[k].isValid)
                    {
                        if (k < copyOfTurn.layout.Length) valueComponent++;
                        else valueComponent--;
                    }
                }
                if (copyOfTurn.currentPlayer.color != PlayerColor.White) valueComponent *= (-1);
                // Check if game is over
                (BoardTurn copyOfNextTurn, Player winner) = copyOfTurn.NextTurn(false);
                if(winner != null) return (moveTuple.move, moveTuple.value + valueComponent, new BoardTurn[0], winner);
                // Add NextTurn
                Array.Resize(ref nextTurns, nextTurns.Length + 1);
                nextTurns[nextTurns.Length - 1] = copyOfNextTurn;
                turnValue += valueComponent;
            }
            turnValue /= moveTuple.turns[i].moves.Length; // Average
            newValue += turnValue;
        }
        newValue = (newValue + moveTuple.value) / 2;
        return (moveTuple.move, newValue, nextTurns, null);
    }
    public void PlayMove(BoardMove move)
    {
        GameManager.Shared.RenderMove(move.source, move.destination);
    }
}
