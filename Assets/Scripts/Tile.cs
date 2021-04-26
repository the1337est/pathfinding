using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int GCost;
    public int HCost;
    public int FCost { get => GCost + HCost; }

    [SerializeField] private Text gCost;
    [SerializeField] private Text hCost;
    [SerializeField] private Text fCost;


    [SerializeField] private GameObject canvas;

    public Tile PreviousTile;

    public State State;

    public int X;
    public int Y;

    private void OnMouseDown()
    {
        Grid.Instance.SelectTile(this);
    }

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Grid.Instance.SelectTile(this);
    //    }
    //}

    public void Highlight(Tile current, Tile target)
    {
        GetComponent<Renderer>().material.color = Grid.Instance.HighlightColour;
        if (current != this)
        {
            canvas.SetActive(true);
            CalculateScores(current, target);
        }
    }

    public void Open()
    {
        State = State.Open;
        GetComponent<Renderer>().material.color = Grid.Instance.OpenColour;
    }

    public void Close()
    {
        State = State.Closed;
        GetComponent<Renderer>().material.color = Grid.Instance.ClosedColour;
    }

    public void Block()
    {
        State = State.Blocked;
        GetComponent<Renderer>().material.color = Grid.Instance.BlockedColour;
    }

    public void SetColour(Color c)
    {
        GetComponent<Renderer>().material.color = c;
    }

    public void CalculateScores(Tile current, Tile target)
    {
        if (current == null || target == null)
        {
            return;
        }
        if (current == target)
        {
            return;
        }
        int distToTarget = 0;
        if (X == target.X)
        {
            distToTarget = Mathf.Abs(target.Y - Y) * 10;
        }
        else if(Y == target.Y)
        {
            distToTarget = Mathf.Abs(target.X - X) * 10;
        }
        else
        {
            float x = Mathf.Abs(target.X - X);
            float y = Mathf.Abs(target.Y - Y);
            distToTarget = (int)(Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f)) * 10);
        }
        int distToCurrent = current.X == X || current.Y == Y ? 10 : 14;

        GCost = distToCurrent;
        HCost = distToTarget;

        gCost.text = GCost.ToString();
        hCost.text = HCost.ToString();
        fCost.text = FCost.ToString();
    }

    public bool IsNeighbour(Tile other)
    {
        if (other == null)
        {
            return false;
        }
        if (other == this)
        {
            return false;
        }
        if (Mathf.Abs(other.X - X) <= 1 && Mathf.Abs(other.Y - Y) <= 1)
        {
            return true;
        }
        return false;
    }  
}
public enum State
{
    Open = 0,
    Highlighted,
    Closed,
    Blocked
}

