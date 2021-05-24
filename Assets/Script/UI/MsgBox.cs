using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox : MonoBehaviour
{
    private EntitiesDatabaseSO cachedDb;
    public Image Icon;
    public GameObject star;
    public Text Name;
    public Text HP;
    public Text Atk;
    public Text Def;
    public Text AtkSpd;
    public Text MS;
    public Image OriginIcon;
    public Text Origin;
    public Image ClassIcon;
    public Text Class;
    public Image Skill1;
    public Image Skill2;
    public Image Skill3;
    public Image Item1;
    public Image Item2;
    public Image Item3;

    private void Start()
    {
        cachedDb = Gamemanager.Instance.entitiesDatabase;
    }

    public void SetUp(BaseEntity Pawn) 
    {
        Name.text = Pawn.name;
        HP.text = Pawn.baseHealth.ToString();
        Atk.text = Pawn.baseDamage.ToString();
        Def.text = "not yet";
        AtkSpd.text = Pawn.attackSpeed.ToString();
        MS.text = Pawn.movementSpeed.ToString();
        star.transform.GetChild(Pawn.level - 1).gameObject.SetActive(true);
    }

	public void BoxReset()
	{
        Name.text = " ";
        HP.text = " ";
        Atk.text = "";
        Def.text = "";
        AtkSpd.text = "";
        MS.text = "";
        Origin.text = "";
        Class.text = "";
        for (int i = 0; i < 3; i++) 
        {
            if(star.transform.GetChild(i).gameObject.activeSelf)
                star.transform.GetChild(i).gameObject.SetActive(false);
        }
	}
}
