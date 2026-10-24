using UnityEngine;

public class PicrossCorrectCell : PicrossCell
{
    private int lastState = 0;

    // š Start() ‚Í‘‚©‚È‚¢ ¨ PicrossCell.Start() ‚ªg‚í‚ê‚é

    protected override void OnStateChanged(int newState)
    {
        if (newState == lastState)
            return;

        if (newState == 1)
        {
            BoardManager.Instance.AddCorrect();
        }
        else if (lastState == 1)
        {
            BoardManager.Instance.RemoveCorrect();
        }

        lastState = newState;
    }
}
