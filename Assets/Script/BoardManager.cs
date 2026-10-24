
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public enum RankingCategory
{
    NORMAL,
    HARD,
    EASY
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public int width = 5;
    public int height = 5;

    public float startX = -4.5f;
    public float startY = -3.5f;
    public float spacing = 1.1f;

    [Header("スコア設定")]
    public float scoreGraceTime = 30f;
    public int maxScore = 10000;
    public int minScore = 1000;
    public int scorePenaltyPerSecond = 20;

    [Header("BGM")]
    public AudioSource mainBGM;
    public AudioSource clearBGM;
    public AudioSource gameOverBGM;

    private PicrossCell[,] cells;

    public GameObject clearPanel;
    public GameObject gameOverPanel;

    public TMP_Text timeText;
    public TMP_Text scoreText;

    private int correctTotal = 0;
    private int correctNow = 0;
    private int missCount = 0;

    private float elapsedTime = 0f;
    private bool gameFinished = false;

    public TMP_InputField nameInputField;

    [Header("ランキング設定")]
    public RankingCategory rankingCategory;

    public Button registerButton;



    private int latestScore;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(InitBoard());
    }

    void Update()
    {
        if (!gameFinished)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            if (timeText != null)
            {
                timeText.text = $"{minutes:00}:{seconds:00}";
            }
        }
    }

    IEnumerator InitBoard()
    {
        yield return null;

        cells = new PicrossCell[width, height];

        var allCells = FindObjectsOfType<PicrossCell>();

        foreach (var c in allCells)
        {
            int col = Mathf.RoundToInt(
                (c.transform.position.x - startX) / spacing);

            int row = Mathf.RoundToInt(
                (c.transform.position.y - startY) / spacing);

            if (col < 0 || col >= width ||
                row < 0 || row >= height)
            {
                continue;
            }

            cells[col, row] = c;
        }

        var correctCells = FindObjectsOfType<PicrossCorrectCell>();
        correctTotal = correctCells.Length;

        Debug.Log("CorrectTotal = " + correctTotal);

        CountMineNumbers();
    }

    void CountMineNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];

                if (cell == null) continue;
                if (cell is MineCell) continue;

                int count = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < 0 || nx >= width ||
                            ny < 0 || ny >= height)
                            continue;

                        if (cells[nx, ny] is MineCell)
                        {
                            count++;
                        }
                    }
                }

                cell.SetNumber(count);
            }
        }
    }

    public void AddCorrect()
    {
        correctNow++;
        CheckClear();
    }

    public void RemoveCorrect()
    {
        correctNow--;

        if (correctNow < 0)
            correctNow = 0;

        CheckClear();
    }

    public void AddMiss()
    {
        missCount++;
        CheckClear();
    }

    public void RemoveMiss()
    {
        if (missCount > 0)
        {
            missCount--;
        }

        CheckClear();
    }

    void CheckClear()
    {
        if (gameFinished)
            return;

        if (correctNow == correctTotal &&
            missCount == 0)
        {
            gameFinished = true;

            // BGM切り替え
            if (mainBGM != null)
            {
                mainBGM.Stop();
            }

            if (clearBGM != null)
            {
                clearBGM.Play();
            }

            //---------------------------------
            // タイムスコア
            //---------------------------------

            int baseScore;

            if (elapsedTime <= scoreGraceTime)
            {
                baseScore = maxScore;
            }
            else
            {
                baseScore = Mathf.Max(
                    minScore,
                    maxScore -
                    Mathf.RoundToInt(
                        (elapsedTime - scoreGraceTime)
                        * scorePenaltyPerSecond)
                );
            }

            //---------------------------------
            // 地雷旗ボーナス
            //---------------------------------

            int flagBonus = 0;

            var mines = FindObjectsOfType<MineCell>();

            int totalMineCount = mines.Length;
            int flaggedMineCount = 0;

            foreach (var mine in mines)
            {
                if (mine.IsFlagged())
                {
                    flaggedMineCount++;
                    flagBonus += 10;
                }
            }

            //---------------------------------
            // 全地雷発見ボーナス
            //---------------------------------

            if (flaggedMineCount == totalMineCount &&
                totalMineCount > 0)
            {
                flagBonus += 1000;
            }

            //---------------------------------
            // 最終スコア
            //---------------------------------

            int score = baseScore + flagBonus;
            latestScore = score;


            Debug.Log("CLEAR!");
            Debug.Log("Time Score : " + baseScore);
            Debug.Log("Flag Bonus : " + flagBonus);
            Debug.Log("Total Score : " + score);

            if (scoreText != null)
            {
                scoreText.text =
                    $"TIME SCORE : {baseScore}\n" +
                    $"FLAG BONUS : {flagBonus}\n" +
                    $"TOTAL SCORE : {score}";
            }

            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
            }
        }
    }

    public void GameOver()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        // BGM切り替え
        if (mainBGM != null)
        {
            mainBGM.Stop();
        }

        if (gameOverBGM != null)
        {
            gameOverBGM.Play();
        }

        Debug.Log("GAME OVER");

        var mines = FindObjectsOfType<MineCell>();

        foreach (var m in mines)
        {
            m.ShowMine();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RegisterRanking()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "NO NAME";
        }

        SaveRanking(playerName, latestScore);

        if (registerButton != null)
        {
            registerButton.interactable = false;
        }
    }

    private void SaveRanking(string playerName, int score)
    {
        Debug.Log("保存カテゴリ=" + rankingCategory);

        string category = rankingCategory.ToString();
        string[] names = new string[10];
        int[] scores = new int[10];

        // 既存ランキング取得
        for (int i = 0; i < 10; i++)
        {
            names[i] =
                PlayerPrefs.GetString(
                    $"{category}_RankName{i}",
                     "---");

            scores[i] =
                PlayerPrefs.GetInt(
                    $"{category}_RankScore{i}",
                    0);
        }

        // ランキングへ挿入
        for (int i = 0; i < 10; i++)
        {
            if (score > scores[i])
            {
                for (int j = 9; j > i; j--)
                {
                    scores[j] = scores[j - 1];
                    names[j] = names[j - 1];
                }

                scores[i] = score;
                names[i] = playerName;

                break;
            }
        }

        // 保存
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetString(
                $"{category}_RankName{i}",
                names[i]);

            PlayerPrefs.SetInt(
                $"{category}_RankScore{i}",
                scores[i]);
        }

        PlayerPrefs.Save();
    }
}