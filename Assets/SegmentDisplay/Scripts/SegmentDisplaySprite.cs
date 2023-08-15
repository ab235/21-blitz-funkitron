//    Sprite based SegmentDisplay for game world


using UnityEngine;

namespace Leguar.SegmentDisplay {

	/// <summary>
	/// Sprite based SegmentDisplay to be used in 2D/3D world space.
	/// </summary>
	public class SegmentDisplaySprite : SegmentDisplay {

		public GameObject sevenSegmentBasicDigitSpritePrefab;
		public GameObject sevenSegmentClassicDigitSpritePrefab;
		public GameObject sevenSegmentSharpDigitSpritePrefab;
		public GameObject sevenSegmentRoundDigitSpritePrefab;
		public GameObject fourteenSegmentBasicDigitSpritePrefab;
		public GameObject fourteenSegmentCheapDigitSpritePrefab;
		public GameObject sixteenSegmentBasicDigitSpritePrefab;
		public GameObject sixteenSegmentMiniLedsDigitSpritePrefab;

		[SerializeField]
		private int spriteSortingOrder=0;
		
		/// <summary>
		/// "Order in Layer" of all SpriteRenderers this display is using is set to this value. This is important especially in 2D games if it is possible that this
		/// display will overlap with other sprites in scene. Change this value to choose should this display be in front or back of these other sprites.
		/// </summary>
		/// <value>
		/// Value to be used as sorting order for all sprites used by this SegmentDisplay.
		/// </value>
		public int SpriteSortingOrder {
			set {
				if (value!=spriteSortingOrder) {
					spriteSortingOrder=value;
					if (base.isPlayingAndInitDone()) {
						base.needSpriteOrderRefresh=true;
					}
				}
			}
			get {
				return spriteSortingOrder;
			}
		}

		protected override GameObject create(string groupName) {

			GameObject groupObject=new GameObject(groupName);
			Transform groupTransform=groupObject.transform;
			groupTransform.parent=this.transform;
			groupTransform.localPosition=Vector3.zero;
			groupTransform.localRotation=Quaternion.identity;
			groupTransform.localScale=Vector3.one;
			
			base.segmentDigits=new SingleDigit[DigitCount];
			for (int n=0; n<DigitCount; n++) {
				GameObject digitObject;
				if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(sevenSegmentBasicDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Classic) {
					digitObject=(GameObject)(Instantiate(sevenSegmentClassicDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Sharp) {
					digitObject=(GameObject)(Instantiate(sevenSegmentSharpDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.SevenSegment && SevenSegmentStyle==SevenSegmentStyles.Round) {
					digitObject=(GameObject)(Instantiate(sevenSegmentRoundDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.FourteenSegment && FourteenSegmentStyle==FourteenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(fourteenSegmentBasicDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.FourteenSegment && FourteenSegmentStyle==FourteenSegmentStyles.Cheap) {
					digitObject=(GameObject)(Instantiate(fourteenSegmentCheapDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.SixteenSegment && SixteenSegmentStyle==SixteenSegmentStyles.Basic) {
					digitObject=(GameObject)(Instantiate(sixteenSegmentBasicDigitSpritePrefab));
				} else if (DisplayType==DisplayTypes.SixteenSegment && SixteenSegmentStyle==SixteenSegmentStyles.MiniLeds) {
					digitObject=(GameObject)(Instantiate(sixteenSegmentMiniLedsDigitSpritePrefab));
				} else {
					Debug.LogError("Leguar.SegmentDisplay: SegmentDisplaySprite.create(...): Internal error, unknown display type or style");
					return null;
				}
				digitObject.name="SegmentDisplay_Digit_"+n;
				digitObject.transform.parent=groupTransform;
				base.segmentDigits[n]=digitObject.GetComponent<SingleDigit>();
				base.segmentDigits[n].init(this, ((base.ColonsEnabled && base.ColonAtIndex[n]) ? SingleDigit.Mode.Colon : SingleDigit.Mode.Digit), base.DecimalPointsEnabled, base.ApostrophesEnabled);
			}
			
			base.setPositionAndSize();

			return groupObject;

		}

		protected override void setPositionAndSize(float digitWidth, float digitHeight, float totalWidth, float[] xs, float[] ys, float[] ms) {

			for (int n=0; n<DigitCount; n++) {
				GameObject digitObject=base.segmentDigits[n].gameObject;
				digitObject.transform.localPosition=new Vector3(-totalWidth*0.5f+xs[n],ys[n],0f);
				digitObject.transform.localRotation=Quaternion.identity;
				digitObject.transform.localScale=Vector3.one*ms[n];
				((SingleDigitSprite)(base.segmentDigits[n])).spread(SegmentSpread);
			}
			
		}

        internal void doSpriteOrderRefresh() {
            foreach (SingleDigit segmentDigit in segmentDigits) {
                ((SingleDigitSprite)(segmentDigit)).doSpriteOrderRefresh();
            }
        }

    }

}
