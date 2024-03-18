namespace Game.Ui
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	[Serializable]
	public struct ImageAlertInput
	{
		public Image Image;
		public Color Color;
		public float Duration;
		public int Repeats;
	}
}
