using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public int coins = 0;
    public int score = 0;
    public int lives = 1;
    public float time = 400f;
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text livesText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text scorePopUpText;
    private float onScreenTime = 0.5f;
    private float timer = 0f;
    private bool onScreen = false;
    public bool win = false;

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
        if(lives <= 0)
            GameOver();
        if(onScreen){
            timer += Time.deltaTime;
            if(timer >= onScreenTime){
                scorePopUpText.gameObject.SetActive(false);
                onScreen = false;
                timer = 0;
            }
        }
        if(!win){
            time -= Time.deltaTime;
        } else
            ConvertTimeToScore();
    }

    public void UpdateHUD(){
        scoreText.text = "Score\n" + score.ToString();
        coinText.text = "Coins\n" + coins.ToString();
        timeText.text = "Time\n" + time.ToString("F0");
        livesText.text = "Lives\n" + lives.ToString();
    }

    public void AddScore(float _score, Transform _transform){
        score += (int)_score;
        // displays score pop up
        if(_transform != null){
            scorePopUpText.text = _score.ToString();
            scorePopUpText.transform.position = Camera.main.WorldToScreenPoint(new Vector2(_transform.position.x, _transform.position.y + 0.5f));
            scorePopUpText.gameObject.SetActive(true);
            onScreen = true;
            timer = 0;
        }
    }

    public void AddCoin(){
        coins++;
        AddScore(200, null);
    }

    public void ConvertTimeToScore(){
        if(time > 0){
            time -= Time.deltaTime * 100;
            AddScore(Time.deltaTime * 100 * 50, null);
        } else
            time = 0;
    }

    public void GameOver(){
        coins = 0;
        score = 0;
        lives = 3;
        time = 400f;
    }
}

