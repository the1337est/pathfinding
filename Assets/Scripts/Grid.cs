using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private Transform container;

    private List<Tile> tiles;

    public Color OpenColour;
    public Color ClosedColour;
    public Color TargetColour;
    public Color BlockedColour;
    public Color HighlightColour;

    public static Grid Instance;

    private Tile currentTile;
    private Tile targetTile;

    [SerializeField] ClickMode mode = ClickMode.Edit;
    [SerializeField] private Text modeText;

    [SerializeField] private Text selectedText;
    [SerializeField] private Text distFromTargetText;
    [SerializeField] private Text totalScore;

    bool found = false;
    bool autoplaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CreateGrid();
        UpdateButtonText();
    }

    void CreateGrid()
    {
        tiles = new List<Tile>();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Transform t = Instantiate(tilePrefab, container).transform;
                t.position = new Vector3(i, -0.55f, j);
                t.name = $"Tile ({i},{j})";
                Tile tile = t.GetComponent<Tile>();
                tile.X = i;
                tile.Y = j;
                tile.Open();
                tiles.Add(tile);
            }
        }
        Camera.main.transform.position = new Vector3(x / 2f, 10f, y / 2f);
        SelectTarget();
    }

    void SelectTarget()
    {
        if (targetTile == null)
        {
            int randX = Random.Range(0, x);
            int randY = Random.Range(0, y);
            Tile t = tiles.Find(tile => tile.X == randX && tile.Y == randY);
            targetTile = t;
            targetTile.SetColour(TargetColour);
        }
    }

    public void SelectTile(Tile tile)
    {
        if (tile != null)
        {
            if (currentTile == null)
            {
                currentTile = tile;
                currentTile.Close();
                //currentTile.Highlight(currentTile, targetTile);
            }
            else
            {
                tile.Block();
                return;

            }
            //switch (mode)
            //{
            //    case ClickMode.Edit:

            //        //set tile as obstacle
            //        tile.Block();
            //        return;

            //    case ClickMode.Navigate:


            //        break;
            //    default:
            //        break;
            //}
            
            //if (tile.State == State.Open)
            //{
            //    if (currentTile != null)
            //    {
            //        if (!currentTile.IsNeighbour(tile))
            //        {
            //            return;
            //        }
            //    }

            //    tile.Close();
            //    currentTile = tile;
            //    Debug.Log("Current tile updated: " + tile.name);
            //}
        }
    }


    public void ToggleMode()
    {
        switch (mode)
        {
            case ClickMode.Edit:
                mode = ClickMode.Navigate;
                break;
            case ClickMode.Navigate:
                mode = ClickMode.Edit;
                break;
        }
        UpdateButtonText();
    }

    public void CheckTile(Tile tile)
    {
        if (currentTile == null || tile == null)
        {
            return;
        }
        if (currentTile == tile)
        {
            return;
        }
        if (currentTile.IsNeighbour(tile))
        {
            //set colour to checkColour
            //display navigation info in UI
        }
    }

    private void UpdateButtonText()
    {
        modeText.text = mode == ClickMode.Edit ? "Click Mode: Edit" : "Click Mode: Navigate";
    }

    public void NextMove()
    {
        FindNextMove();
    }

    public bool FindNextMove()
    {
        if (currentTile == null)
        {
            Debug.Log("[Error] Unable to make a move, select starting tile first");
            return false;
        }
        //find unblocked neighbour tiles
        List<Tile> neighbours = tiles.FindAll(t => t.State == State.Open && currentTile.IsNeighbour(t));
        if (neighbours != null && neighbours.Count > 0)
        {
            Tile best = neighbours[0];
            for (int i = 0; i < neighbours.Count; i++)
            {
                Tile t = neighbours[i];
                t.Highlight(currentTile, targetTile);
                if (t.FCost < best.FCost)
                {
                    best = t;
                }
            }
            if (best == currentTile)
            {
                return false;
            }
            currentTile = best;
            currentTile.Close();

            if (currentTile == targetTile)
            {
                found = true;
            }
            return true;
        }
        return false;
    }

    public void Autoplay()
    {
        if (!autoplaying)
        {
            StartCoroutine(AutoplayRoutine());
        }
    }

    IEnumerator AutoplayRoutine()
    {
        int moves = 0;
        autoplaying = true;
        while (!found)
        {
            if (FindNextMove())
            {
                moves++;
                Debug.Log("Moves: " + moves);
            }
            else
            {
                Debug.Log("Unable to find a solution currently.");
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    enum ClickMode
    {
        Edit,
        Navigate
    }
}