public abstract class Text3DUpdate : Text3DAbstact
{
    protected virtual void FixedUpdate()
    {
        this.ShowingText();
    }

    protected abstract void ShowingText();
}
