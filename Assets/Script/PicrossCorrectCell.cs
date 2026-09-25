using UnityEngine;

public class PicrossCorrectCell : PicrossCell
{
    private bool isBlueNow = false;

    protected override void OnStateChanged(int newState)
    {
        bool wasBlue = isBlueNow;
        isBlueNow = (newState == 1);

        if (!wasBlue && isBlueNow)
        {
            BoardManager.Instance.AddCorrect();
        }
        else if (wasBlue && !isBlueNow)
        {
            BoardManager.Instance.RemoveCorrect();
        }
    }
}
