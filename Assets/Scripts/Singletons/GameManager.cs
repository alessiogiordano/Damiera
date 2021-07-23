using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameBoard;
    private GameObject[] positionPool = new GameObject[33]; // Container + 32 Positions
    [SerializeField] private GameObject pedinaPrefab;
    [SerializeField] private Material pedinaBianca;
    [SerializeField] private Material pedinaNera;
    private GameObject[] pedinaPool = new GameObject[25]; // Container + 24 Items

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
        // SetupValidPositions();
        SetupBoard();
    }

    void SetupValidPositions()
    {
        // Clear if not empty
        for (int i = 0; i < positionPool.Length; i++)
        {
            Destroy(positionPool[i]);
        }
        // Account for human error in designing the Texture
        const float centerCorrection = 0.9990662932f;
        const float lengthCorrection = 1.0209039548f;
        // Setup Root Object
        positionPool[0] = new GameObject("Posizioni Valide");
        positionPool[0].transform.parent = gameBoard.transform;
        const float rootOffset = (0.272f / 2) * (1.0f - centerCorrection);
        positionPool[0].transform.localPosition = new Vector3(rootOffset, 0.0f, -rootOffset);
        GameObject root = positionPool[0];
        // Values
        const float cellDimension = 0.03f * lengthCorrection;
        const float startingPoint = cellDimension * 3.5f;
        const float horizontalOffset = cellDimension * 2;
        // Instantiate
        for (int i = 0; i < 8; i++)
        {
            string[] columns = (i % 2 == 0) ? new string[] { "A", "C", "E", "G" } : new string[] { "B", "D", "F", "H" };
            float startingX = startingPoint;
            float startingZ = startingPoint - (cellDimension * i);
            float oddRowOffset = (i % 2 == 0) ? 0 : cellDimension;
            for (int j = 0; j < 4; j++)
            {
                float cellOffset = startingX - oddRowOffset - (horizontalOffset * j);
                positionPool[i+1] = new GameObject(columns[j] + (i + 1));
                positionPool[i+1].transform.parent = root.transform;
                positionPool[i+1].transform.localPosition = new Vector3(cellOffset, 0.0f, startingZ);
            }
        }
    }

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
            bool isWhite = i < 12;
            pedinaPool[i] = Instantiate(pedinaPrefab);
            pedinaPool[i].name = (isWhite) ? "Pedina Bianca n°" + (i + 1) :  "Pedina Nera n°" + (i - 11);
            pedinaPool[i].transform.parent = root.transform;
            pedinaPool[i].transform.localPosition = layout[i].localPosition;
            pedinaPool[i].transform.GetChild(0).GetComponent<Renderer>().material = (isWhite) ? pedinaBianca : pedinaNera;
            pedinaPool[i].transform.GetChild(1).GetComponent<Renderer>().material = (isWhite) ? pedinaBianca : pedinaNera;
            pedinaPool[i].SetActive(layout[i].isActive);
        }
        /*
        // Clear if not empty
        for (int i = 0; i < pedinaPool.Length; i++)
        {
            Destroy(pedinaPool[i]);
        }
        // Instantiate
        for (int i = 0; i < 24; i++)
        {
            pedinaPool[i] = Instantiate(pedinaPrefab);
            pedinaPool[i].transform.parent = gameBoard.transform;
        }
        // Set position and ownership // 0.107
        float[] startingPositions = new float[] {0.107f, 0.077f, 0.047f, -0.107f, -0.077f, -0.047f};
        const float spacer = 0.0613f;
        for (int i = 0; i < 24; i++)
        {
            int rowNumber = i / 4;
            float startingPosition = startingPositions[rowNumber];
            float thirdRowOffset = (rowNumber == 2) ? spacer : (rowNumber == 5) ? -spacer : 0.0f;
            int direction = (rowNumber < 3) ? -1 : 1;
            int offset = i < 4 ? i : (i - 4 * rowNumber);
            pedinaPool[i].transform.localPosition = new Vector3(startingPosition + (offset * spacer * direction) + thirdRowOffset, 0.0f, startingPosition);
            pedinaPool[i].transform.GetChild(0).GetComponent<Renderer>().material = (rowNumber < 3) ? pedinaBianca : pedinaNera;
            pedinaPool[i].transform.GetChild(1).GetComponent<Renderer>().material = (rowNumber < 3) ? pedinaBianca : pedinaNera;
            pedinaPool[i].SetActive(true);
        }
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
