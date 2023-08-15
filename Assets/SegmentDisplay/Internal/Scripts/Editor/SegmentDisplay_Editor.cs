//    SegmentDisplay - Custom editor


using UnityEngine;
using UnityEditor;
using System;

namespace Leguar.SegmentDisplay {

	public abstract class SegmentDisplay_Editor : Editor {

		private static GUIStyle headerStyle;
		private static GUIStyle noteStyle;

		public void mainEditor(SegmentDisplay sdTarget, ref bool changes) {

			addHeader("Display type and style");

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Display type defines number of segments in each digit of display");
			sdTarget.DisplayType=(SegmentDisplay.DisplayTypes)(enumChoice("Display type",sdTarget.DisplayType,ref changes));
			if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.SevenSegment) {
				help(sdTarget.EditorHelp,"Display style chooses shape of the segments");
				sdTarget.SevenSegmentStyle=(SegmentDisplay.SevenSegmentStyles)(enumChoice("7-segment style",sdTarget.SevenSegmentStyle,ref changes));
			} else if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.FourteenSegment) {
				help(sdTarget.EditorHelp,"Display style chooses shape of the segments");
				sdTarget.FourteenSegmentStyle=(SegmentDisplay.FourteenSegmentStyles)(enumChoice("14-segment style",sdTarget.FourteenSegmentStyle,ref changes));
			} else if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.SixteenSegment) {
				help(sdTarget.EditorHelp,"Display style chooses shape of the segments");
				sdTarget.SixteenSegmentStyle=(SegmentDisplay.SixteenSegmentStyles)(enumChoice("16-segment style",sdTarget.SixteenSegmentStyle,ref changes));
			}
			EditorGUI.EndDisabledGroup();

			if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.SevenSegment) {
				help(sdTarget.EditorHelp,"Special mods change how certain numbers appears in seven segment display");
				sdTarget.Mod_S7S14_SixNine=boolToggle("6 and 9 without last segment",sdTarget.Mod_S7S14_SixNine,ref changes);
				sdTarget.Mod_S7_Seven=boolToggle("7 with extra segment",sdTarget.Mod_S7_Seven,ref changes);
			} else if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.FourteenSegment) {
				help(sdTarget.EditorHelp,"Special mods change how certain numbers appears in fourteen segment display");
				sdTarget.Mod_S14S16_Zero=boolToggle("0 with slash",sdTarget.Mod_S14S16_Zero,ref changes);
				sdTarget.Mod_S14S16_One=boolToggle("1 without extra segment",sdTarget.Mod_S14S16_One,ref changes);
				sdTarget.Mod_S7S14_SixNine=boolToggle("6 and 9 without last segment",sdTarget.Mod_S7S14_SixNine,ref changes);
				sdTarget.Mod_S14S16_Seven=boolToggle("7 with diagonal line",sdTarget.Mod_S14S16_Seven,ref changes);
			} else if (sdTarget.DisplayType==SegmentDisplay.DisplayTypes.SixteenSegment) {
				help(sdTarget.EditorHelp,"Special mods change how characters appears in sixteen segment display");
				sdTarget.Mod_S14S16_Zero=boolToggle("0 with slash",sdTarget.Mod_S14S16_Zero,ref changes);
				sdTarget.Mod_S14S16_One=boolToggle("1 without extra segment",sdTarget.Mod_S14S16_One,ref changes);
				sdTarget.Mod_S14S16_Seven=boolToggle("7 with diagonal line",sdTarget.Mod_S14S16_Seven,ref changes);
			}

			addHeader("Display capacity");

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Defines actual size of display in digits/characters");
			sdTarget.DigitCount=intSlider("Digit/char count",sdTarget.DigitCount,1,SegmentDisplay.DIGIT_COUNT_SAFETY_LIMIT,ref changes);
			help(sdTarget.EditorHelp, "Choose whatever each digit have apostrophe after it");
			sdTarget.ApostrophesEnabled=boolToggle("Include apostrophes", sdTarget.ApostrophesEnabled, ref changes);
			help(sdTarget.EditorHelp,"Choose whatever each digit have decimal point after it");
			sdTarget.DecimalPointsEnabled=boolToggle("Include decimal points",sdTarget.DecimalPointsEnabled,ref changes);
			EditorGUI.EndDisabledGroup();

			addHeader("Segment colors");

			help(sdTarget.EditorHelp,"Set segment color when it is not lit, note that color can be also fully or partially transparent");
			sdTarget.OffColor=colorField("Segment Off Color",sdTarget.OffColor,ref changes);
			help(sdTarget.EditorHelp,"Set segment color when it is lit, note that color can also have transparency");
			sdTarget.OnColor=colorField("Segment On Color",sdTarget.OnColor,ref changes);

			addHeader("Display visuals");

			help(sdTarget.EditorHelp,"Change spacing between segments in one digit, making digits tighter or looser");
			sdTarget.SegmentSpread=intSlider("Segment spread (%)",Mathf.RoundToInt(sdTarget.SegmentSpread*100f),80,150,ref changes)/100f;
			help(sdTarget.EditorHelp,"Change spacing between digits to make display tighter or looser");
			sdTarget.DigitSpacing=intSlider("Digit spacing (%)",Mathf.RoundToInt(sdTarget.DigitSpacing*100f),20,200,ref changes)/100f;

			if (sdTarget is SegmentDisplaySprite) {

				SegmentDisplaySprite sdSpriteTarget=(SegmentDisplaySprite)(sdTarget);

				help(sdTarget.EditorHelp,"Change sorting order of all SpriteRenderers this display is using, this will set display in 2D worlds front or behind other sprites");
				sdSpriteTarget.SpriteSortingOrder=intField("Sprite sorting order",sdSpriteTarget.SpriteSortingOrder,-32768,32767,ref changes);

			} else if (sdTarget is SegmentDisplayImage) {

				SegmentDisplayImage sdImageTarget=(SegmentDisplayImage)(sdTarget);

				Rect goRect=sdImageTarget.GetComponent<RectTransform>().rect;
				if (goRect.width<=0f || goRect.height<=0f) {
					Debug.LogWarning("SegmentDisplay ("+sdImageTarget.gameObject.name+"): RectTransform 'width' or 'height' is zero or negative. Display will not be visible.");
				}

				help(sdTarget.EditorHelp,"Set whatever display should fill whole RectTransform rect, or keep its aspect ratio and position itself center, side or corner of the rect");
				sdImageTarget.UIPosition=(SegmentDisplayImage.UIPositions)(enumChoice("Position inside rect",sdImageTarget.UIPosition,ref changes));

			}

			addHeader("Colons");

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Enable colons to change digits in wanted positions to colons (:)");
			sdTarget.ColonsEnabled=boolToggle("Enable colons",sdTarget.ColonsEnabled,ref changes);
			EditorGUI.EndDisabledGroup();
			if (sdTarget.ColonsEnabled) {
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				help(sdTarget.EditorHelp,"Click buttons below to choose which digits should be colons, click again to change it back to normal digit");
				GUILayout.BeginHorizontal();
				for (int n=0; n<sdTarget.DigitCount; n++) {
					if (GUILayout.Button((sdTarget.ColonAtIndex[n]?":":"#"),GUILayout.Width(25f))) {
						sdTarget.ColonAtIndex[n]=!sdTarget.ColonAtIndex[n];
						changes=true;
					}
					if (n%16==15 && n<sdTarget.DigitCount-1) {
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
					}
				}
				GUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				help(sdTarget.EditorHelp,"Change spacing just around colons to tighter or looser");
				sdTarget.ColonRelativeSpacing=intSlider("Colon spacing (%)",Mathf.RoundToInt(sdTarget.ColonRelativeSpacing*100f),0,100,ref changes)/100f;
			}

			addHeader("Smaller digits");

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Enable variable sizes to make some digits smaller");
			sdTarget.SmallerDigitsEnabled=boolToggle("Enable smaller digits",sdTarget.SmallerDigitsEnabled,ref changes);
			EditorGUI.EndDisabledGroup();
			if (sdTarget.SmallerDigitsEnabled) {
				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				help(sdTarget.EditorHelp,"Click buttons below to choose which digits should be smaller, click again to change it back to normal size");
				GUILayout.BeginHorizontal();
				for (int n=0; n<sdTarget.DigitCount; n++) {
					if (GUILayout.Button((sdTarget.SmallerDigitAtIndex[n]?"s":"N"),GUILayout.Width(25f))) {
						sdTarget.SmallerDigitAtIndex[n]=!sdTarget.SmallerDigitAtIndex[n];
						changes=true;
					}
					if (n%16==15 && n<sdTarget.DigitCount-1) {
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
					}
				}
				GUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				help(sdTarget.EditorHelp,"Set the size of smaller digits compared to normal size digits");
				sdTarget.SmallerDigitsSizePercent=intSlider("Smaller digits size (%)",Mathf.RoundToInt(sdTarget.SmallerDigitsSizePercent*100f),10,95,ref changes)/100f;
				help(sdTarget.EditorHelp,"Set smaller digits vertical position compared to normal size digits");
				sdTarget.SmallerDigitsVerticalPosition=intSlider("Smaller digits vertical position",Mathf.RoundToInt(sdTarget.SmallerDigitsVerticalPosition*100f),0,100,ref changes)/100f;
			}

			addHeader("Default content");

			help(sdTarget.EditorHelp,"Default alignment is used runtime, but it also defines alignment of initial text and editor test text");
			sdTarget.DefaultAlignment=(SegmentDisplay.Alignments)(enumChoice("Default text alignment",sdTarget.DefaultAlignment,ref changes));
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Initial text to display when application runs, can be also empty string");
			sdTarget.InitialText=textArea("Initial text at startup",sdTarget.InitialText,ref changes);
			EditorGUI.EndDisabledGroup();

			addHeader("Test in Unity Editor");

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			help(sdTarget.EditorHelp,"Display content here in editor, this have no effect when application is running");
			sdTarget.EditorTestContent=(SegmentDisplay.EditorTestContents)(enumChoice("Test content",sdTarget.EditorTestContent,ref changes));
			EditorGUI.BeginDisabledGroup(sdTarget.EditorTestContent!=SegmentDisplay.EditorTestContents.Text);
			bool textChange = false;
			help(sdTarget.EditorHelp,"Try out your display with type of texts you eventually will use on display");
			sdTarget.EditorTestText=textArea("Editor test text",sdTarget.EditorTestText,ref textChange);
			if (textChange) {
				sdTarget.EditorTestContent=SegmentDisplay.EditorTestContents.Text;
				changes=true;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();

			addHeader("Help");

			sdTarget.EditorHelp=boolToggle("Enable inspector help texts",sdTarget.EditorHelp,ref changes);

			if (!Application.isPlaying && changes) {
				sdTarget.needEditorReCreate=true;
				EditorUtility.SetDirty(sdTarget);
			}

		}

		private static void help(bool enabled, string text) {
			if (enabled) {
				if (noteStyle==null) {
					noteStyle=new GUIStyle();
					noteStyle.fontStyle=FontStyle.Italic;
					noteStyle.wordWrap=true;
				}
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(text,noteStyle);
			}
		}

		public static void addHeader(string labelText) {
			
			if (headerStyle==null) {
				headerStyle=new GUIStyle();
				headerStyle.fontStyle=FontStyle.Bold;
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(labelText,headerStyle);
			
		}

		private static bool boolToggle(string labelText, bool currentValue, ref bool changes) {
			bool newValue=EditorGUILayout.Toggle(labelText,currentValue);
			if (newValue!=currentValue) {
				changes=true;
			}
			return newValue;
		}

		private static int intSlider(string labelText, int currentValue, int minimum, int maximum, ref bool changes) {
			int newValue=EditorGUILayout.IntSlider(labelText,currentValue,minimum,maximum);
			if (newValue!=currentValue) {
				changes=true;
			}
			return newValue;
		}

		private static int intField(string labelText, int currentValue, int minValue, int maxValue, ref bool changes) {
			int newValue=EditorGUILayout.IntField(labelText,currentValue);
			if (newValue<minValue) {
				newValue = minValue;
			}
			if (newValue>maxValue) {
				newValue = maxValue;
			}
			if (newValue!=currentValue) {
				changes=true;
			}
			return newValue;
		}

		private static Enum enumChoice(string labelText, Enum currentlySelected, ref bool changes) {
			Enum newSelected=EditorGUILayout.EnumPopup(labelText,currentlySelected);
			if (!newSelected.Equals(currentlySelected)) {
				changes=true;
			}
			return newSelected;
		}

		private static Color colorField(string labelText, Color currentColor, ref bool changes) {
			Color newColor=EditorGUILayout.ColorField(labelText,currentColor);
			if (newColor!=currentColor) {
				changes=true;
			}
			return newColor;
		}
		
		private static string textArea(string labelText, string contentText, ref bool changes) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(labelText);
			int indent=EditorGUI.indentLevel;
			EditorGUI.indentLevel=0;
			string newContentText=EditorGUILayout.TextArea(contentText);
			EditorGUI.indentLevel=indent;
			EditorGUILayout.EndHorizontal();
			if (newContentText!=contentText) {
				changes=true;
			}
			return newContentText;
		}

	}

}
