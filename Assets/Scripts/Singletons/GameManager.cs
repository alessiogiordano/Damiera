using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Parameters
    [SerializeField] private GameObject gameBoard;
    [SerializeField] private GameObject pedinaPrefab;
    [SerializeField] private Material pedinaBianca;
    [SerializeField] private Material pedinaNera;
    // Internal Structure
    private GameObject[] pedinaPool = new GameObject[25]; // Container + 24 Items
    private bool pedinaPoolReady = false;
    public BoardCell[] layout
    {
        get
        {
            if (!pedinaPoolReady) return new BoardCell[0];
            BoardCell[] result = new BoardCell[24];
            for (int i = 0; i < 24; i++)
            {
                result[i] = pedinaPool[i].GetComponent<Pedina>().cell;
            }
            return result;
        }
        set
        {
            SetupBoard(value);
        }
    }
    public GameObject GetInstanceFromCell(BoardCell cell)
    {
        if (!pedinaPoolReady) return null;
        for (int i = 0; i < 24; i++)
            if (pedinaPool[i].GetComponent<Pedina>().cell == cell) return pedinaPool[i];
        return null;
    }
    public GameObject GetInstanceFromIndex(int index)
    {
        if (pedinaPoolReady && index >= 0 && index < pedinaPool.Length)
            return pedinaPool[index];
        return null;
    }
    // Gameplay
    private GameObject selectedPedina;
    private GameObject forcedSelection;
    private BoardTurn turn;
    private PlayerColor conclusion;

    // Singleton
    private static GameManager _shared;
    public static GameManager Shared {
        get {
            if(_shared==null)
            {
                _shared = GameObject.FindObjectOfType<GameManager>();
                if(_shared==null)
                {
                    Debug.LogError("Attempted to use non instantiated singleton");
                }
            }
            return _shared;
        }
    }
    void Awake()
    {
        if(_shared==null)
        {
            _shared = this;
            GameObject.DontDestroyOnLoad(this.transform.parent.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
        NewGame();
    }
    void NewGame()
    {
        SetupBoard();
        selectedPedina = null;
        forcedSelection = null;
        /*
        SetupBoard(new BoardCell[24] {
            new BoardCell(0,0), BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell,
            new BoardCell(7,7), BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell, BoardCell.invalidCell
        });
        */
        turn = new BoardTurn(PlayerColor.White, layout);
        conclusion = (PlayerColor) (-1);
    }
    // Game Lifecycle Methods
    void SetupBoard()
    {
        SetupBoard(BoardCell.defaultLayout);
    }
    void SetupBoard(BoardCell[] layout) {
        if (layout.Length != 24)
        {
            Debug.Log("Malformed Board Layout");
            return;
        }
        // Clear if not empty
        pedinaPoolReady = true;
        for (int i = 0; i < pedinaPool.Length; i++)
        {
            Destroy(pedinaPool[i]);
        }
        // Create Container
        pedinaPool[0] = new GameObject("Pedine");
        pedinaPool[0].transform.parent = gameBoard.transform;
        pedinaPool[0].transform.localPosition = BoardCell.localCenterOffset;
        GameObject root = pedinaPool[0];
        for (int i = 0; i < 24; i++)
        {
            PlayerColor color = (i < 12) ? PlayerColor.White : PlayerColor.Black;
            pedinaPool[i] = Instantiate(pedinaPrefab);
            pedinaPool[i].GetComponent<Pedina>().Setup(i, root, layout[i], color, pedinaBianca, pedinaNera);
        }
        pedinaPoolReady = true;
    }

    public void Clicked(RaycastHit target)
    {
        if (conclusion != (PlayerColor)(-1))
        {
            SoundManager.Shared.Beep();
            return;
        }
        if (target.collider.name == "Tavolo da Gioco")
        {
            Vector3 relativePosition = target.transform.InverseTransformPoint(target.point);
            BoardCell hit = new BoardCell(relativePosition);
            if (hit.cell != null)
            {
                (bool hasMoved, bool hasCaptured, bool mustChainCapture) = MoveAction(hit);
                // React to move
                if (!hasMoved)
                    SoundManager.Shared.Beep();
                else {
                    if (mustChainCapture)
                        forcedSelection = selectedPedina;
                    else {
                        forcedSelection = null;
                        turn = turn.NextTurn();
                        conclusion = turn.CheckConclusion();
                        if (conclusion != (PlayerColor)(-1))
                        {
                            // Game Over
                            Debug.Log("Game Over - " + conclusion + " Wins");
                        }
                    }
                }
                if (forcedSelection == null && selectedPedina != null)
                {
                    selectedPedina.GetComponent<Pedina>().selected = false;
                    selectedPedina = null;
                    forcedSelection = null;
                }

            }
        }
        else if(target.collider.name == "Pedina Normale" || target.collider.name == "Dama")
        {
            GameObject pedina = target.collider.transform.parent.gameObject;
            // Validate Selection
            if (forcedSelection == null && (pedina.GetComponent<Pedina>().cell.GetOwner(layout) == turn.color))
            {
                // Set selection
                if (selectedPedina == null)
                {
                    selectedPedina = pedina;
                    pedina.GetComponent<Pedina>().selected = true;
                }
                else if (selectedPedina == pedina)
                {
                    selectedPedina = null;
                    pedina.GetComponent<Pedina>().selected = false;
                }
                else
                {
                    selectedPedina.GetComponent<Pedina>().selected = false;
                    selectedPedina = pedina;
                    pedina.GetComponent<Pedina>().selected = true;
                }
            } else {
                SoundManager.Shared.Beep();
            }
        }
    }

    // (hasMoved, hasCaptured, mustChainCapture)
    public (bool, bool, bool) MoveAction(BoardCell hit)
    {
        if (selectedPedina != null)
        {
            // Validate Move and do something with it
            BoardCell source = selectedPedina.GetComponent<Pedina>().cell;
            bool isDama = selectedPedina.GetComponent<Pedina>().dama;
            //if (layout.AvailableDestinations(source, isDama).Contains(hit))
            int moveIndex = turn.Check(source, hit);
            if (moveIndex != -1)
            {
                BoardMove move = turn.moves[moveIndex];
                (bool turnNotOver, bool hasMoved, bool hasCaptured) = turn.Procede(move);
                // Move
                selectedPedina.GetComponent<Pedina>().cell = hit;
                // Check if dama
                if (!isDama)
                {
                    isDama = (turn.color == PlayerColor.White) ? hit.indices.Item2 == 7 : hit.indices.Item2 == 0;
                    selectedPedina.GetComponent<Pedina>().dama = isDama;
                }
                // Check if captured
                if(hasCaptured)
                {
                    GetInstanceFromCell(move.capture).GetComponent<Pedina>().captured = true;
                    if (turnNotOver)
                        return (true, true, true);
                    else
                    {
                        return (true, true, false);
                    }
                }
                else
                {
                    return (true, false, false);
                }
            }
            else
            {
                return (false, false, false);
            }
        }
        else
        {
            return (false, false, false);
        }
    }
}
