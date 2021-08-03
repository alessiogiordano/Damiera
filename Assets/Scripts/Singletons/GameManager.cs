using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
    public bool[] damaLayout
    {
        get
        {
            if (!pedinaPoolReady) return new bool[0];
            bool[] result = new bool[24];
            for (int i = 0; i < 24; i++)
            {
                result[i] = pedinaPool[i].GetComponent<Pedina>().dama;
            }
            return result;
        }
        set
        {
            if (value.Length == pedinaPool.Length)
                for (int i = 0; i < pedinaPool.Length; i++)
                {
                    pedinaPool[i].GetComponent<Pedina>().dama = value[i];
                }
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

    private GameStatus _status;
    private int selectedSlot;
    public GameStatus status { get => _status; }
    public void PauseGame() { if(_status != GameStatus.Uninitialized) _status = GameStatus.Paused; }
    public void ResumeGame() { if(_status != GameStatus.Uninitialized) _status = GameStatus.Running; }
    public string shortStats
    {
        get
        {
            Player firstPlayer = turn.currentPlayer;
            string firstPlayerName = (firstPlayer is HumanPlayer) ? ((HumanPlayer) firstPlayer).name : "Computer";
            Player secondPlayer = turn.adversaryPlayer;
            string secondPlayerName = (secondPlayer is HumanPlayer) ? ((HumanPlayer) secondPlayer).name : "Computer";
            string gameStatus = (conclusion != (PlayerColor)(-1)) ? ((conclusion == PlayerColor.White) ? "(Vince il bianco)" : (conclusion == PlayerColor.Black) ? "(Vince il nero)" : "(Patta)") : ((firstPlayer.color == PlayerColor.White) ? "(Muove il bianco)" : (firstPlayer.color == PlayerColor.Black) ? "(Muove il nero)" : "");
            return (firstPlayer.color == PlayerColor.White) ? $"{firstPlayerName} - {secondPlayerName}   {gameStatus}" : $"{secondPlayerName} - {firstPlayerName}   {gameStatus}";
        }
    }
    public (int, string, string, int, int, int, int, int, int, bool) detailedStats
    {
        get
        {
            Player firstPlayer = turn.currentPlayer;
            string firstPlayerName = (firstPlayer is HumanPlayer) ? ((HumanPlayer) firstPlayer).name : "Computer";
            Player secondPlayer = turn.adversaryPlayer;
            string secondPlayerName = (secondPlayer is HumanPlayer) ? ((HumanPlayer) secondPlayer).name : "Computer";
            string whitePlayer = (firstPlayer.color == PlayerColor.White) ? firstPlayerName : secondPlayerName;
            string blackPlayer = (firstPlayer.color == PlayerColor.Black) ? firstPlayerName : secondPlayerName;
            BoardCell[] currentLayout = (BoardCell[]) layout.Clone();
            bool[] currentDamaLayout = (bool[]) damaLayout.Clone();
            int whitePieceCount = 0;
            int whiteDamaCount = 0;
            int whiteScore = (firstPlayer.color == PlayerColor.White) ? firstPlayer.score : secondPlayer.score;
            int blackPieceCount = 0;
            int blackDamaCount = 0;
            int blackScore = (firstPlayer.color == PlayerColor.Black) ? firstPlayer.score : secondPlayer.score;
            for(int i = 0; i < layout.Length; i++)
            {
                if (layout[i].isValid && i < layout.Length / 2)
                {
                    whitePieceCount++;
                    if (damaLayout[i]) whiteDamaCount++;
                }
                if (layout[i].isValid && i >= layout.Length / 2)
                {
                    blackPieceCount++;
                    if (damaLayout[i]) blackDamaCount++;
                }
            }
            bool isConcluded = (conclusion != (PlayerColor)(-1));
            return (selectedSlot, whitePlayer, blackPlayer, whitePieceCount, whiteDamaCount, whiteScore, blackPieceCount, blackDamaCount, blackDamaCount, isConcluded);
        }
    }

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
        _status = GameStatus.Uninitialized;
    }
    public void LoadGame(int slot)
    {
        if (PersistanceManager.SlotContainsData(slot))
        {
            selectedSlot = slot;
            var loadedGame = PersistanceManager.GetSlot(slot);
            SetupBoard(loadedGame.Item11);
            damaLayout = loadedGame.Item12;
            selectedPedina = null;
            forcedSelection = null;
            Player whitePlayer = (loadedGame.Item4) ? (Player) new HumanPlayer(PlayerColor.White, loadedGame.Item2) : (Player) new ComputerPlayer(PlayerColor.White, loadedGame.Item8);
            Player blackPlayer = (loadedGame.Item5) ? (Player) new HumanPlayer(PlayerColor.Black, loadedGame.Item3) : (Player) new ComputerPlayer(PlayerColor.Black, loadedGame.Item8);
            // If game is not over set up turns
            if (!loadedGame.Item9 && loadedGame.Item10 == PlayerColor.White)       
                turn = new BoardTurn(whitePlayer, blackPlayer, loadedGame.Item11, loadedGame.Item12);
            else if (!loadedGame.Item9 && loadedGame.Item10 == PlayerColor.Black)       
                turn = new BoardTurn(blackPlayer, whitePlayer, loadedGame.Item11, loadedGame.Item12);
            // If game is done then set winner
            if (loadedGame.Item9)
                conclusion = loadedGame.Item10;
            else
                conclusion = (PlayerColor) (-1);
            UXManager.Shared.UpdateStatus(shortStats);
            StartCoroutine("TryComputerPlay"); // Otherwise an AI vs. AI game gets stuck at start
        }
        else
        {
            selectedSlot = 0;
            UXManager.Shared.AbortGameLoad();
        }
    }
    public void NewGame(int slot, Player whitePlayer, Player blackPlayer)
    {
        selectedSlot = slot;
        SetupBoard();
        selectedPedina = null;
        forcedSelection = null;
        turn = new BoardTurn(whitePlayer, blackPlayer, layout, damaLayout);
        conclusion = (PlayerColor) (-1);
        UXManager.Shared.UpdateStatus(shortStats);
        StartCoroutine("TryComputerPlay"); // Otherwise an AI vs. AI game gets stuck at start
    }
    // TODO: LoadGame & SaveGame
    public void QuitGame()
    {
        UnsetBoard();
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
        _status = GameStatus.Running;
    }
    void UnsetBoard()
    {
        pedinaPoolReady = false;
        for (int i = 0; i < pedinaPool.Length; i++)
        {
            Destroy(pedinaPool[i]);
        }
        _status = GameStatus.Uninitialized;
    }

    public void Clicked(RaycastHit target)
    {
        if (conclusion != (PlayerColor)(-1))
        {
            SoundManager.Shared.Beep();
            return;
        }
        if(turn.currentPlayer is ComputerPlayer computerPlayer)
        {
            SoundManager.Shared.Beep();
            return;
        }
        if (target.collider.name == "Tavolo da Gioco")
        {
            Vector3 relativePosition = target.transform.InverseTransformPoint(target.point);
            BoardCell hit = new BoardCell(relativePosition);
            RenderMove(hit);
        }
        else if(target.collider.name == "Pedina Normale" || target.collider.name == "Dama")
        {
            GameObject pedina = target.collider.transform.parent.gameObject;
            // Validate Selection
            if (forcedSelection == null && (pedina.GetComponent<Pedina>().cell.GetOwner(layout) == turn.currentPlayer.color))
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
    public void RenderMove(BoardCell selection, BoardCell hit)
    {
        selectedPedina = GameManager.Shared.GetInstanceFromIndex(selection.GetIndex(layout));
        RenderMove(hit);
    }
    public void RenderMove(BoardCell hit)
    {
        if (hit.cell != null)
        {
            (bool hasMoved, bool hasCaptured, bool mustChainCapture) = MoveAction(hit);
            // React to move
            if (!hasMoved)
                SoundManager.Shared.Beep();
            else {
                if (mustChainCapture)
                {
                    forcedSelection = selectedPedina;
                    StartCoroutine("TryComputerPlay");
                }
                else {
                    forcedSelection = null;
                    bool rotateCamera = turn.adversaryPlayer is HumanPlayer;
                    (BoardTurn nextTurn, Player winner) = turn.NextTurn(rotateCamera);
                    turn = nextTurn;
                    conclusion = (winner != null) ? winner.color : (PlayerColor)(-1);
                    UXManager.Shared.UpdateStatus(shortStats);
                    if (conclusion == (PlayerColor)(-1)) StartCoroutine("TryComputerPlay");
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
                (bool turnNotOver, bool hasMoved, bool hasCaptured, bool hasGraduated) = turn.Procede(move);
                // Move
                selectedPedina.GetComponent<Pedina>().cell = hit;
                // Check if dama
                if (!isDama)
                {
                    isDama = (turn.currentPlayer.color == PlayerColor.White) ? hit.indices.Item2 == 7 : hit.indices.Item2 == 0;
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

    IEnumerator TryComputerPlay()
    {
        if(turn.currentPlayer is ComputerPlayer computerPlayer) {
            DateTime startOfPlay = DateTime.Now;
            // Check Move in Background
            Task<BoardMove> evaluationHandle = Task<BoardMove>.Factory.StartNew(() => computerPlayer.ChooseMove(turn));
            while (!evaluationHandle.IsCompleted) yield return null;
            // Prepare to Play Move
            if (evaluationHandle.Result != null)
            {
                DateTime endOfPlay = DateTime.Now;
                double elapsedTime = (endOfPlay - startOfPlay).TotalSeconds;
                float waitTime = 2f - ((float) elapsedTime);
                waitTime = (waitTime > 0) ? waitTime : 0;
                yield return new WaitForSeconds(waitTime);
                // Play Move
                //Debug.Log("Computer thinks that " + evaluationHandle.Result.ToString() + " is its best move");
                computerPlayer.PlayMove(evaluationHandle.Result);
            } else Debug.Log("Computer is stuck");
        }
    }
    IEnumerator TryChainComputerPlay()
    {
        if(turn.currentPlayer is ComputerPlayer computerPlayer) {
            yield return new WaitForSeconds(1.1f);
            computerPlayer.PlayMove(turn.moves[0]);
        }
    }
}
