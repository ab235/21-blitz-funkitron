using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Unity.VisualScripting.Dependencies.NCalc;

// Enumeration for the different types of scoring
public enum ScoreType { none, normal, vegas};

// Class which manage game stats
public class MatchStatistics : MonoBehaviour {

	// Create a static instance of the class to access it from other scripts
	public static MatchStatistics instance;

	// Property to store if the draw 3 option is selected
	public bool isDraw3 { get; private set; }

	// Text components to display time, moves and score
	public Text timeText;
	public Text movesText;
	public Text scoreText;
	public Text failureText;
	public Text streakText;

	// Private variable to store the score
	private int _score;

	private int _base_score;

	// Private variable to store the number of moves
	private int _moves;

	private int _failures;

	private int _streak;

	public int num_combos;

	public int max_streak_points;

	// Private variable to store the starting time of the game
	private float startTime;

	// Private variable to store the vegas score
	private int _vegasScore = 0;

	// Private variable to store the total time
	private float totalTime;

	// Private variable to keep track if the time is running
	private bool timeRun = true;

	// Private variable to store the type of scoring selected
	private ScoreType scoreType = ScoreType.normal;

	// Method to stop the time
	public void StopTime()
	{
		timeRun = false;
	}

	// Method to start the time
	public void StartTime()
	{
		timeRun = true;
	}

	// Method to check if the game is a Vegas game
	public bool IsVegasGame()
	{
		return scoreType == ScoreType.vegas;
	}

	// Method to get the string representation of the scoring type
	public string GetStringScoreType()
	{
		return scoreType.ToString();
	}

	// Property to get and set the score
	public int score
	{
		get
		{ return _score; }
		set
		{
			_score = value;
			RefreshPointsText();
		}
	}

	public int streak
	{
		get { return _streak; }
		set
		{
			_streak = value;
			RefreshStreakText();
		}
	}

	// Method to add the score
	public void AddScore(int amount)
	{
		score += amount;
	}

	// Property to get and set the Vegas score
	public int vegasScore
	{
		get
		{
			return _vegasScore;
		}
		set
		{
			_vegasScore = value;
			RefreshPointsText();
		}
	}

	// Method to handle the normal score toggle setting
	public void OnNormalScoreToggleSetting(bool state) {
		if (state)
			ChangeScoringType(ScoreType.normal);
	}

	// Method to handle the Vegas score toggle setting
	public void OnVegasScoreToggleSetting(bool state) {
		if (state)
			ChangeScoringType(ScoreType.vegas);
	}

	// Method to handle the Draw 3 toggle setting
	public void OnDraw3ToggleSetting(bool state) {
		isDraw3 = state;
	}

	// Method to change the scoring type
	private void ChangeScoringType(ScoreType type)
	{
		scoreType = type;
		RefreshPointsText();
	}

	// Property to get and set the number of moves
	public int moves
	{
		get { return _moves; }
		set
		{
			_moves = value;
			RefreshMovesText();
		}
	}

    public int failures
    {
        get { return _failures; }
        set
        {
            _failures = value;
            RefreshFailuresText();
        }
    }

    private void RefreshFailuresText()
	{
        failureText.text = "Failures: " + _failures.ToString() + "/3";
    }

    private void RefreshStreakText()
    {
        streakText.text = _streak.ToString() + "/4";
    }

    // This script is used for game state management and display of game time, score, and moves
    void Awake()
	{
		// Assigns the instance to the current object
		instance = this;
		// Calls the ResetState method to reset the game state
		ResetState();
	}

	public float GetGameTime()
	{
		// Returns the current game time (total time - start time)
		return totalTime - startTime;
	}

	public void ResetState()
	{
		// Resets the start time to the current time
		startTime = Time.time;

		// Sets the total time to the current time
		totalTime = startTime;

		// Resets the vegas score to -52
		vegasScore = -52;

		// Resets the score to 0
		score = 0;

		streak = 0;

		num_combos = 0;

		// Resets the moves to 0
		moves = 51;

		RefreshFailuresText();

        // Calls the StartTime method
        StartTime();
	}

	void Update()
	{
		// Calls the ShowTime method
		ShowTime();
	}

	private void ShowTime() {
		// If the timeRun flag is true
		if (timeRun)
		{
			// Increments the total time by delta time
			totalTime += Time.deltaTime;

			// Calculates the time as total time - start time
			float t = 180 - (totalTime - startTime);
			if (t <= 0)
			{
                BoardManager.instance.FinishGame();
                BoardManager.instance.LockBoard();
			}

			// Formats the time string
			string stringTime = t > 99 * 60 ? "∞" : TimeToString.GetTimeStringMSFormat(180 - (totalTime - startTime));

			// Updates the time text with the formatted time string
			timeText.text = "time: " + stringTime;
		}
	}

	private void RefreshPointsText()
	{
		// Switch statement to determine the score type and update the score text accordingly
		switch (scoreType)
		{
			case ScoreType.none:
				scoreText.text = "";
				break;
			case ScoreType.normal:
				scoreText.text = "score: " + score.ToString();
				break;
			case ScoreType.vegas:
				scoreText.text = "score: " + vegasScore.ToString() + "$";
				break;
			default:
				break;
		}
	}

	private void RefreshMovesText()
	{
		// Updates the moves text with the current number of moves
		movesText.text = moves.ToString();
	}   
}
