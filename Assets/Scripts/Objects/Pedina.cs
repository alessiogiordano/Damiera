using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedina : MonoBehaviour
{
    private bool _isSetUp = false;
    private PedinaAnimation _slideAnimation;
    private float _fadeAnimation = 0.4f;

    public PlayerColor color;
    public bool dama
    {
        get
        {
            return this.gameObject.transform.GetChild(1).gameObject.activeSelf;
        }
        set
        {
            if (dama != value) StartCoroutine("AnimatedToggleDama");
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
            _slideAnimation = new PedinaAnimation(cell, value);
            StartCoroutine("Slide");
        }
    }
    public bool eaten
    {
        get
        {
            return cell.isValid;
        }
        set
        {
            if(value)
            {
                StartCoroutine("AnimatedSetEaten");
            }
            else
            {
                Debug.Log("An eaten cell cannot be brought back to gameplay");
            }
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
        while (_slideAnimation.running)
        {
            this.gameObject.transform.localPosition = _slideAnimation.frame;
            yield return null;
        }
    }

    IEnumerator AnimatedToggleDama()
    {
        float progress = 0.0f;
        while (progress < 1f)
        {
            progress += Time.fixedDeltaTime / _fadeAnimation;
            Color newColor = this.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color;
            newColor.a = (dama == true) ? (1f - progress) : progress;
            this.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
        this.gameObject.transform.GetChild(1).gameObject.SetActive(!dama);
    }
    IEnumerator AnimatedSetEaten()
    {
        float progress = 0.0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime / _fadeAnimation;
            if (progress > 1.0f) progress = 1f;
            if (dama)
            {
                Color damaColor = this.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color;
                damaColor.a = 1f - progress;
                this.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = damaColor;
            }
            Color pedinaColor = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color;
            pedinaColor.a = 1f - progress;
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = pedinaColor;
            yield return null;
        }
        this.gameObject.SetActive(false);
        this.gameObject.transform.localPosition = BoardCell.invalidCell.localPosition;
    }
}
