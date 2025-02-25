using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public abstract class UIMoving : SaiBehaviour
{
    [SerializeField] protected Vector3 standByPosition;
    [SerializeField] protected Vector3 start;
    [SerializeField] protected Vector3 end;
    [SerializeField] protected float speed = 2000f;
    [SerializeField] protected bool movingToB = false;
    [SerializeField] protected bool canMove = false;

    [SerializeField] static List<UIMoving> instances = new List<UIMoving>();

    protected override void Awake()
    {
        base.Awake();
        instances.Add(this);
    }

    protected virtual void Update()
    {
        this.Moving();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadStandByPosition();
        this.LoadPointA();
        this.LoadPointB();
    }

    protected abstract void LoadPointA();

    protected abstract void LoadPointB();


    protected virtual void LoadStandByPosition()
    {
        if (this.standByPosition != Vector3.zero) return;
        this.standByPosition = transform.localPosition;
        Debug.LogWarning(transform.name + ": LoadPointA", gameObject);
    }

    protected virtual void Moving()
    {
        if (!this.canMove) return;
        Vector3 target = movingToB ? this.start : this.end;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, target) < 0.01f)
        {
            this.movingToB = !this.movingToB;
            this.canMove = false;
        }
    }

    [ProButton]
    public virtual void Move()
    {
        DisappearAll(this);
        this.canMove = true;
        if (!this.movingToB) transform.localPosition = this.start;
    }

    [ProButton]
    public virtual void Disappear()
    {
        transform.localPosition = this.start;
        this.movingToB = false;
    }

    public static void DisappearAll(UIMoving caller = null)
    {
        foreach (var instance in instances)
        {
            if (instance == caller) continue;
            instance.Disappear();
        }
    }
}
