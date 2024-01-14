namespace Game.Units
{
	using Game.Fx;
	using UnityEngine;

    public class ShootFx : MonoBehaviour
	{
		[SerializeField] private ParticleFx _particleFx;

		#region IShootFx

		public void Play() =>
			_particleFx.Play();

		#endregion
	}
}
