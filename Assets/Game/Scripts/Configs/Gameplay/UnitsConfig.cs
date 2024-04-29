namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using DG.Tweening;
	using Sirenix.OdinInspector;


	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : SerializedScriptableObject
	{
		[Header("Visual")]
		public GameObject		UnitPrefab;
		public float			UiHealthIndent;
		public Sprite[]			GradeSprites;

		[FoldoutGroup("Damage Fx")]
		public DamageNumberFx		DamageFxPrefab;
		[FoldoutGroup("Damage Fx")]
		public int					DamageFxPoolSize;
		[FoldoutGroup("Damage Fx")]
		public Color				DamageFxHeroUnitColor;
		[FoldoutGroup("Damage Fx")]
		public Color				DamageFxEnemyUnitColor;
		[FoldoutGroup("Damage Fx")]
		public float				DamageFxSpawnOffset;
		[FoldoutGroup("Damage Fx")]
		public float				DamageFxHeight;
		[FoldoutGroup("Damage Fx")]
		public float				DamageFxDuration;
		[FoldoutGroup("Damage Fx")]
		public Ease					DamageFxEase;
		[FoldoutGroup("Damage Fx")]
		public Color				DamageFxMaterialColor;

		[Header("Gameplay")]
		public float		Speed;

		[Tooltip("Seconds to full rotate (360gr)")]
		public float			RotationSpeed;

		public Dictionary<Species, UnitConfig>		Units;

		[Header("Summon")]
		public float			SummonCountRatioLimit;

		[Header("Hero")]
		public List<Species>	HeroUnits;

		[Header("Merge")]
		public int[]			AdditionalMergeGradePower;

		public List<Species>	HeroDefaultSquad;

		
		public int GetAdditionalPower(int gradeIndex)
		{
			if (gradeIndex >= AdditionalMergeGradePower.Length)
				return 0;

			return AdditionalMergeGradePower[gradeIndex];
		}
    }
}