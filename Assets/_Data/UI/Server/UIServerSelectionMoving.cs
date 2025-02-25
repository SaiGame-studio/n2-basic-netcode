using UnityEngine;

public class UIServerSelectionMoving : UIMoving
{
    protected override void Start()
    {
        base.Start();
        this.Move();
    }

    protected override void LoadPointA()
    {
        if (this.start != Vector3.zero) return;
        this.start = new Vector3(-600, -15, 0);
        Debug.LogWarning(transform.name + ": LoadPointA", gameObject);
    }

    protected override void LoadPointB()
    {
        if (this.end != Vector3.zero) return;
        this.end = new Vector3(-342, -15, 0);
        Debug.LogWarning(transform.name + ": LoadPointB", gameObject);
    }
}
