//    Leguar SegmentDisplay


using System;
using UnityEngine;

namespace Leguar.SegmentDisplay {

	[ExecuteInEditMode]
	/// <summary>
	/// Main class of SegmentDisplay. This class contains majority of all the methods and settings needed to use SegmentDisplay.
	/// 
	/// There are two other classes derived from this class, SegmentDisplaySprite and SegmentDisplayImage for sprite and image based displays.
	/// Sprite based display is used in 2D and 3D worlds, while Image based display is used in UI. Those two classes contains some setting specific
	/// for these environments.
	/// 
	/// Normally SegmentDisplay is set up in Unity Editor inspector. All properties and fields of this class need to be changed by code only in case
	/// SegmendDisplay is created from prefab during runtime and it wasn't in scene before. But if SegmendDisplay prefab is already added to scene
	/// in Unity Editor, you only need to use methods of this class in your code.
    /// 
    /// Note when setting or adding text to display that is using colons. Make sure that text you are setting is matching display digits, so that
    /// colon in your text will match the location of colon in display. If display is for example "##:##" (5 digits where third one is colon") and
	/// you try to add text "2:15", that may cause display to show for example "2  15". In this case solution is to add text " 2:15" instead (notice
    /// space before number 2) or add text "2:15" as right aligned.
	/// </summary>
	public abstract class SegmentDisplay : MonoBehaviour {

		// Need more than this? Just increase this value, there is no program limitations how many digits display may have.
		// This is mostly to keep inspector editor digit slider easy to use.
		public const int DIGIT_COUNT_SAFETY_LIMIT=32;

		private const string GENERATED_DIGIT_GROUP_NAME_EDITOR="Generated_Digits_Preview";
		private const string GENERATED_DIGIT_GROUP_NAME_RUNTIME="Generated_Digits";


		/// <summary>
		/// Different display types, telling how many segments there are in each digit.
		/// </summary>
		public enum DisplayTypes {
			/// <summary>
			/// Classic seven segment display. This type of display typically shows just numbers. Best choice for simple clocks, timers or scoreboards.
			/// 
			/// This type display support also letters, but some of them may require imagination to read and some are more like symbolic, like M and W.
			/// This is sufficient for showing short, one word messages such "Start", "End", "Play", "Stop", "Pause", "Open", "Close".
			/// But due limitation of segments, these texts may have upper and lower case letters mixed and "start" looks more like "StArt".
			/// </summary>
			SevenSegment,
			/// <summary>
			/// Fourteen segment display is good choice if your display also needs to show text. Only upper case letters are supported but text is very readable.
			/// Multiple symbols are also supported.
			/// </summary>
			FourteenSegment,
			/// <summary>
			/// Very much like fourteen segment display but can also show lower case letters and few extra symbols such curly brackets.
			/// </summary>
			SixteenSegment
		}

		[SerializeField]
		private DisplayTypes displayType = DisplayTypes.SevenSegment;

		/// <summary>
		/// Type of the display, number of segments in each digit.
		/// </summary>
		/// <value>
		/// One of the choices from enum DisplayTypes.
		/// </value>
		public DisplayTypes DisplayType {
			set {
				if (value!=displayType) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Display type can not be changed after SegmentDisplay is initialized");
						return;
					}
					displayType=value;
				}
			}
			get {
				return displayType;
			}
		}

		/// <summary>
		/// Different styles for seven segment display.
		/// </summary>
		public enum SevenSegmentStyles {
			/// <summary>
			/// Basic all-around style.
			/// </summary>
			Basic,
			/// <summary>
			/// Classic style where each of seven segments are completely identical.
			/// </summary>
			Classic,
			/// <summary>
			/// Very sharp corners and square decimal points and colons.
			/// </summary>
			Sharp,
			/// <summary>
			/// Round style where each segment is different and they make smooth shapes.
			/// </summary>
			Round
		}
		
		[SerializeField]
		private SevenSegmentStyles sevenSegmentStyle = SevenSegmentStyles.Basic;
		
		/// <summary>
		/// Set or get seven segment display style.
		/// </summary>
		/// <value>
		/// One of the choices from enum SevenSegmentStyles.
		/// </value>
		public SevenSegmentStyles SevenSegmentStyle {
			set {
				if (value!=sevenSegmentStyle) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Seven segment display style can not be changed after SegmentDisplay is initialized");
						return;
					}
					sevenSegmentStyle=value;
				}
			}
			get {
				return sevenSegmentStyle;
			}
		}
		
		/// <summary>
		/// Different styles for fourteen segment display.
		/// </summary>
		public enum FourteenSegmentStyles {
			/// <summary>
			/// Basic style.
			/// </summary>
			Basic,
			/// <summary>
			/// Cheap electronics style. Practically just 7 segment display turned to 14 segment as easily as possible.
			/// </summary>
			Cheap
		}
		
		[SerializeField]
		private FourteenSegmentStyles fourteenSegmentStyle = FourteenSegmentStyles.Basic;
		
		/// <summary>
		/// Set or get fourteen segment display style.
		/// </summary>
		/// <value>
		/// One of the choices from enum FourteenSegmentStyles.
		/// </value>
		public FourteenSegmentStyles FourteenSegmentStyle {
			set {
				if (value!=fourteenSegmentStyle) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Fourteen segment display style can not be changed after SegmentDisplay is initialized");
						return;
					}
					fourteenSegmentStyle=value;
				}
			}
			get {
				return fourteenSegmentStyle;
			}
		}
		
		/// <summary>
		/// Different styles for sixteen segment display.
		/// </summary>
		public enum SixteenSegmentStyles {
			/// <summary>
			/// Basic style.
			/// </summary>
			Basic,
			/// <summary>
			/// Style where each segment is generated by rows of small, round lights.
			/// Very common in modern CD/DVD/BluRay players or stereos.
			/// </summary>
			MiniLeds
		}
		
		[SerializeField]
		private SixteenSegmentStyles sixteenSegmentStyle = SixteenSegmentStyles.Basic;
		
		/// <summary>
		/// Set or get sixteen segment display style.
		/// </summary>
		/// <value>
		/// One of the choices from enum SixteenSegmentStyles.
		/// </value>
		public SixteenSegmentStyles SixteenSegmentStyle {
			set {
				if (value!=sixteenSegmentStyle) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Sixteen segment display style can not be changed after SegmentDisplay is initialized");
						return;
					}
					sixteenSegmentStyle=value;
				}
			}
			get {
				return sixteenSegmentStyle;
			}
		}

		/// <summary>
		/// Draw number 0 with slash going through the number. Have effect on 14 and 16 segment displays.
		/// This can be changed during runtime, but it doesn't affect texts that is already on display.
		/// </summary>
		public bool Mod_S14S16_Zero = false;

		/// <summary>
		/// Draw number 1 without extra diagonal segment. Have effect on 14 and 16 segment displays.
		/// This can be changed during runtime, but it doesn't affect texts that is already on display.
		/// </summary>
		public bool Mod_S14S16_One = false;

		/// <summary>
		/// Draw numbers 6 and 9 without having last horizontal segment. Have effect on 7 and 14 segment displays.
		/// This can be changed during runtime, but it doesn't affect texts that is already on display.
		/// </summary>
		public bool Mod_S7S14_SixNine = false;

		/// <summary>
		/// Draw number 7 with extra vertical line on top left. Have effect on 7 segment display.
		/// This can be changed during runtime, but it doesn't affect texts that is already on display.
		/// </summary>
		public bool Mod_S7_Seven = false;

		/// <summary>
		/// Draw number 7 with diagonal line instead vertical. Have effect on 14 and 16 segment displays.
		/// This can be changed during runtime, but it doesn't affect texts that is already on display.
		/// </summary>
		public bool Mod_S14S16_Seven = false;


		[SerializeField]
		private int digitCount = 4;
		
		/// <summary>
		/// Sets or gets number of digits (numbers and/or characters) in this display.
		/// </summary>
		/// <value>
		/// Number of digits in display.
		/// </value>
		public int DigitCount {
			set {
				if (value<1) {
					Debug.LogError("Leguar.SegmentDisplay: DigitCount can't be set below 1 (trying to set "+value+")");
					return;
				}
				if (value>DIGIT_COUNT_SAFETY_LIMIT) {
					Debug.LogError("Leguar.SegmentDisplay: DigitCount can't be set over "+DIGIT_COUNT_SAFETY_LIMIT+" (trying to set "+value+")");
					return;
				}
				if (value!=digitCount) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: DigitCount can not be changed after SegmentDisplay is initialized");
						return;
					}
					digitCount=value;
					if (colonsEnabled) {
						checkColonAtIndexArrayLength();
					}
					if (smallerDigitsEnabled) {
						checkSmallerDigitAtIndexArrayLength();
					}
				}
			}
			get {
				return digitCount;
			}
		}


		[SerializeField]
		private bool decimalPointsEnabled = false;

		/// <summary>
		/// Include extra decimal point segment after each digit.
		/// 
		/// Note that this cause that in some case display may hold longer texts than number of digits. If display size is for example 4 digits and decimal points are enabled,
		/// display pattern will be "#.#.#.#." meaning that even text "1.2.3.4." (8 characters) will fit to this 4 digit display. But on the other hand, "...12" (5 characters)
		/// will not fit.
		/// </summary>
		/// <value>
		/// True to enable decimal points, false to have digits without decimal point segment.
		/// </value>
		public bool DecimalPointsEnabled {
			set {
				if (value!=decimalPointsEnabled) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Decimal points visibility can not be changed after SegmentDisplay is initialized");
						return;
					}
					decimalPointsEnabled=value;
				}
			}
			get {
				return decimalPointsEnabled;
			}
		}

		[SerializeField]
		private bool apostrophesEnabled = false;

		/// <summary>
		/// Include extra apostrophe segment after each digit.
		/// 
		/// Note that this cause that in some case display may hold longer texts than number of digits. If display size is for example 4 digits and apostrophes are enabled,
		/// display pattern will be "#'#'#'#'" meaning that even text "1'2'3'4'" (8 characters) will fit to this 4 digit display. But on the other hand, "'''12" (5 characters)
		/// will not fit.
		/// </summary>
		/// <value>
		/// True to enable apostrophes, false to have digits without apostrophe segment.
		/// </value>
		public bool ApostrophesEnabled {
			set {
				if (value!=apostrophesEnabled) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Apostrophes visibility can not be changed after SegmentDisplay is initialized");
						return;
					}
					apostrophesEnabled=value;
				}
			}
			get {
				return apostrophesEnabled;
			}
		}


		[SerializeField]
		private bool colonsEnabled = false;

		/// <summary>
		/// Enable or disable colons.
		/// </summary>
		/// <value>
		/// True to enable colons, false to keep all digits normal.
		/// </value>
		public bool ColonsEnabled {
			set {
				if (value!=colonsEnabled) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Colons visibility can not be changed after SegmentDisplay is initialized");
						return;
					}
					colonsEnabled=value;
					if (colonsEnabled) {
						checkColonAtIndexArrayLength();
					}
				}
			}
			get {
				return colonsEnabled;
			}
		}

		private void checkColonAtIndexArrayLength() {
			if (ColonAtIndex==null) {
				ColonAtIndex=new bool[digitCount];
			} else if (ColonAtIndex.Length<digitCount) {
				bool[] oldArray=ColonAtIndex;
				ColonAtIndex=new bool[digitCount];
				Array.Copy(oldArray,ColonAtIndex,oldArray.Length);
			}
		}

		/// <summary>
		/// Array of boolean values that defines in which indexes there will be colon instead of normal digit.
		/// 
		/// This array is automatically created and set to correct length when ColonsEnabled is set to true, or when DigitCount is
		/// changed while colons are enabled. Before that, this array may be null or have invalid length.
		/// 
		/// Changing values in this array have no effect after display is created to the scene.
		/// </summary>
		public bool[] ColonAtIndex = null;


		[SerializeField]
		private bool smallerDigitsEnabled = false;

		/// <summary>
		/// Enable or disable some digits being smaller than another.
		/// </summary>
		/// <value>
		/// True to enable digits that are smaller than rest, false to keep all digits in same size.
		/// </value>
		public bool SmallerDigitsEnabled {
			set {
				if (value!=smallerDigitsEnabled) {
					if (isPlayingAndInitDone()) {
						Debug.LogError("Leguar.SegmentDisplay: Smaller digits setting can not be changed after SegmentDisplay is initialized");
						return;
					}
					smallerDigitsEnabled=value;
					if (smallerDigitsEnabled) {
						checkSmallerDigitAtIndexArrayLength();
					}
				}
			}
			get {
				return smallerDigitsEnabled;
			}
		}

		private void checkSmallerDigitAtIndexArrayLength() {
			if (SmallerDigitAtIndex==null) {
				SmallerDigitAtIndex=new bool[digitCount];
			} else if (SmallerDigitAtIndex.Length<digitCount) {
				bool[] oldArray=SmallerDigitAtIndex;
				SmallerDigitAtIndex=new bool[digitCount];
				Array.Copy(oldArray,SmallerDigitAtIndex,oldArray.Length);
			}
		}

		/// <summary>
		/// Array of boolean values that defines in which indexes there will be smaller digits instead of normal size digit.
		/// 
		/// This array is automatically created and set to correct length when SmallerDigitsEnabled is set to true, or when DigitCount is
		/// changed while smaller digits are enabled. Before that, this array may be null or have invalid length.
		/// 
		/// Changing values in this array have no effect after display is created to the scene.
		/// </summary>
		public bool[] SmallerDigitAtIndex = null;

		[SerializeField]
		private float smallerDigitsSizePercent = 0.5f;

		/// <summary>
		/// Set the size of smaller digits compared to normal size digits. Any value between 0.0 and 1.0 (exclusive) is accepted.
		/// For example, 1.0 would make smaller digits same size as normal, and 0.0 would make them invisibly small.
		/// </summary>
		/// <value>
		/// Percentage value defining size of smaller digits compared to normal size digits.
		/// </value>
		public float SmallerDigitsSizePercent {
			set {
				if (value<=0f) {
					Debug.LogError("Leguar.SegmentDisplay: SmallerDigitsSizePercent can't be 0 or less (trying to set "+value+")");
					return;
				}
				if (value>=1f) {
					Debug.LogError("Leguar.SegmentDisplay: SmallerDigitsSizePercent can't be 1 or more (trying to set "+value+")");
					return;
				}
				if (value!=smallerDigitsSizePercent) {
					smallerDigitsSizePercent=value;
					if (isPlayingAndInitDone()) {
						needRepositioning=true;
					}
				}
			}
			get {
				return smallerDigitsSizePercent;
			}
		}

		[SerializeField]
		private float smallerDigitsVerticalPosition = 0f;

		/// <summary>
		/// Set the vertical position of smaller digits compared to normal size digits. Any value between 0.0 and 1.0 (inclusive) is accepted.
		/// </summary>
		/// <value>
		/// Value defining vertical position of smaller digits compared to normal size digits, 0 being in bottom and 1 being in top.
		/// </value>
		public float SmallerDigitsVerticalPosition {
			set {
				if (value<0f) {
					Debug.LogError("Leguar.SegmentDisplay: SmallerDigitsVerticalPosition can't be less than 0 (trying to set "+value+")");
					return;
				}
				if (value>1f) {
					Debug.LogError("Leguar.SegmentDisplay: SmallerDigitsVerticalPosition can't be more than 1 (trying to set "+value+")");
					return;
				}
				if (value!=smallerDigitsVerticalPosition) {
					smallerDigitsVerticalPosition=value;
					if (isPlayingAndInitDone()) {
						needRepositioning=true;
					}
				}
			}
			get {
				return smallerDigitsVerticalPosition;
			}
		}


		[SerializeField]
		private float segmentSpread = 1f;
		
		/// <summary>
		/// Spread out segments or bring them closer inside one digit. 1.0 is default, positive value spreads segments away from each other, negative brings them closer to each other.
		/// </summary>
		/// <value>
		/// Percentage value defining how close or far away single digit segments are from each other.
		/// </value>
		public float SegmentSpread {
			set {
				if (value!=segmentSpread) {
					segmentSpread=value;
					if (isPlayingAndInitDone()) {
						needRepositioning=true;
					}
				}
			}
			get {
				return segmentSpread;
			}
		}

		[SerializeField]
		private float digitSpacing= 1f;

		/// <summary>
		/// Spacing between digits. 1.0 is default spacing, smaller value brings digits closer to each other and larger pushes digits away from each other.
		/// </summary>
		/// <value>
		/// Percentage value defining how close or far away digits are from each other.
		/// </value>
		public float DigitSpacing {
			set {
				if (value<0f) {
					Debug.LogError("Leguar.SegmentDisplay: DigitSpacing can't be set below 0 (trying to set "+value+")");
					return;
				}
				if (value!=digitSpacing) {
					digitSpacing=value;
					if (isPlayingAndInitDone()) {
						needRepositioning=true;
					}
				}
			}
			get {
				return digitSpacing;
			}
		}

		[SerializeField]
		private float colonRelativeSpacing = 0.5f;

		/// <summary>
		/// Spacing around colons, relative to digit spacing. 1.0 means no change to normal digit spacing, colon will take same amount of space as any other digit.
		/// 0.0 means no space for colons, digits around colon are as close each other as they would be without any colon between them.
		/// </summary>
		/// <value>
		/// Percentage value defining how much space is between colons compared to normal spacing between digits.
		/// </value>
		public float ColonRelativeSpacing {
			set {
				if (value!=colonRelativeSpacing) {
					colonRelativeSpacing=value;
					if (isPlayingAndInitDone()) {
						needRepositioning=true;
					}
				}
			}
			get {
				return colonRelativeSpacing;
			}
		}


		[SerializeField]
		private Color offColor = new Color(0.25f,0f,0f,1f);

		/// <summary>
		/// Color of segment when it is not lit. Note that color can be also partially or fully transparent.
		/// This color can be changed at any time also after display is created to scene. Colors will be updated in this or next update loop.
		/// </summary>
		/// <value>
		/// Color to be used for segments that are turned off.
		/// </value>
		public Color OffColor {
			set {
				if (value!=offColor) {
					offColor=value;
					if (Application.isPlaying) {
						needColorRefresh=true;
					}
				}
			}
			get {
				return offColor;
			}
		}

		[SerializeField]
		private Color onColor = new Color(1f,0f,0f,1f);

		/// <summary>
		/// Color of segment when it is lit. Note that color can be also transparency.
		/// This color can be changed at any time also after display is created to scene. Colors will be updated in this or next update loop.
		/// </summary>
		/// <value>
		/// Color to be used for segments that are turned on.
		/// </value>
		public Color OnColor {
			set {
				if (value!=onColor) {
					onColor=value;
					if (Application.isPlaying) {
						needColorRefresh=true;
					}
				}
			}
			get {
				return onColor;
			}
		}


		/// <summary>
		/// Text alignments.
		/// </summary>
		public enum Alignments {
			/// <summary>
			/// Align text to left side of display. First character of text will be in leftmost digit. If text is longer than display, end of the text will be cut out.
			/// </summary>
			Left,
			/// <summary>
			/// Align text to center of display. If text is longer than display, text may be cut out both in start and end. Note that center aligned text may not work
			/// correctly in displays that have colons enabled.
			/// </summary>
			Center,
			/// <summary>
			/// Align text to right side of display. Last character of text will be in rightmost digit. If text is longer than display, start of the text will be cut out.
			/// </summary>
			Right
		}

		[SerializeField]
		private Alignments defaultAlignment = Alignments.Left;

		/// <summary>
		/// Default alignment of text if not specified when calling SetText() or AddText() methods. This is also used in editor as test text alignment.
		/// </summary>
		/// <value>
		/// One of the choices from enum Alignments.
		/// </value>
		public Alignments DefaultAlignment {
			set {
				defaultAlignment=value;
			}
			get {
				return defaultAlignment;
			}
		}

		[SerializeField]
		private string initialText = "";

		/// <summary>
		/// Initial text that is added to display when display starts. Can be also empty string in which case display will be empty at start.
		/// </summary>
		/// <value>
		/// Text to show when scene and display starts.
		/// </value>
		public string InitialText {
			set {
				if (!value.Equals(initialText)) {
					if (isPlayingAndInitDone()) {
						Debug.LogWarning("Leguar.SegmentDisplay: Setting 'InitialText' have no effect after SegmentDisplay is initialized");
					}
					initialText=value;
				}
			}
			get {
				return initialText;
			}
		}


#if UNITY_EDITOR

		public enum EditorTestContents {
			Empty,
			Filled,
			Text
		}

		[SerializeField]
		private EditorTestContents editorTestContent = EditorTestContents.Text;

		public EditorTestContents EditorTestContent {
			set {
				if (value!=editorTestContent) {
					if (Application.isPlaying) {
						Debug.LogWarning("Leguar.SegmentDisplay: Setting 'EditorTestContent' when application is playing have no effect");
						return;
					}
					editorTestContent=value;
				}
			}
			get {
				return editorTestContent;
			}
		}

		[SerializeField]
		private string editorTestText = "0123456789ABCDEF";

		public string EditorTestText {
			set {
				if (!value.Equals(editorTestText)) {
					if (Application.isPlaying) {
						Debug.LogWarning("Leguar.SegmentDisplay: Setting 'EditorTestText' when application is playing have no effect");
					}
					editorTestText=value;
				}
			}
			get {
				return editorTestText;
			}
		}

		public bool EditorHelp = false;

		public bool needEditorReCreate;

#endif


		protected SingleDigit[] segmentDigits;

		protected bool needRepositioning;
        protected bool needSpriteOrderRefresh;
        private bool needColorRefresh;

		private SDController sdController;

		private bool runtimeInitDone = false;

		/// <summary>
		/// Gets reference to certain digit of this display. Useful if you want to control some digit separately.
		/// </summary>
		/// <param name="digitIndex">
		/// Digit index, 0 is leftmost digit.
		/// </param>
		public SingleDigit this[int digitIndex] {
			get {
				runtimeInit();
				return segmentDigits[digitIndex];
			}
		}

		void Start() {

#if UNITY_EDITOR

			if (!Application.isPlaying) {
				reCreateInEditMode();
				return;
			}

#endif

			runtimeInit();

		}

		void Update() {

#if UNITY_EDITOR

			if (!Application.isPlaying) {
				if (needEditorReCreate) {
					reCreateInEditMode();
				} else {
					try {
						if (this is SegmentDisplayImage) {
							setPositionAndSize();
						}
						doColorRefresh();
					}
					catch (Exception) {
						// May happen in some cases if editor created SegmentDisplay have lost references to digits or segments
						reCreateInEditMode();
					}
				}
				return;
			}

#endif

			if (needRepositioning) {
				setPositionAndSize();
				needRepositioning=false;
			}

            if (needSpriteOrderRefresh) {
                if (this is SegmentDisplaySprite) {
                    ((SegmentDisplaySprite)(this)).doSpriteOrderRefresh();
                }
                needSpriteOrderRefresh=false;
            }

            if (needColorRefresh) {
				doColorRefresh();
				needColorRefresh=false;
			}

			if (sdController!=null) {
				sdController.update(Time.deltaTime);
			}

		}

#if UNITY_EDITOR

		// This method is used only by custom editor and there is no need to call this manually.
		private void reCreateInEditMode() {

			if (Application.isPlaying) {
				Debug.LogWarning("Leguar.SegmentDisplay: Calling 'reCreateInEditMode' when application is playing have no effect");
				return;
			}

			Transform oldGroup=this.transform.Find(GENERATED_DIGIT_GROUP_NAME_EDITOR);
			if (oldGroup!=null) {
				DestroyImmediate(oldGroup.gameObject);
			}

			create(true);

			if (editorTestContent==EditorTestContents.Empty) {
				Clear();
			} else if (editorTestContent==EditorTestContents.Filled) {
				Fill();
			} else {
				SetText(editorTestText);
			}

			needEditorReCreate = false;

		}

#endif

		private void create(bool editorPreview) {
			GameObject groupObject=create(editorPreview ? GENERATED_DIGIT_GROUP_NAME_EDITOR : GENERATED_DIGIT_GROUP_NAME_RUNTIME);
			setLayersAndHideFlags(groupObject, this.gameObject.layer, editorPreview);
		}

		protected abstract GameObject create(string groupName);

		private void setLayersAndHideFlags(GameObject go, int layer, bool editorPreview) {
			go.layer=layer;
			if (editorPreview) {
				go.hideFlags=HideFlags.DontSave;
			}
			foreach (Transform child in go.transform) {
				setLayersAndHideFlags(child.gameObject, layer, editorPreview);
			}
		}

		protected void setPositionAndSize() {

			float digitWidth;
			if (DisplayType==DisplayTypes.SevenSegment) {
				digitWidth=2.4f;
				if (SevenSegmentStyle==SevenSegmentStyles.Basic || SevenSegmentStyle==SevenSegmentStyles.Round) {
					digitWidth+=0.25f;
				}
			} else {
				digitWidth=2.5f;
			}
			digitWidth*=SegmentSpread;
			
			float esWidth = (DecimalPointsEnabled || ApostrophesEnabled ? 0.2f : 0f);
			esWidth*=SegmentSpread;
			
			float spacing=0.85f*DigitSpacing;
			
			float digitHeight = 4.6f;
			if ((DisplayType == DisplayTypes.FourteenSegment && FourteenSegmentStyle == FourteenSegmentStyles.Basic) || DisplayType == DisplayTypes.SixteenSegment) {
				digitHeight=4.2f;
			}
			digitHeight*=SegmentSpread;

			float colonApparentZeroWidth = -spacing;
			float colonWidth = colonApparentZeroWidth + (digitWidth - colonApparentZeroWidth) * ColonRelativeSpacing;
			
			float x = 0f;
			float[] xs = new float[DigitCount];
			float[] ys = new float[DigitCount];
			float[] ms = new float[DigitCount];
			float totalWidth = 0f;
			
			for (int n=0; n<DigitCount; n++) {
				ms[n] = 1f;
				if (SmallerDigitsEnabled && SmallerDigitAtIndex[n]) {
					ms[n] *= SmallerDigitsSizePercent;
				}
				if (n>0) {
					x+=spacing*0.5f*ms[n];
					totalWidth+=spacing*0.5f*ms[n];
				}
				if (ColonsEnabled && ColonAtIndex[n]) {
					x+=colonWidth*0.5f*ms[n];
				} else {
					x+=digitWidth*0.5f*ms[n];
				}
				xs[n]=x;
				if (ColonsEnabled && ColonAtIndex[n]) {
					x+=colonWidth*0.5f*ms[n];
					totalWidth+=colonWidth*ms[n];
				} else {
					x+=(digitWidth*0.5f+esWidth)*ms[n];
					totalWidth+=(digitWidth+esWidth)*ms[n];
				}
				if (n<DigitCount-1) {
					x+=spacing*0.5f*ms[n];
					totalWidth+=spacing*0.5f*ms[n];
				}
				float verPosMax=digitHeight*0.5f-digitHeight*ms[n]*0.5f;
				ys[n] = -verPosMax + verPosMax * 2f * SmallerDigitsVerticalPosition;

			}

			totalWidth+=esWidth;

			setPositionAndSize(digitWidth,digitHeight,totalWidth,xs,ys,ms);

		}

		protected abstract void setPositionAndSize(float digitWidth, float digitHeight, float totalWidth, float[] xs, float[] ys, float[] ms);

        private void doColorRefresh() {
			foreach (SingleDigit segmentDigit in segmentDigits) {
				segmentDigit.doColorRefresh();
			}
		}

		/// <summary>
		/// Clear the display. This will turn all segments of all digits to off state.
		/// </summary>
		public void Clear() {
			runtimeInit();
			foreach (SingleDigit segmentDigit in segmentDigits) {
				segmentDigit.Clear();
			}
		}

		/// <summary>
		/// Clear part of the display. This will turn all segments of selected digits to off state.
		/// </summary>
		/// <param name="startIndex">
		/// Index of first digit to clear.
		/// </param>
		/// <param name="count">
		/// How many digits to clear.
		/// </param>
		public void Clear(int startIndex, int count) {
			runtimeInit();
			for (int n=0; n<count; n++) {
				segmentDigits[startIndex+n].Clear();
			}
		}

		/// <summary>
		/// Fill the display. This will turn all segments of all digits to on state.
		/// </summary>
		public void Fill() {
			runtimeInit();
			foreach (SingleDigit segmentDigit in segmentDigits) {
				segmentDigit.Fill();
			}
		}

		/// <summary>
		/// Fill part of the display. This will turn all segments of selected digits to on state.
		/// </summary>
		/// <param name="startIndex">
		/// Index of first digit to fill.
		/// </param>
		/// <param name="count">
		/// How many digits to fill.
		/// </param>
		public void Fill(int startIndex, int count) {
			runtimeInit();
			for (int n=0; n<count; n++) {
				segmentDigits[startIndex+n].Fill();
			}
		}

		/// <summary>
		/// Check whatever display is completely empty.
		/// </summary>
		/// <returns>
		/// True if all segments are not lit, false otherwise.
		/// </returns>
		public bool IsEmpty() {
			runtimeInit();
			foreach (SingleDigit segmentDigit in segmentDigits) {
				if (!segmentDigit.IsEmpty()) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Check whatever display is completely filled.
		/// </summary>
		/// <returns>
		/// True if all segments are lit, false otherwise.
		/// </returns>
		public bool IsFull() {
			runtimeInit();
			foreach (SingleDigit segmentDigit in segmentDigits) {
				if (!segmentDigit.IsFull()) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Move all the content in display to one digit left. Content of leftmost digit will be lost and rightmost digit is set empty.
		/// Note that this will not work correctly in display that have colons enabled since content can't be copied between normal digit and colon.
		/// </summary>
		public void MoveLeft() {
			runtimeInit();
			for (int n=0; n<digitCount-1; n++) {
				segmentDigits[n].copyState(segmentDigits[n+1]);
			}
			segmentDigits[digitCount-1].Clear();
		}

		/// <summary>
		/// Move all the content in display to one digit right. Content of rightmost digit will be lost and lefttmost digit is set empty.
		/// Note that this will not work correctly in display that have colons enabled since content can't be copied between normal digit and colon.
		/// </summary>
		public void MoveRight() {
			runtimeInit();
			for (int n=digitCount-1; n>0; n--) {
				segmentDigits[n].copyState(segmentDigits[n-1]);
			}
			segmentDigits[0].Clear();
		}

		/// <summary>
		/// Set text to this display using default text alignment.
		/// 
		/// All the digits in display that this text is not using, are set to off state. Meaning that display will be cleared from all previous content it may had.
		/// </summary>
		/// <param name="text">
		/// Text to set.
		/// </param>
		public void SetText(string text) {
			SetText(text,defaultAlignment,0);
		}

		/// <summary>
		/// Set text to this display using specified text alignment.
		/// 
		/// All the digits in display that this text is not using, are set to off state. Meaning that display will be cleared from all previous content it may had.
		/// </summary>
		/// <param name="text">
		/// Text to set.
		/// </param>
		/// <param name="alignment">
		/// Alignment of the text.
		/// </param>
		public void SetText(string text, Alignments alignment) {
			SetText(text,alignment,0);
		}

		/// <summary>
		/// Set text to this display using specified text alignment and possible offset.
		/// 
		/// All the digits in display that this text is not using, are set to off state. Meaning that display will be cleared from all previous content it may had.
		/// </summary>
		/// <param name="text">
		/// Text to set.
		/// </param>
		/// <param name="alignment">
		/// Alignment of the text.
		/// </param>
		/// <param name="offset">
		/// Offset of the text. This will move text certain amount of digits to wanted direction. For example, when using left alignment and offset 2,
		/// text will start from third digit, leaving two first digits empty. If using left alignment and offset -1, first character of text will be ignored
		/// (it would be outside the display) and second character of text will be in first digit of display.
		/// </param>
		public void SetText(string text, Alignments alignment, int offset) {
			setOrAddText(text,alignment,offset,true);
		}

		/// <summary>
		/// Add text to this display using default text alignment.
		/// 
		/// All the digits in display that this text is not using are not changed. Meaning previous content will stay on display if this new text is not written over it.
		/// </summary>
		/// <param name="text">
		/// Text to add.
		/// </param>
		public void AddText(string text) {
			AddText(text,defaultAlignment,0);
		}

		/// <summary>
		/// Add text to this display using specified text alignment.
		/// 
		/// All the digits in display that this text is not using are not changed. Meaning previous content will stay on display if this new text is not written over it.
		/// </summary>
		/// <param name="text">
		/// Text to add.
		/// </param>
		/// <param name="alignment">
		/// Alignment of new text.
		/// </param>
		public void AddText(string text, Alignments alignment) {
			AddText(text,alignment,0);
		}

		/// <summary>
		/// Add text to this display using specified text alignment and possible offset.
		/// 
		/// All the digits in display that this text is not using are not changed. Meaning previous content will stay on display if this new text is not written over it.
		/// </summary>
		/// <param name="text">
		/// Text to add.
		/// </param>
		/// <param name="alignment">
		/// Alignment of new text.
		/// </param>
		/// <param name="offset">
		/// Offset of new text. This will move text certain amount of digits to wanted direction. For example, when using left alignment and offset 2,
		/// text will start from third digit, leaving two first digits empty. If using left alignment and offset -1, first character of text will be ignored
		/// (it would be outside the display) and second character of text will be in first digit of display.
		/// </param>
		public void AddText(string text, Alignments alignment, int offset) {
			setOrAddText(text,alignment,offset,false);
		}

		private void setOrAddText(string text, Alignments alignment, int offset, bool clearUnused) {

			runtimeInit();

			if (alignment==Alignments.Left || alignment==Alignments.Center) {

				int textIndex=-offset;
				if (alignment==Alignments.Center) {
					int textLength=text.Length;
					if (decimalPointsEnabled || apostrophesEnabled) {
						int approxTextLength=0;
						bool lastWasES=false;
						for (int n=0; n<textLength; n++) {
							if ( (decimalPointsEnabled && isDecimalPointCharacter(text[n])) || (apostrophesEnabled && isApostropheCharacter(text[n])) ) {
								if (lastWasES) {
									approxTextLength++;
								} else {
									lastWasES=true;
								}
							} else {
								approxTextLength++;
								lastWasES=false;
							}
						}
						textLength=approxTextLength;
					}
					textIndex+=(textLength/2)-(digitCount/2);
				}

				for (int n=0; n<digitCount; n++) {
					if (textIndex>=0 && textIndex<text.Length) {
						char chr=text[textIndex];
						if ((!decimalPointsEnabled || !isDecimalPointCharacter(chr)) && (!apostrophesEnabled || !isApostropheCharacter(chr))) {
							if (segmentDigits[n].SetChar(chr)) {
								textIndex++;
							} else if (!segmentDigits[n].IsColon()) {
								textIndex++;
							}
						}
					} else {
						if (clearUnused) {
							segmentDigits[n].Clear();
						}
						textIndex++;
					}
					if (textIndex>=0 && textIndex<text.Length) {
						char chr=text[textIndex];
						if (isDecimalPointCharacter(chr)) {
							if (segmentDigits[n].IsDecimalPointEnabled()) {
								segmentDigits[n].SetDecimalPointState(true);
								textIndex++;
							}
						} else if (isApostropheCharacter(chr)) {
							if (segmentDigits[n].IsApostropheEnabled()) {
								segmentDigits[n].SetApostropheState(true);
								textIndex++;
							}
						}
					}
				}

			} else {

				int textIndex=text.Length-1-offset;
				for (int n=digitCount-1; n>=0; n--) {
					bool setDecimalPoint=false;
					bool setApostrophe=false;
					if (textIndex>=0 && textIndex<text.Length) {
						char chr = text[textIndex];
						if (segmentDigits[n].IsDecimalPointEnabled() && isDecimalPointCharacter(chr)) {
							setDecimalPoint=true;
							textIndex--;
						} else if (segmentDigits[n].IsApostropheEnabled() && isApostropheCharacter(chr)) {
							setApostrophe=true;
							textIndex--;
						}
					}
					if (textIndex>=0 && textIndex<text.Length) {
						char chr=text[textIndex];
						if ((!decimalPointsEnabled || !isDecimalPointCharacter(chr)) && (!apostrophesEnabled || !isApostropheCharacter(chr))) {
							if (segmentDigits[n].SetChar(chr)) {
								textIndex--;
							} else if (!segmentDigits[n].IsColon()) {
								textIndex--;
							}
						}
					} else {
						if (clearUnused) {
							segmentDigits[n].Clear();
						}
						textIndex--;
					}
					if (setDecimalPoint) {
						segmentDigits[n].SetDecimalPointState(true);
					}
					if (setApostrophe) {
						segmentDigits[n].SetApostropheState(true);
					}
				}

			}

		}

		internal static bool isDecimalPointCharacter(char chr) {
			return (chr=='.' || chr==',');
		}

		internal static bool isApostropheCharacter(char chr) {
			return (chr=='\'' || chr==180 || chr==96 || chr==8217);
		}

		/// <summary>
		/// Gets the SegmentDisplay Controller instance for this display. Note that you do not necessarily need this. If you just want to add text to display, you can
		/// use SetText() methods in this class. SDController is useful if you wish to run series of text changes or for example scroll text on display.
		/// </summary>
		/// <returns>
		/// SDController instance. Each display have only one controller instance, same one is returned every time this method is called.
		/// </returns>
		public SDController GetSDController() {
			runtimeInit();
			if (sdController==null) {
				sdController=new SDController(this);
			}
			return sdController;
		}

		private void runtimeInit() {
			
			if (!Application.isPlaying || runtimeInitDone) {
				return;
			}
			
			Transform editorPreview=this.transform.Find(GENERATED_DIGIT_GROUP_NAME_EDITOR);
			if (editorPreview!=null) {
				Destroy(editorPreview.gameObject);
			}

			create(false);
			runtimeInitDone=true;
			SetText(initialText);
			
            needRepositioning=false;
            needSpriteOrderRefresh=false;
			needColorRefresh=false;
			
		}
		
		protected bool isPlayingAndInitDone() {
			return (Application.isPlaying && runtimeInitDone);
		}

	}

}
