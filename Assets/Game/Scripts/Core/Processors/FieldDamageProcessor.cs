namespace Game.Core.Processors
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using Game.Field;
	using Game.Units;
	using System.Linq;

	public class FieldDamageProcessor : ControllerBase, IInitializable
	{
		[Inject] private IBattleEvents		_battleEvents;
		[Inject] private IFieldFacade[]		_fields;

		const float MaxScaleInitValue		= 1f;
		const float MinScaleTargetValue		= 0.4f;
		const float MaxScaleTargetValue		= 0.9f;

		public void Initialize()
		{
			_battleEvents.DamageApplyed
				.Where( d => d.Type == EDamageType.Aoe )
				.Subscribe( OnAoeDamageApplyed )
				.AddTo( this );
		}

		private void OnAoeDamageApplyed( DamageData data )
		{
			var field = _fields.Where( f => f.HasUnit(data.Target) ).FirstOrDefault();

			if (field == null)
				return;

			var targets = field.Units
				.Select( u => (Unit : u, Distance : Vector3.Distance( u.Transform.position, data.Position )) )
				.Where( r => r.Distance < data.Range )
				.Select( r => ( r.Unit, Damage: CalcAoeDamage( data, r.Distance )) )
				.ToList();

			targets.ForEach( t => t.Unit.TakeDamage( t.Damage ) );
		}

		private float CalcAoeDamage( DamageData data, float distance )
		{
			float distanceRatio		= distance / data.Range;
			float multiplier		= 1 - Normalize(distanceRatio, 0, MaxScaleInitValue, MinScaleTargetValue, MaxScaleTargetValue);

			return multiplier * data.Amount;
		}

		private IFieldFacade GetAllyField( IUnitFacade unit )
		{
			for (int i = 0; i < _fields.Length; i++)
				if (_fields[i].HasUnit(unit))
					return _fields[i];
			
			return null;
		}

		private float Normalize(float val, float valmin, float valmax, float min, float max) =>
			(((val - valmin) / (valmax - valmin)) * (max - min)) + min;

	}
}
