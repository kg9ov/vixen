﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Text
{
	[DataContract]
	[KnownType(typeof (SerializableFont))]
	public class TextData: EffectTypeModuleData
	{
		public TextData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red)};
			BaseColor = Color.Lime;
			UseBaseColor = false;
			Direction = TextDirection.Left;
			Speed = 1;
			Position = 0;
			PositionX = 0;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			FontScaleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			ScaleText = 0;
			CenterStop = false;
			Text = new List<string>{String.Empty};
			GradientMode = GradientMode.AcrossElement;
			Orientation=StringOrientation.Vertical;
			Font = new SerializableFont(new Font("Arial", 8));
			LevelCurve = new Curve(CurveType.Flat100);
			BaseLevelCurve = new Curve(CurveType.Flat100);
			TextSource = TextSource.None;
			TextFade = TextFade.Out;
			TimeVisibleLength = 750;
			DirectionPerWord = false;
			RepeatText = false;
			TextDuration = TextDuration.AutoFit;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Color BaseColor { get; set; }

		[DataMember]
		public Curve BaseLevelCurve { get; set; }

		[DataMember]
		public bool UseBaseColor { get; set; }

		[DataMember]
		public TextDirection Direction { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int ScaleText { get; set; }

		[DataMember]
		public bool CenterStop { get; set; }

		[DataMember]
		public bool CenterText { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public TextMode TextMode { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Position { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int PositionX { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve FontScaleCurve { get; set; }

		[DataMember]
		public Curve AngleCurve { get; set; }

		[DataMember]
		public List<string> Text { get; set; }

		[DataMember]
		public SerializableFont Font { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }
		
		[DataMember]
		public TextSource TextSource { get; set; }

		[DataMember]
		public TextFade TextFade { get; set; }
		
		[DataMember]
		public int TimeVisibleLength { get; set; }
		
		[DataMember]
		public bool DirectionPerWord { get; set; }

		[DataMember]
		public bool RepeatText { get; set; }

		[DataMember]
		public TextDuration TextDuration { get; set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (TimeVisibleLength == 0) TimeVisibleLength = 750;

			if (LevelCurve == null)
			{
				LevelCurve = new Curve(CurveType.Flat100);
			}

			if (BaseLevelCurve == null)
			{
				BaseLevelCurve = new Curve(CurveType.Flat100);
			}

			//if one of them is null the others probably are, and if this one is not then they all should be good.
			//Try to save some cycles on every load

			if (FontScaleCurve == null)
			{
				AngleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));

				double value;

				if (FontScaleCurve == null)
				{
					FontScaleCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
					ScaleText = 0;
				}

				if (XOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(PositionX, 100, -100);
					XOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					PositionX = 0;
				}

				if (YOffsetCurve == null)
				{
					value = PixelEffectBase.ScaleValueToCurve(Position, 100, -100);
					YOffsetCurve = new Curve(new PointPairList(new[] {0.0, 100.0}, new[] {value, value}));
					Position = 0;
				}
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			TextData result = new TextData
			{
				Colors = Colors.ToList(),
				Direction = Direction,
				Speed = Speed,
				CenterStop = CenterStop,
				Orientation = Orientation,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				AngleCurve = new Curve(AngleCurve),
				Text = Text.ToList(),
				ScaleText = ScaleText,
				GradientMode = GradientMode,
				TextMode = TextMode,
				CenterText = CenterText,
				UseBaseColor = UseBaseColor,
				BaseColor = BaseColor,
				Font = new SerializableFont(Font.FontValue),
				LevelCurve = LevelCurve,
				FontScaleCurve = new Curve(FontScaleCurve),
				BaseLevelCurve = BaseLevelCurve,
				TextSource = TextSource,
				MarkCollectionId = MarkCollectionId,
				TextFade = TextFade,
				TimeVisibleLength = TimeVisibleLength,
				DirectionPerWord = DirectionPerWord,
				RepeatText = RepeatText,
				TextDuration = TextDuration
			};
			return result;
		}
	}
}
