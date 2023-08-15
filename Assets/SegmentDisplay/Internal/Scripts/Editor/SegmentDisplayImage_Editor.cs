//    SegmentDisplayImage - Custom editor


using UnityEngine;
using UnityEditor;
using System;

namespace Leguar.SegmentDisplay {

	[CustomEditor(typeof(SegmentDisplayImage))]
	public class SegmentDisplayImage_Editor : SegmentDisplay_Editor {

		void OnEnable() {
			
			SegmentDisplayImage sdTarget=(SegmentDisplayImage)(target);

			// Possibly automatically add to Canvas
			if (shouldCheckAndSetParent(sdTarget)) {
				sdTarget.CheckAndSetParent();
			}
			
		}

		private bool shouldCheckAndSetParent(SegmentDisplayImage displayTarget) {
			if (Application.isPlaying) {
				return false;
			}
#if UNITY_2018_3_OR_NEWER
			return (PrefabUtility.GetPrefabAssetType(displayTarget)==PrefabAssetType.Regular && PrefabUtility.GetPrefabInstanceStatus(displayTarget)==PrefabInstanceStatus.Connected);
#else
			return (PrefabUtility.GetPrefabType(displayTarget)==PrefabType.PrefabInstance);
#endif
		}

		public override void OnInspectorGUI() {

			Undo.RecordObject(target,"SegmentDisplay_UI Change");

			SegmentDisplayImage sdTarget=(SegmentDisplayImage)(target);
			bool changes=false;

			base.mainEditor(sdTarget,ref changes);

/*
			SegmentDisplay_Editor.addHeader("Development");
			sdTarget.sevenSegmentBasicDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit basic prefab",sdTarget.sevenSegmentBasicDigitImagePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentClassicDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit classic prefab",sdTarget.sevenSegmentClassicDigitImagePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentSharpDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit sharp prefab",sdTarget.sevenSegmentSharpDigitImagePrefab,typeof(GameObject),false));
			sdTarget.sevenSegmentRoundDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("7 segment digit round prefab",sdTarget.sevenSegmentRoundDigitImagePrefab,typeof(GameObject),false));
			sdTarget.fourteenSegmentBasicDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("14 segment digit basic prefab",sdTarget.fourteenSegmentBasicDigitImagePrefab,typeof(GameObject),false));
			sdTarget.fourteenSegmentCheapDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("14 segment digit cheap prefab",sdTarget.fourteenSegmentCheapDigitImagePrefab,typeof(GameObject),false));
			sdTarget.sixteenSegmentBasicDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("16 segment digit basic prefab",sdTarget.sixteenSegmentBasicDigitImagePrefab,typeof(GameObject),false));
			sdTarget.sixteenSegmentMiniLedsDigitImagePrefab=(GameObject)(EditorGUILayout.ObjectField("16 segment digit minileds prefab",sdTarget.sixteenSegmentMiniLedsDigitImagePrefab,typeof(GameObject),false));
*/

		}

	}

}
