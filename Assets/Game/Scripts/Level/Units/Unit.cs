namespace Game.Units
{
	using UnityEngine;
	using Zenject;

	public interface IUnit
	{
		Species Species { get; }
	}

	public class Unit : IUnit
	{
		[Inject] private Species _species;

		#region IUnit

		public Species Species => _species;
		
		#endregion
	}
}
