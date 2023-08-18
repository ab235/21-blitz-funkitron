using UnityEngine;
using System;

// Constants class for holding constant values used in the game
public class Constants {
	// Points for different card movements
	public const int TURN_OVER_CARD_POINTS = 5;
	public const int WASTE2TABLEAU_POINTS = 0;
	public const int WASTE2FOUNDATIONS_POINTS = 0;
	public const int STREAK_POINTS = 250;
	public const int TABLEAU2FOUNDATIONS_POINTS = 10;
	public const int FOUNDATION2TABLEAU_POINTS = -15;
	public const int NUMBER_OF_FULL_COLOR_CARDS = 13;
	public const int VEGAS_SCORE_PER_CARD = 5;
	public const int STACK_CLEAR_POINTS = 400;
	public const int NUMBER_OF_TABLEAUS = 0;
	public const int NUMBER_OF_FOUNDATIONS = 4;
	public const float AUTOCOMPLETE_TIME_SCALE = 1.5f;
	public const int STOCK_START_INDEX = 0;
	public const float CARD_SCALE = (float)1.3;
	public const float CARD_HEIGHT_DIFF = (float)1.2;
	public const int DROPZONE_HEIGHT = 1;
	public const int PADDING_SIDE = 20;
	public const int MAX_FOUND_WIDTH = 220;

	// Constants for deck of cards
	public const int CARDS_IN_DECK = 52;
	public const int CARDS_IN_COLOR = 13;
	public const int CARDS_COLORS = 4;

	// Constants for animations and spacing
	public const float TABLEAU_SPACING = -127.7778f;
	public const float LANDSCAPE_TABLEAU_SPACING = -139f;
	public const float ANIM_TIME = 0.208f;
	public const float STOCK_ANIM_TIME = 0.125f;
	public const float MOVE_ANIM_TIME = 0.33f;
	public const float HINT_ANIM_TIME = 0.5f;
	public const float SOUND_LOCK_TIME = 0.075f;
	public const float CARD_SIZE_ANIM_SPEED = 0.33f;
	public const float TOP_BAR_SIZE = 80f;

	// Readonly Vector2 and Vector3 constants
	public static readonly Vector2 vectorZero = new Vector2(0, 0);
	public static readonly Vector2 vectorHalf = new Vector2(0.5f, 0.5f);
	public static readonly Vector2 baseCardSize = new Vector2(127.7778f, 180f);

	public static readonly Vector3 vectorRight = new Vector3(1, 0, 0);
	public static readonly Vector3 vectorOne = new Vector3(1, 1, 1);
	public static readonly Vector3 vectorZoom = new Vector3(1.1f, 1.1f, 0);
	public static readonly Vector3 rotationZ180 = new Vector3(0, 0, 180);
	public static readonly Vector3 rotationZ0 = new Vector3(0, 0, 0);

	// Array of available card indexes
	public static readonly int[] availableCardsIndexes = new int[] { 0, 2, 5, 9, 14, 20, 27 };
}
