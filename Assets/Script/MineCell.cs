using UnityEngine;

public class MineCell : PicrossCell
{
    public Sprite mineSprite;

    protected override void OnStateChanged(int newState)
    {
        // Â‚É‚È‚Á‚½¶ƒNƒŠƒbƒN‚³‚ê‚½
        if (!isFlagged && newState == 1)
        {
            BoardManager.Instance.GameOver();
        }
    }

    public void ShowMine()
    {
        sr.sprite = mineSprite;
    }
}
