using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gamemanager : Manager<Gamemanager>
{
    public EntitiesDatabaseSO entitiesDatabase;
    public List<BaseEntity> allPrepab;
    public Transform team1Parent;
    public Transform team2Parent;
    public Transform team1tmpParent;
    public GameObject Teams;
    public UIBench uIBench;
    List<BaseEntity> team1Entities = new List<BaseEntity>();
    List<BaseEntity> team1tmpEntities = new List<BaseEntity>();
    List<BaseEntity> team2Entities = new List<BaseEntity>();
    public int playerunits = 5;
    UIBenchIcon icononhold = null;
    public int currentUnitsOnBoard = 0;
    bool roundStart = false;

    public Action OnRoundStart;
    public Action OnRoundEnd;
    public Action<BaseEntity> OnUnitDied;


    public bool getRoundStart() 
    {
        return roundStart;
    }
    public List<BaseEntity> getTeam1() 
    {
        return team1tmpEntities;
    }

    public List<BaseEntity> GetEntitiesAgainst(Team against)
    {
        if (against == Team.Team1)
            return team2Entities;
        else
            return team1Entities;
    }
	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.D))
        {
            DebugFight();
        }

        if ((!team1Entities.Any() || !team2Entities.Any()) && roundStart) 
        {
            //reset the board
            roundStart = false;
            if (team2Entities.Any()) 
            {
                //remove all enemy
                removeEnemy();
            }
            reset_map();
            //need to save pos of the player pawn
        }
	}
    private void removeEnemy() 
    {
        while (team2Entities.Any()) 
        {
            BaseEntity tmp = team2Entities[0];
            team2Entities.Remove(tmp);
            tmp.CurrentNode.SetOccupied(false);
            Destroy(tmp.gameObject);
        }
    }

    private void reset_map() 
    {
        List<Tile> allTiles = Gridmanager.Instance.getTilesList();
        for (int i = 0; i < allTiles.Count(); i++) 
        {
            Node tmp = Gridmanager.Instance.GetNodeForTile(allTiles[i]);
            if (tmp.IsOccupied) 
            {
                tmp.SetOccupied(false);
            }
        }
        Teams.transform.GetChild(2).gameObject.SetActive(true);
        for (int i = 0; i < team1tmpParent.childCount; i++)
        {
            Node tmpNode = team1tmpParent.GetChild(i).GetComponent<BaseEntity>().CurrentNode;
            tmpNode.SetOccupied(true);
        }
    }

    public void OnEntitySpawn(EntitiesDatabaseSO.EntityData entityData, UIBenchIcon icon) 
    {
        if (currentUnitsOnBoard < playerunits)
        {
            BaseEntity newEntity = Instantiate(entityData.prefab, team1tmpParent);
            newEntity.gameObject.name = entityData.name;
            newEntity.level = icon.getlevel() + 1;
            newEntity.setID(entityData.ID);
            team1tmpEntities.Add(newEntity);
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            Vector3 adjustZ = new Vector3(wp.x, wp.y, 0);
            this.icononhold = icon;
            newEntity.Setup(Team.Team1tmp, null);
            newEntity.transform.position = adjustZ;
            
            currentUnitsOnBoard++;
            int pawnonbench = uIBench.getSlotsUsed() - 1;
            uIBench.setSlotsUsed(pawnonbench);
        }
        else 
        {
            //need a msg here
            Debug.Log("Can't add more pawn to the field");
        }
    }

    public void releaseicon() 
    {
        icononhold.iconupdate();
        icononhold = null;
    }
    public void UnitDead(BaseEntity entity)
    {
        //need change don't want to delete the gameobject but instead have death animation and then set active = false. When the fight is over just respawn it
        //in the spawn point of the pawn (which haven't impliment yet)
        team1Entities.Remove(entity);
        team2Entities.Remove(entity);

        OnUnitDied?.Invoke(entity);

        Destroy(entity.gameObject);
    }

    public void DebugFight()
    {
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, entitiesDatabase.allEntities.Count);
            BaseEntity newEntity = Instantiate(entitiesDatabase.allEntities[randomIndex].prefab, team2Parent);
            newEntity.gameObject.GetComponent<Draggable>().enabled = false;
            team2Entities.Add(newEntity);

            newEntity.Setup(Team.Team2, Gridmanager.Instance.GetFreeNode(Team.Team2));
        }
    }

    public void SetRoundStart() 
    {
        for (int i = 0; i < team1tmpParent.childCount; i++) 
        {
            BaseEntity newEntity = Instantiate(entitiesDatabase.allEntities[team1tmpParent.GetChild(i).GetComponent<BaseEntity>().getID()].prefab, team1Parent);
            Node tmpNode = team1tmpParent.GetChild(i).GetComponent<BaseEntity>().CurrentNode;
            newEntity.level = team1tmpParent.GetChild(i).GetComponent<BaseEntity>().level;
            team1Entities.Add(newEntity);
            tmpNode.SetOccupied(false);
            newEntity.Setup(Team.Team1, tmpNode);
            newEntity.gameObject.GetComponent<Draggable>().enabled = false;
        }
        Teams.transform.GetChild(2).gameObject.SetActive(false);
        roundStart = true;
    }
}
public enum Team
{
    Team1,
    Team2,
    Team1tmp
}