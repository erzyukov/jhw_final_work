namespace Game.Field
{
	using Game.Level;
	using UnityEngine;
	using Zenject;

	public interface IFieldEnemyFacade : IFieldFacade
	{
		void Clear();
	}

	public class FieldEnemyFacade : FieldFacade, IFieldEnemyFacade
	{
		#region IFieldEnemyFacade

		public void Clear()
		{
			Field.Clear();
		}

		#endregion
	}
}
