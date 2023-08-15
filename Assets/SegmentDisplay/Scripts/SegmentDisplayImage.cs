//    Image based SegmentDisplay for UI


using UnityEngine;
using UnityEngine.UI;

namespace Leguar.SegmentDisplay {

	/// <summary>
	/// Image based SegmentDisplay to be used in UI.
	/// </summary>
	public class SegmentDisplayImage : SegmentDisplay {

		/// <summary>
		/// List of positions of SegmentDisplay inside UI RectTransform.
		/// </summary>
		public enum UIPositions {
			/// <summary>Fill whole rect, without keeping aspect ratio.</summary>
			FillIgnoringAspectRatio,
			/// <summary>Set segment display to upper left corner of RectTransform.</summary>
			UpperLeft,
			/// <summary>Set segment display to upper center position of RectTransform.</summary>
			UpperCenter,
			/// <summary>Set segment display to upper right corner of RectTransform.</summary>
			UpperRight,
			/// <summary>Set segment display to middle left position of RectTransform.</summary>
			MiddleLeft,
			/// <summary>Set segment display to middle center position of RectTransform.</summary>
			MiddleCenter,
			/// <summary>Set segment display to middle right position of RectTransform.</summary>
			MiddleRight,
			/// <summary>Set segment display to lower left corner of RectTransform.</summary>
			LowerLeft,
			/// <summary>Set segment display to lower center position of RectTransform.</summary>
			LowerCenter,
			/// <summary>Set segment display to lower right corner of RectTransform.</summary>
			LowerRight
		}
		
		[SerializeField]
		private UIPositions uiPosition=UIPositions.MiddleCenter;
		
		/// <summary>
		/// Sets or gets position of the segment display inside RectTransform.
		/// </summary>
		/// <value>
		/// One of the choices from enum UIPositions.
		/// </value>
		public UIPositions UIPosition {
			set {
				if (value!=uiPosition) {
					uiPosition=value;
					if (base.isPlayingAndInitDone()) {
						base.needRepositioning=true;
					}
				}
			}
			get {
				return uiPosition;
			}
		}

		public GameObject sevenSegmentBasicDigitImagePrefab;
		public GameObject sevenSegmentClassicDigitImagePrefab;
		public GameObject sevenSegmentSharpDigitImagePrefab;
		public GameObject sevenSegmentRoundDigitImagePrefab;
		public GameObject fourteenSegmentBasicDigitImagePrefab;
		public GameObject fourteenSegmentCheapDigitImagePrefab;
		public GameObject sixteenSegmentBasicDigitImagePrefab;
		public GameObject sixteenSegmentMiniLedsDigitImagePrefab;

		private RectTransform groupTransform;

		protected override GameObject create(string groupName) {

			GameObject groupObject=new GameObject(groupName);
			groupTransform=groupObject.AddComponent<RectTransform>();
			groupTransform.SetParent(this.transform);
			groupTransform.localPosition=Vector3.zero;
			groupTransform.localRotation=Quaternion.identity;
			groupTransform.localScale=Vector3.one;
			groupTransform.anchoredPosition3D=Vector3.zero;
			groupTransform.sizeDelta=Vector2.zero;

			base.segmentDigits=new SingleDigitImage[DigitCount];
			for (int n=0; n<DigitCount; n++) {
				GameObject digitObject;
				if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(sevenSegmentBasicDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Classic) {
					digitObject=(GameObject)(Instantiate(sevenSegmentClassicDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Sharp) {
					digitObject=(GameObject)(Instantiate(sevenSegmentSharpDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Round) {
					digitObject=(GameObject)(Instantiate(sevenSegmentRoundDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.FourteenSegment && FourteenSegmentStyle==FourteenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(fourteenSegmentBasicDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.FourteenSegment && FourteenSegmentStyle==FourteenSegmentStyles.Cheap) {
					digitObject=(GameObject)(Instantiate(fourteenSegmentCheapDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.SixteenSegment && SixteenSegmentStyle==SixteenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(sixteenSegmentBasicDigitImagePrefab));
				} else if (DisplayType==DisplayTypes.SixteenSegment && SixteenSegmentStyle==SixteenSegmentStyles.MiniLeds) {
					digitObject=(GameObject)(Instantiate(sixteenSegmentMiniLedsDigitImagePrefab));
				} else {
					Debug.LogError("Leguar.SegmentDisplay: SegmentDisplayImage.create(...): Internal error, unknown display type or style");
					return null;
				}
				digitObject.name="SegmentDisplay_Digit_"+n;
				digitObject.transform.SetParent(groupTransform);
				base.segmentDigits[n]=digitObject.GetComponent<SingleDigitImage>();
				base.segmentDigits[n].init(this, ((base.ColonsEnabled && base.ColonAtIndex[n]) ? SingleDigit.Mode.Colon : SingleDigit.Mode.Digit), base.DecimalPointsEnabled, base.ApostrophesEnabled);
			}
			
			base.setPositionAndSize();

			return groupObject;

		}

		protected override void setPositionAndSize(float digitWidth, float digitHeight, float totalWidth, float[] xs, float[] ys, float[] ms) {

			RectTransform rectTransform = this.GetComponent<RectTransform>();

			float sizeMultipHor = rectTransform.rect.width / totalWidth;
			float sizeMultipVer = rectTransform.rect.height / digitHeight;

			if (uiPosition!=UIPositions.FillIgnoringAspectRatio) {
				float x = 0f;
				float y = 0f;
				if (sizeMultipHor<sizeMultipVer) {
					sizeMultipVer=sizeMultipHor;
					float dh = digitHeight * sizeMultipVer;
					if (uiPosition==UIPositions.LowerLeft || uiPosition==UIPositions.LowerCenter || uiPosition==UIPositions.LowerRight) {
						y=-(rectTransform.rect.height-dh)*0.5f;
					}
					if (uiPosition==UIPositions.UpperLeft || uiPosition==UIPositions.UpperCenter || uiPosition==UIPositions.UpperRight) {
						y=(rectTransform.rect.height-dh)*0.5f;
					}
				} else if (sizeMultipVer<sizeMultipHor) {
					sizeMultipHor=sizeMultipVer;
					float dtw = totalWidth * sizeMultipHor;
					if (uiPosition==UIPositions.LowerLeft || uiPosition==UIPositions.MiddleLeft || uiPosition==UIPositions.UpperLeft) {
						x=-(rectTransform.rect.width-dtw)*0.5f;
					}
					if (uiPosition==UIPositions.LowerRight || uiPosition==UIPositions.MiddleRight || uiPosition==UIPositions.UpperRight) {
						x=(rectTransform.rect.width-dtw)*0.5f;
					}
				}
				groupTransform.anchoredPosition3D = new Vector3(x,y,0f);
			} else {
				groupTransform.anchoredPosition3D = Vector3.zero;
			}

			for (int n=0; n<DigitCount; n++) {
				GameObject digitObject=base.segmentDigits[n].gameObject;
				digitObject.transform.localPosition=new Vector3((-totalWidth*0.5f+xs[n])*sizeMultipHor,ys[n]*sizeMultipVer,0f);
				digitObject.transform.localRotation=Quaternion.identity;
				digitObject.transform.localScale=Vector3.one;
				((SingleDigitImage)(base.segmentDigits[n])).spreadAndMultiplySize(SegmentSpread,sizeMultipHor*ms[n],sizeMultipVer*ms[n]);
			}

		}

		/// <summary>
		/// Check whatever this segment display object is child of UI Canvas and tries to set the parent if not.
		/// If there is no canvas in scene hierarchy, this method will create one.
		/// If this object parent is changed, object's scale and anchored position will reset also.
		/// 
		/// Note that this method is normally used only internally in Unity Editor and there is no need to call this when application is running.
		/// If you create new UI-type SegmentDisplay from prefab runtime, rather make sure your scene already contains Canvas and then use normal
		/// myUISegmentDisplay.transform.SetParent(myKnownCanvasOrOtherUIElement.transform) method to set parent like with any other UI object.
		/// </summary>
		public void CheckAndSetParent() {
			if (this.GetComponentInParent<Canvas>()==null) {
				Canvas canvas=getOrCreateCanvas();
				this.transform.SetParent(canvas.transform,false);
				this.transform.localScale=Vector3.one;
				this.GetComponent<RectTransform>().anchoredPosition3D=Vector3.zero;
			}
		}
		
		private Canvas getOrCreateCanvas() {
			Canvas canvas=Transform.FindObjectOfType<Canvas>();
			if (canvas==null) {
				GameObject canvasObject=new GameObject("Canvas");
				canvas=canvasObject.AddComponent<Canvas>();
				canvas.renderMode=RenderMode.ScreenSpaceOverlay;
				canvasObject.AddComponent<CanvasScaler>();
				canvasObject.AddComponent<GraphicRaycaster>();
				canvasObject.layer=this.gameObject.layer; // UI
			}
			return canvas;
		}

		void LateUpdate() {
			if (base.isPlayingAndInitDone()) {
				if (this.transform.hasChanged) {
					base.setPositionAndSize();
					this.transform.hasChanged=false;
				}
			}
		}

	}

}
