using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    public int StartMana = 1;
    public int MaxMana = 10;
    public float Duration = 2;

    private GameObject map;
    protected virtual void AffectedArea() { }

    protected virtual void Skill() { }

    protected virtual void Passive() { }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        map = Gridmanager.Instance.terrainGrid;
    }

    public void UseSkill()
    {
        AffectedArea();
        Skill();
        Passive();
    }
}
