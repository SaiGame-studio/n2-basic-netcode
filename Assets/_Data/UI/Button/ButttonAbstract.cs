using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButttonAbstract : SaiBehaviour
{
    [Header("Button")]
    [SerializeField] protected Button button;
    [SerializeField] protected TextMeshProUGUI buttonText;

    [SerializeField] protected float doubleClickThreshold = 0.3f;
    [SerializeField] protected float lastClickTime = 0f;         

    public abstract void OnClick();

    protected override void Start()
    {
        base.Start();
        this.AddOnClickEvent();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadButton();
    }

    protected virtual void LoadButton()
    {
        if (this.button != null) return;
        this.button = GetComponent<Button>();
        this.buttonText = this.button.GetComponentInChildren<TextMeshProUGUI>();
        Debug.LogWarning(transform.name + ": LoadButton", gameObject);
    }

    protected virtual void AddOnClickEvent()
    {
        //this.button.onClick.AddListener(this.OnClick);
        this.button.onClick.AddListener(this.DoubleClickChecker);
    }

    protected virtual void DoubleClickChecker()
    {
        float currentTime = Time.time;

        if (currentTime - lastClickTime <= doubleClickThreshold) this.OnDoubleClick();
        else this.OnClick();

        this.lastClickTime = currentTime;
    }

    public virtual void OnDoubleClick()
    {
        this.OnClick();
    }

    public virtual void SetText(string text)
    {
        this.buttonText.text = text;
    }
}
