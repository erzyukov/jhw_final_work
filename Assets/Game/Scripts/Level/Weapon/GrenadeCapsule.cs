namespace Game.Weapon
{
	using UnityEngine;
	using Zenject;

	public class GrenadeCapsule : Projectile
	{
		private float _time;
		private float _elapse;
		private Vector3 _startPosition;
		private float _height;
		private float _distance;
		private Vector3 _prevPosition;
		private bool _activated;

		protected override void Move()
		{
			float t = _elapse / _time;

			Vector3 hPosition = Vector3.Lerp(_startPosition, TargetPosition, t);
			Vector3 vPosition = Vector3.up * (1 - Mathf.Pow( 2 * t - 1, 2 )) * _height;

			_elapse += Time.fixedDeltaTime;

			Vector3 position = hPosition + vPosition;

			Rigidbody.MovePosition( position );
			Rigidbody.MoveRotation( Quaternion.LookRotation( position - _prevPosition, Vector3.up ) );

			if (t >= 1 && _activated == false)
			{
				BattleEvents.DamageApplyed.Execute( new() {
					Target = Target,
					Amount = Damage,
					Type = EDamageType.Aoe,
					Range = Args.DamageRange,
					Position = transform.position,
				} );

				_activated = true;
				Dispose();
			}

			_prevPosition = position;
		}

		public override void OnSpawned(ProjectileArgs data, IMemoryPool pool )
		{
			base.OnSpawned( data, pool );

			_activated = false;
			_startPosition = data.StartPosition;
			TargetPosition = Target.Transform.position;
			_distance = Vector3.Distance( data.StartPosition, Target.Transform.position );
			_time = _distance / Speed;
			_height = Mathf.Lerp(0, data.Height, _distance / data.MaxDistance);
			_elapse = 0;
			Rigidbody.MoveRotation( Quaternion.identity );
		}

		public class Factory : PlaceholderFactory<ProjectileArgs, GrenadeCapsule> { }
	}
}
