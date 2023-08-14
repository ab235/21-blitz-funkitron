using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class WebGLLoader : MonoBehaviour {

	private void Awake() {
#if UNITY_WEBGL
		// This line creates a CommandBuffer and clears the render target to the specified color
		Vector2 fullhd = new Vector2(1080, 1920);
		float ratio = fullhd.y / fullhd.x;
		Screen.SetResolution((int)(Screen.height / ratio), Screen.height, false);

		// This line creates a CommandBuffer and clears the render target to the specified color
		CommandBuffer mybuffer = new CommandBuffer();
		mybuffer.ClearRenderTarget(true, true, Color.white, 1.0f);
		Graphics.ExecuteCommandBuffer(mybuffer);
#endif

		// This line loads scene 1 asynchronously
		SceneManager.LoadSceneAsync(1);
	}
}
