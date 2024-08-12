using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup CanvasGroup;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI HighScore; 

    private int score;


    private void Start()
    {
        NewGame();
    }

    public void NewGame(){

        setScore(0);
        HighScore.text = loadHighScore().ToString();
        
        CanvasGroup. alpha = 0f;
        CanvasGroup. interactable = false;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()  {
        board.enabled = false;
        CanvasGroup.interactable = true;
        Debug.Log("Game overrrrrrrrrrrrrrrr");
        StartCoroutine(Fade(CanvasGroup,1f,1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay){

        yield return new WaitForSeconds(delay);

        float elapsed = 0f; 
        float duraiton = 0.5f;

        float from = canvasGroup.alpha;

        while (elapsed < duraiton)
        {
            canvasGroup.alpha = Mathf.Lerp(from,to, elapsed / duraiton);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void setScore(int score){
        this.score = score;
        scoreText.text = score.ToString();

        SaveHighScore();
    }

    public void IncreaseScore(int points){
        setScore(score + points);
    }

    private void SaveHighScore(){
        int HighScore = loadHighScore();

        if (score > HighScore)
        {
            PlayerPrefs.SetInt("HighScore",score);
        }
    }

    private int loadHighScore(){
        return PlayerPrefs.GetInt("HighScore",0);
    }
}
