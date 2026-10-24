using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public TMP_Text rankingText;
    public TMP_Text categoryText;

    private string rankingCategory = "NORMAL";

    void Start()
    {
        UpdateRankingText();
    }

    public void ShowNormalRanking()
    {
        rankingCategory = "NORMAL";
        UpdateRankingText();
    }

    public void ShowHardRanking()
    {
        rankingCategory = "HARD";
        UpdateRankingText();
    }

    public void ShowEasyRanking()
    {
        rankingCategory = "EASY";
        UpdateRankingText();
    }

    void UpdateRankingText()
    {
        if (rankingText == null)
            return;

        if (categoryText != null)
        {
            categoryText.text = rankingCategory + " RANKING";
        }

        rankingText.text = "";
        Debug.Log("現在のカテゴリ=" + rankingCategory);

        for (int i = 0; i < 10; i++)
        {
            Debug.Log(
                 $"{rankingCategory}_RankName{i}"
                    );
            string name =
                PlayerPrefs.GetString(
                    $"{rankingCategory}_RankName{i}",
                    "---");

            int score =
                PlayerPrefs.GetInt(
                    $"{rankingCategory}_RankScore{i}",
                    0);

            rankingText.text +=
                $"{i + 1}位　{name}　{score}\n";
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ClearCurrentRanking()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey(
                $"{rankingCategory}_RankName{i}");

            PlayerPrefs.DeleteKey(
                $"{rankingCategory}_RankScore{i}");
        }

        PlayerPrefs.Save();

        UpdateRankingText();
    }
}