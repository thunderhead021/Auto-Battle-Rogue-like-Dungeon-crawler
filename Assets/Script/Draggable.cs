using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerClickHandler
{
    public LayerMask releaseMask;
    public LayerMask PawmMask;
    private GameObject MsgBox;
    private GameObject Map;
    public Vector3 dragOffset = new Vector3(0, -0.4f, 0);
    
    private bool MsgBoxOpened;
    private Camera cam;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col;
    public Vector3 colsize;

    private Vector3 oldPosition;
    private int oldSortingOrder;
    private Tile previousTile = null;
    private int PawnPos;

    public bool IsDragging = false;
    bool firstime = true;

    public void set_firstime(bool firstime) 
    {
        this.firstime = firstime;
    }

    private void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsDragging = true;
        col = GetComponent<BoxCollider2D>();
        colsize = col.bounds.size;
        MsgBox = GameObject.FindGameObjectWithTag("MsgBox");
        Map = GameObject.FindGameObjectWithTag("Map");
    }
    private void Update()
    {
        if (firstime)
        {
            OnStartDrag();
            OnDragging();
        }
        if (Input.GetMouseButtonUp(0) && firstime)
        {
            OnEndDrag(); 
            firstime = false;
        }
        RaycastHit2D hit =
            Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, PawmMask);
        if (hit.collider == null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && MsgBox.transform.GetChild(0).gameObject.activeSelf)
        {
            MsgBox.transform.GetChild(0).gameObject.SetActive(false);
            MsgBox.GetComponent<MsgBox>().BoxReset();
        }
    }
    public void OnStartDrag()
    {
        //Debug.Log(this.name + " start drag");
        if (MsgBox.transform.GetChild(0).gameObject.activeSelf)
        {
            MsgBox.transform.GetChild(0).gameObject.SetActive(false);
            MsgBox.GetComponent<MsgBox>().BoxReset();
        }
        oldPosition = this.transform.position;
        oldSortingOrder = spriteRenderer.sortingOrder;

        spriteRenderer.sortingOrder = 20;
        IsDragging = true;
    }

    public void OnDragging()
    {
        if (!IsDragging)
            return;

        //Debug.Log(this.name + " dragging");

        Vector3 newPosition = cam.ScreenToWorldPoint(Input.mousePosition) + dragOffset;
        newPosition.z = 0;
        this.transform.position = newPosition;

        Tile tileUnder = GetTileUnder();
        if (tileUnder != null && !tileUnder.EnemyTile)
        {
            tileUnder.SetHighlight(true, !Gridmanager.Instance.GetNodeForTile(tileUnder).IsOccupied);

            if (previousTile != null && tileUnder != previousTile)
            {
                //We are over a different tile.
                previousTile.SetHighlight(false, false);
            }

            previousTile = tileUnder;
        }
        else if (tileUnder != null && tileUnder.EnemyTile) 
        {
            tileUnder.SetHighlight(true, false);


            if (previousTile != null && tileUnder != previousTile)
            {
                //We are over a different tile.
                previousTile.SetHighlight(false, false);
            }

            previousTile = tileUnder;
        }
    }

    public void OnEndDrag()
    {
        /*if (!IsDragging)
            return;*/
 
        BaseEntity thisEntity = GetComponent<BaseEntity>();
        if (!TryRelease())
        {

            //need to check if the pawn oldPos is under a tile or not. If not => it's from the bench and need to be return
            if (thisEntity.CurrentNode == null)
            {
                Gamemanager.Instance.getTeam1().Remove(this.GetComponent<BaseEntity>());
                Gamemanager.Instance.currentUnitsOnBoard--;
                Destroy(this.gameObject);
            }

            //Nothing was found, return to original position.
            this.transform.position = oldPosition;
        }
        else if (firstime) 
        {
            Gamemanager.Instance.releaseicon();
        }

        if (previousTile != null)
        {
            previousTile.SetHighlight(false, false);
            previousTile = null;
        }
        if (thisEntity.CurrentNode != null)
        {
            Tile t = GetTileUnder();
            spriteRenderer.sortingOrder = t.SortNumber;
        }
        IsDragging = false;

    }

    private bool TryRelease()
    {
        //Released over something!
        Tile t = GetTileUnder();
        
        if (t != null && !t.EnemyTile)
        {
            //It's a tile!
            BaseEntity thisEntity = GetComponent<BaseEntity>();
            Node candidateNode = Gridmanager.Instance.GetNodeForTile(t);
            if (candidateNode != null && thisEntity != null)
            {
                if (!candidateNode.IsOccupied)
                {
                    //Let's move this pawn to that node
                    if (thisEntity.CurrentNode != null) 
                    {
                        thisEntity.CurrentNode.SetOccupied(false);
                    }
                    thisEntity.SetCurrentNode(candidateNode);
                    candidateNode.SetOccupied(true);
                    thisEntity.transform.position = candidateNode.worldPosition;
                    Debug.Log(thisEntity.transform.position);
                    PawnPos = Gridmanager.Instance.GetNumberForTile(t);
                    return true;
                }
            }
        }
        return false;
    }

    public Tile GetTileUnder()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100, releaseMask);

        if (hit.collider != null)
        {
            //Released over something!
            Tile t = hit.collider.GetComponent<Tile>();
            return t;
        }

        return null;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        float x, y;
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log(this.gameObject.GetComponent<BaseEntity>().getID() + "something to see");
            if (MsgBox.transform.GetChild(0).gameObject.activeSelf)
            {
                MsgBox.transform.GetChild(0).gameObject.SetActive(false);
                MsgBox.GetComponent<MsgBox>().BoxReset();
            }
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int startPos = PawnPos / 9;

            if (PawnPos > startPos * 9 + 4)
            {
                x = transform.position.x - colsize.x * 2.6f;
            }
            else
            {
                x = transform.position.x + colsize.x * 2.6f;
            }
            if (PawnPos >= 36)
            {
                y = transform.position.y - colsize.y;
            }
            else 
            {
                y = transform.position.y + colsize.y;
            }
            Vector2 MsgBoxPos = new Vector2(x, y);
            Vector2 pos = cam.WorldToScreenPoint(MsgBoxPos);
            //Debug.Log(worldPosition.y + " - " + (colsize.y + colsize.y/2));
            MsgBox.transform.position = pos;
            if (!MsgBox.transform.GetChild(0).gameObject.activeSelf)
            {
                MsgBox.GetComponent<MsgBox>().SetUp(this.gameObject.GetComponent<BaseEntity>());
                MsgBox.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
