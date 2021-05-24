using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public HealthBar HealthbarPrefab;
    public ManaBar ManabarPrefab;
    public SpriteRenderer spriteRender;
    //public Animator animator;

    public int baseDamage = 1;
    public int baseHealth = 3;
    public int baseMana = 3;
    public int level;
    int curMana = 0;
    int ID;
    [Range(1, 5)]
    public int range = 1;
    public float attackSpeed = 1f; //Attacks per second
    public float movementSpeed = 1f; //movementSpeed per second

    protected Team myTeam;
    protected BaseEntity currentTarget = null;
    protected Node currentNode;

    public Node CurrentNode => currentNode;

    protected bool HasEnemy => currentTarget != null;
    protected bool IsInRange => currentTarget != null && Vector3.Distance(this.transform.position, currentTarget.transform.position) <= range;
    protected bool moving;
    protected Node destination;
    protected HealthBar healthbar;
    protected ManaBar manabar;

    protected bool dead = false;
    protected bool canAttack = true;
    protected float waitBetweenAttack;

    public List<BaseSkill> Skills = new List<BaseSkill>();

    protected virtual void OnRoundStart() { }
    protected virtual void OnRoundEnd() { }
    protected virtual void OnUnitDied(BaseEntity diedUnity) { }
    // Start is called before the first frame update
    void Start()
    {
        Gamemanager.Instance.OnRoundStart += OnRoundStart;
        Gamemanager.Instance.OnRoundEnd += OnRoundEnd;
        Gamemanager.Instance.OnUnitDied += OnUnitDied;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Team team, Node currentNode)
    {
        myTeam = team;
        if (myTeam == Team.Team2)
        {
            spriteRender.flipX = true;
        }

        this.currentNode = currentNode;
        if (currentNode != null) 
        {
            transform.position = currentNode.worldPosition;
            currentNode.SetOccupied(true);
        }
		healthbar = Instantiate(HealthbarPrefab, this.transform);
		healthbar.Setup(this.transform, baseHealth);

        manabar = Instantiate(ManabarPrefab, this.transform);
        manabar.Setup(this.transform, baseMana);
    }
    public void setID(int id) 
    {
        ID = id;
    }
    public int getID() 
    {
        return ID;
    }
    public void AddPawntoPlayer() 
    {
        //transform.position = ???;

        healthbar = Instantiate(HealthbarPrefab, this.transform);
        healthbar.Setup(this.transform, baseHealth);

        manabar = Instantiate(ManabarPrefab, this.transform);
        manabar.Setup(this.transform, baseMana);
    }

    protected void FindTarget()
    {
        var allEnemies = Gamemanager.Instance.GetEntitiesAgainst(myTeam);
        float minDistance = Mathf.Infinity;
        BaseEntity entity = null;
        foreach (BaseEntity e in allEnemies)
        {
            if (Vector3.Distance(e.transform.position, this.transform.position) <= minDistance)
            {
                minDistance = Vector3.Distance(e.transform.position, this.transform.position);
                entity = e;
            }
        }

        currentTarget = entity;
    }
    protected bool MoveTowards(Node nextNode)
    {
        Vector3 direction = (nextNode.worldPosition - this.transform.position);
        if (direction.sqrMagnitude <= 0.005f)
        {
            transform.position = nextNode.worldPosition;
            //animator.SetBool("walking", false);
            return true;
        }
        //animator.SetBool("walking", true);

        this.transform.position += direction.normalized * movementSpeed * Time.deltaTime;
        return false;
    }
    public void SetCurrentNode(Node node)
    {
        currentNode = node;
    }
    protected void GetInRange()
    {
        if (currentTarget == null)
            return;

        if (!moving)
        {
            destination = null;
            List<Node> candidates = Gridmanager.Instance.GetNodesCloseTo(currentTarget.CurrentNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPosition, this.transform.position)).ToList();
            for (int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    destination = candidates[i];
                    break;
                }
            }
            if (destination == null)
                return;

            var path = Gridmanager.Instance.GetPath(currentNode, destination);
            if (path == null && path.Count >= 1)
                return;

            if (path[1].IsOccupied)
                return;

            path[1].SetOccupied(true);
            destination = path[1];
        }

        moving = !MoveTowards(destination);
        if (!moving)
        {
            //Free previous node
            currentNode.SetOccupied(false);
            SetCurrentNode(destination);
        }
    }

    public void TakeDamage(int amount)
    {
        baseHealth -= amount;
        if (baseHealth < 0)
        {
            baseHealth = 0;
        }
        healthbar.UpdateBar(baseHealth);
        curMana += 1;
        if (curMana > baseMana)
        {
            curMana = baseMana;
        }
        if (!dead) 
        {
            if (baseHealth == 0)
            {
                dead = true;
                currentNode.SetOccupied(false);
                Gamemanager.Instance.UnitDead(this);
            }
            else if (curMana == baseMana)
            {
                //active skill
                Skills[level].UseSkill();
                curMana = 0;
                manabar.UpdateBar(curMana);
            }
            else
            {
                manabar.UpdateBar(curMana);
            }
        }
    }

    protected virtual void Attack()
    {
        if (!canAttack)
            return;

        //animator.SetTrigger("attack");

        waitBetweenAttack = 1 / attackSpeed;
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        canAttack = false;
        yield return null;
        //animator.ResetTrigger("attack");
        yield return new WaitForSeconds(waitBetweenAttack);
        canAttack = true;
    }
}
