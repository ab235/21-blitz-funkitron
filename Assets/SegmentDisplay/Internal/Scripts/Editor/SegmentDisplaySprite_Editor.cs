//    SegmentDisplaySprite - Custom editor


using UnityEngine;
using UnityEditor;
using System;

namespace Leguar.SegmentDisplay {

	[CustomEditor(typeof(SegmentDisplaySprite))]
	public class SegmentDisplaySprite_Editor : SegmentDisplay_Editor {

		public override void OnInspectorGUI() {

			Undo.RecordObject(target,"SegmentDisplay_World Change");

			SegmentDisplaySprite sdTarget=(SegmentDisplaySprite)(target);
			bool changes=false;

			base.mainEditor(sdTarget,ref changes);

/*
			SegmentDisplay_Editor.addHeader("Development");
			sdTarget.sevenSegmentBasicDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit basic prefab",sdTarget.sevenSegmentBasicDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentClassicDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit classic prefab",sdTarget.sevenSegmentClassicDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentSharpDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit sharp prefab",sdTarget.sevenSegmentSharpDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentRoundDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit round prefab",sdTarget.sevenSegmentRoundDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.fourteenSegmentBasicDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("14 segment digit basic prefab",sdTarget.fourteenSegmentBasicDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.fourteenSegmentCheapDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("14 segment digit cheap prefab",sdTarget.fourteenSegmentCheapDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.sixteenSegmentBasicDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("16 segment digit basic prefab",sdTarget.sixteenSegmentBasicDigitSpritePrefab,typeof(GameObject),false));
			sdTarget.sixteenSegmentMiniLedsDigitSpritePrefab=(GameObject)(EditorGUILayout.ObjectField("16 segment digit minileds prefab",sdTarget.sixteenSegmentMiniLedsDigitSpritePrefab,typeof(GameObject),false));
*/

		}

	}

}
