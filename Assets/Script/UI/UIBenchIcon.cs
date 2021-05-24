using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBenchIcon : MonoBehaviour
{
    public Image icon;
    public GameObject star;
    public GameObject slot;
    public Text mutiply;
    public Transform pawnsonfield;
    int numberofpawn = 0;
    int level = 0;
    public int MyID;
    public List<GameObject> pawnslist = new List<GameObject>();
    UIBench benchRef;
    EntitiesDatabaseSO.EntityData myData;

    public void Setup(EntitiesDatabaseSO.EntityData myData, UIBench benchRefn, int pawn)
    {
        this.slot.SetActive(true);
        icon.sprite = myData.bench_icon;
        this.myData = myData;
        this.benchRef = benchRefn;
        numberofpawn = pawn;
        MyID = myData.ID;
        star.transform.GetChild(level).gameObject.SetActive(true);
    }
    public int countthepawn() 
    {
        int result = 0;
        pawnslist.Clear();
        for (int i = 0; i < pawnsonfield.childCount; i++) 
        {
            if (pawnsonfield.GetChild(i).GetComponent<BaseEntity>().getID() == myData.ID
                && pawnsonfield.GetChild(i).GetComponent<BaseEntity>().level == level + 1) 
            {
                pawnslist.Add(pawnsonfield.GetChild(i).gameObject);
                result+=1;
            }
        }
        return result;
    }
    public int getlevel()
    {
        return level;
    }
    public void setlevel(int level) 
    {
        this.level = level;
    }
    public EntitiesDatabaseSO.EntityData getmyData()
    {
        return myData;
    }

    public void setmyData(EntitiesDatabaseSO.EntityData myData) 
    {
        this.myData = myData;
    }
    public int getNumberOfPawn() 
    {
        return numberofpawn;
    }

    public void setNumberOfPawn(int pawn) 
    {
        numberofpawn = pawn;
    }

    public void NumberOfPawn() 
    {
        if (!mutiply.gameObject.activeSelf) 
        {
            mutiply.gameObject.SetActive(true);          
        }
        numberofpawn += 1;
        mutiply.text = "x" + numberofpawn.ToString();
        benchRef.AddPawns();
    }

    public void UpdateNumberOfPawn() 
    {
        if (!this.mutiply.gameObject.activeSelf)
        {
            this.mutiply.gameObject.SetActive(true);
        }
        if (numberofpawn == 1)
        {
            mutiply.text = "";
        }
        else 
        {
            mutiply.text = "x" + numberofpawn.ToString();
        }

    }
    public void OnClick() 
    {
        //generate a pawn when in prep mode
        if(!Gamemanager.Instance.getRoundStart())
            Gamemanager.Instance.OnEntitySpawn(myData, this);
    }

	public void iconupdate() 
    {
        numberofpawn -= 1;
        if (numberofpawn == 1)
        {
            mutiply.text = "";
        }
        else if (numberofpawn == 0)
        {
            slot.SetActive(false);
            numberofpawn = 0;
            star.transform.GetChild(level).gameObject.SetActive(false);
            level = 0;
            star.transform.GetChild(level).gameObject.SetActive(true);
            MyID = 0;
            //update bench
            benchRef.UpdateBench();
        }
        else
        {
            mutiply.text = "x" + numberofpawn.ToString();
        }
    }
}
