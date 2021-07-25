using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedina : MonoBehaviour
{
    private bool _isSetUp = false;
    private PedinaAnimation _animation;

    public PlayerColor color;
    public bool dama
    {
        get
        {
            return this.gameObject.transform.GetChild(1).gameObject.activeSelf;
        }
        set
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(value);
        }
    }
    public bool selected
    {
        get
        {
            return this.gameObject.transform.GetChild(2).gameObject.activeSelf;
        }
        set
        {
            this.gameObject.transform.GetChild(2).gameObject.SetActive(value);
        }
    }
    public BoardCell cell
    {
        get
        {
            return new BoardCell(this.gameObject.transform.localPosition);
        }
        set
        {
            _animation = new PedinaAnimation(cell, value);
            StartCoroutine("Slide");
        }
    }
    public bool ToggleSelection()
    {
        selected = !selected;
        return selected;
    }

    public void Setup(int index, GameObject root, BoardCell cell, PlayerColor color, Material whiteMaterial, Material blackMaterial)
    {
        if (_isSetUp) return;
        bool isWhite = (color == PlayerColor.White);
        this.gameObject.name = (isWhite) ? "Pedina Bianca n°" + (index + 1) :  "Pedina Nera n°" + (index - 11);
        this.gameObject.transform.parent = root.transform;
        this.gameObject.transform.localPosition = cell.localPosition;
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = (isWhite) ? whiteMaterial : blackMaterial;
        this.gameObject.transform.GetChild(1).GetComponent<Renderer>().material = (isWhite) ? whiteMaterial : blackMaterial;
        this.gameObject.SetActive(cell.isValid);
    }

    IEnumerator Slide()
    {
        while (_animation.running)
        {
            this.gameObject.transform.localPosition = _animation.frame;
            yield return null;
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
