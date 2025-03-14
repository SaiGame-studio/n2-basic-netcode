public abstract class TextUpdate : TextAbstact
{
    protected virtual void FixedUpdate()
    {
        this.ShowingText();
    }

    protected abstract void ShowingText();
}
