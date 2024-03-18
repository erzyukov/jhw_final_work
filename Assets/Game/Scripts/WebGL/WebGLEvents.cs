namespace Game
{
	using Game.Core;
	using System.Runtime.InteropServices;
	using UniRx;
	using UnityEngine;

	public class WebGLEvents : MonoBehaviour, IApplicationPaused
	{
		[DllImport( "__Internal" )]
		private static extern void RegisterVisibilityChangeEvent();

		private void Awake()
		{
			transform.SetParent( null );
			DontDestroyOnLoad( this );
		}

		private void Start()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			RegisterVisibilityChangeEvent();
#endif
		}

		public void OnVisibilityChange( string visibilityState )
		{
			IsApplicationPaused.Value = visibilityState == "visible";
		}

		#region IApplicationPaused

		public BoolReactiveProperty IsApplicationPaused { get; } = new BoolReactiveProperty();

		#endregion
	}
}