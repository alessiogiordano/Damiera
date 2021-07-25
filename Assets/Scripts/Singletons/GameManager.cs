using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameBoard;
    [SerializeField] private GameObject pedinaPrefab;
    [SerializeField] private Material pedinaBianca;
    [SerializeField] private Material pedinaNera;
    private GameObject[] pedinaPool = new GameObject[25]; // Container + 24 Items
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
    private bool pedinaPoolReady = false;
    private GameObject selectedPedina;

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
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
        SetupBoard(); // Debug Setup
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
        if (target.collider.name == "Tavolo da Gioco")
        {
            Vector3 relativePosition = target.transform.InverseTransformPoint(target.point);
            BoardCell hit = new BoardCell(relativePosition);
            if (hit.cell != null)
            {
                // Do Something
                if (selectedPedina != null)
                {
                    // Validate Move and do something with it
                    if (layout.AvailableDestinations(selectedPedina.GetComponent<Pedina>().cell).Contains(hit))
                        selectedPedina.GetComponent<Pedina>().cell = hit;
                    // Deselect
                    selectedPedina.GetComponent<Pedina>().selected = false;
                    selectedPedina = null;
                }
            }
        }
        else if(target.collider.name == "Pedina Normale" || target.collider.name == "Dama")
        {
            GameObject pedina = target.collider.transform.parent.gameObject;
            // Validate if item can be selected
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
        }
    }
}
