using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBench : MonoBehaviour
{
    public List<UIBenchIcon> allIcons;
    int SlotsUsed = 0;
    public int getSlotsUsed() 
    {
        return SlotsUsed;
    }
    public void setSlotsUsed(int newNumber) 
    {
        SlotsUsed = newNumber;
    }
    public void OnPawnBought(EntitiesDatabaseSO.EntityData entityData) 
    {
        for (int i = 0; i < allIcons.Count; i++)
        {
            GameObject tmp = allIcons[i].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
            if (tmp.activeSelf)
            {
                if (allIcons[i].MyID == entityData.ID && allIcons[i].getlevel() + 1 == entityData.prefab.level)
                {
                    allIcons[i].NumberOfPawn();
                    break;
                }
            }
            else if(!tmp.activeSelf)
            {
                allIcons[i].Setup(entityData, this, 1);
                tmp.SetActive(true);
                AddPawns();
                break;
            }
        }
        SlotsUsed++;
    }

    public void UpdateBench() 
    {
        for (int i = 0; i < allIcons.Count - 1; i++)
        {
            GameObject tmp = allIcons[i].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
            GameObject tmp1 = allIcons[i+1].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
            if (!tmp.activeSelf && tmp1.activeSelf) 
            {
                allIcons[i].icon.sprite = allIcons[i + 1].icon.sprite;
                allIcons[i].star.transform.GetChild(allIcons[i].gameObject.GetComponent<UIBenchIcon>().getlevel()).gameObject.SetActive(false);
                allIcons[i].star.transform.GetChild(allIcons[i+1].gameObject.GetComponent<UIBenchIcon>().getlevel()).gameObject.SetActive(true);
                allIcons[i + 1].star.transform.GetChild(allIcons[i + 1].gameObject.GetComponent<UIBenchIcon>().getlevel()).gameObject.SetActive(false);
                allIcons[i + 1].setlevel(0);
                allIcons[i].setNumberOfPawn(allIcons[i + 1].getNumberOfPawn());
                allIcons[i + 1].setNumberOfPawn(1);
                allIcons[i].UpdateNumberOfPawn();
                allIcons[i + 1].UpdateNumberOfPawn();
                allIcons[i].setmyData(allIcons[i + 1].getmyData());
                allIcons[i].MyID = allIcons[i + 1].MyID;
                allIcons[i + 1].MyID = 0;
                tmp.SetActive(true);
                tmp1.SetActive(false);
            } 
        }
    }
    void resetSlot(int place) 
    {
        GameObject tmp = allIcons[place].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
        allIcons[place].star.transform.GetChild(allIcons[place].gameObject.GetComponent<UIBenchIcon>().getlevel()).gameObject.SetActive(false);
        allIcons[place].setlevel(0);
        allIcons[place].setNumberOfPawn(1);
        allIcons[place].UpdateNumberOfPawn();
        allIcons[place].MyID = 0;
        tmp.SetActive(false);
    }

    //check if any slot have the required condition to upgrade, if it's lower than 3 star then this will repeat until there aren't any possible move left
    public void AddPawns() 
    {
        for (int i = 0; i < allIcons.Count; i++) 
        {
            GameObject tmp = allIcons[i].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
            int level = allIcons[i].gameObject.GetComponent<UIBenchIcon>().getlevel();
            int benchcount = allIcons[i].gameObject.GetComponent<UIBenchIcon>().getNumberOfPawn();
            int pawnscount = allIcons[i].gameObject.GetComponent<UIBenchIcon>().countthepawn();
            //Debug.Log(pawnscount);
            if (tmp.activeSelf) 
            {
                if (!Gamemanager.Instance.getRoundStart())
                {
                    if (level < 2 && benchcount + pawnscount == 3)
                    {
                        if (pawnscount != 0)
                        {
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().countthepawn();
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().pawnslist[0].GetComponent<BaseEntity>().level++;
                            switch (pawnscount)
                            {
                                case 1:
                                    resetSlot(i);
                                    SlotsUsed -= 2;
                                    break;
                                case 2:
                                    Destroy(allIcons[i].gameObject.GetComponent<UIBenchIcon>().pawnslist[1]);
                                    resetSlot(i);
                                    SlotsUsed -= 1;
                                    break;
                            }
                            //repeat the process again
                            AddPawns();
                        }
                        else
                        {
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().setNumberOfPawn(1);
                            SlotsUsed -= 2;
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().star.transform.GetChild(level).gameObject.SetActive(false);
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().setlevel(level + 1);
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().star.transform.GetChild(level + 1).gameObject.SetActive(true);
                            allIcons[i].gameObject.GetComponent<UIBenchIcon>().mutiply.text = "";
                            Add_and_Repeat(6);
                        }
                    }
                }
                else 
                {
                    if (level < 2 && benchcount == 3)
                    {
                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().setNumberOfPawn(1);
                        SlotsUsed -= 2;
                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().star.transform.GetChild(level).gameObject.SetActive(false);
                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().setlevel(level + 1);
                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().star.transform.GetChild(level + 1).gameObject.SetActive(true);
                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().mutiply.text = "";
                        Add_and_Repeat(6);
                    }
                }
            }
        }
        //Bench will be update when there aren't any move left
        UpdateBench();
    }

    //repeat the process of adding all the icon with the same lvl to 1 slot
    void Add_and_Repeat(int slot) 
    {
        GameObject tmp = allIcons[slot].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
        if (!tmp.activeSelf)
        {
            if(slot - 1 > 0)
                Add_and_Repeat(slot - 1);
        }
        else 
        {
            for (int i = 0; i < slot; i++) 
            {
                GameObject tmp1 = allIcons[i].gameObject.GetComponent<UIBenchIcon>().slot.gameObject;
                if (tmp1.activeSelf) 
                {
                    if (allIcons[i].gameObject.GetComponent<UIBenchIcon>().MyID == allIcons[slot].gameObject.GetComponent<UIBenchIcon>().MyID
                        && allIcons[i].gameObject.GetComponent<UIBenchIcon>().getlevel() == allIcons[slot].gameObject.GetComponent<UIBenchIcon>().getlevel()) 
                    {
                        int count = allIcons[i].gameObject.GetComponent<UIBenchIcon>().countthepawn() + allIcons[i].gameObject.GetComponent<UIBenchIcon>().getNumberOfPawn()
                            + allIcons[slot].gameObject.GetComponent<UIBenchIcon>().getNumberOfPawn();

                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().setNumberOfPawn(
                                    allIcons[i].gameObject.GetComponent<UIBenchIcon>().getNumberOfPawn() + allIcons[slot].gameObject.GetComponent<UIBenchIcon>().getNumberOfPawn());

                        allIcons[i].gameObject.GetComponent<UIBenchIcon>().UpdateNumberOfPawn();
                        resetSlot(slot);
                        //repeat the process after every change
                        AddPawns();
                    }
                }
            }   
        }
    }
}
