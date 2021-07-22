using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameBoard;
    [SerializeField] private GameObject pedinaPrefab;
    [SerializeField] private Material pedinaBianca;
    [SerializeField] private Material pedinaNera;
    private GameObject[] pedinaPool = new GameObject[24];

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
        SetupBoard();
    }

    void SetupBoard() {
        // Clear if not empty
        for (int i = 0; i < pedinaPool.Length; i++)
        {
            Destroy(pedinaPool[0]);
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
