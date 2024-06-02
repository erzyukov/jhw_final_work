#if UNITY_EDITOR

namespace Game.Editor
{
	using Game.Units;
	using UniRx;
	using UnityEngine.UIElements;
	using WaveUnit = Game.Configs.WaveConfig.WaveUnit;

	public struct WaveElementArgs
	{
		public VisualTreeAsset		Template;
		public WaveUnit             Unit;
	}

    public class WaveElement
    {
		public VisualElement					Element { get; private set; }
		public ReactiveCommand<WaveUnit>		ElementChanged { get; }				= new();

		private WaveUnit        _waveUnit;

		private EnumField       _speciesField;
		private IntegerField    _powerField;
		private IntegerField    _gradeField;

		public WaveElement(WaveElementArgs args)
		{
			_waveUnit = new WaveUnit {
				Position = args.Unit.Position,
			};

			Element = args.Template.Instantiate();
			Element.AddToClassList( "wave-element" );

			_speciesField		= Element.Q<EnumField>("SpeciesSelect");
			_powerField			= Element.Q<IntegerField>("Power");
			_gradeField			= Element.Q<IntegerField>("Grade");

			SetSpecies( args.Unit.Species );
			SetPower( args.Unit.Power );
			SetGrade( args.Unit.GradeIndex );

			_speciesField.RegisterValueChangedCallback( e => SetSpecies( (Species)e.newValue ) );
			_powerField.RegisterValueChangedCallback( e => SetPower( e.newValue ) );
			_gradeField.RegisterValueChangedCallback( e => SetGrade( e.newValue ) );
		}

		private void SetGrade( int value )
		{
			_gradeField.value		= value;
			_waveUnit.GradeIndex	= value;
			ElementChanged.Execute( _waveUnit );
		}

		private void SetPower( int value )
		{
			_powerField.value		= value;
			_waveUnit.Power			= value;
			ElementChanged.Execute( _waveUnit );
		}

		private void SetSpecies( Species value )
		{
			_speciesField.value		= value;
			_waveUnit.Species		= value;
			ElementChanged.Execute( _waveUnit );

			if (value != Species.None )
				_speciesField.AddToClassList("species-field");
			else
				_speciesField.RemoveFromClassList("species-field");
		}

    }
}
#endif