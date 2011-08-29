﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Timeline
{
	public partial class TimelineControl : TimelineControlBase
	{

		public TimelineControl()
		{
			InitializeComponent();

			this.DoubleBuffered = true;

			// Reasonable defaults
			TotalTime = TimeSpan.FromMinutes(2);

			// Splitter
			splitContainer.Panel1MinSize = 100;
			splitContainer.Panel2MinSize = 100;
			splitContainer.FixedPanel = FixedPanel.Panel1;

			timelineGrid.Scroll += GridScrolledHandler;
			timelineGrid.VerticalOffsetChanged += GridScrollVerticalHandler;
			timelineGrid.VisibleTimeStartChanged += GridScrollHorizontalHandler;
		}


		#region Properties

		public TimeSpan TotalTime
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.TotalTime;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.TotalTime = value;
			}
		}


		public override TimeSpan VisibleTimeStart
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VisibleTimeStart;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.VisibleTimeStart = value;
			}
		}

		public override TimeSpan TimePerPixel
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.TimePerPixel;
				else
					return TimeSpan.Zero;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.TimePerPixel = value;
				if (timelineHeader != null)
					timelineHeader.TimePerPixel = value;
			}
		}

		public override TimeSpan VisibleTimeSpan
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VisibleTimeSpan;
				else
					return TimeSpan.Zero;
			}
		}

		public int VerticalOffset
		{
			get
			{
				if (timelineGrid != null)
					return timelineGrid.VerticalOffset;
				else
					return 0;
			}
			set
			{
				if (timelineGrid != null)
					timelineGrid.VerticalOffset = value;
			}
		}


		#endregion


		#region Methods

		// Zoom in or out (ie. change the visible time span): give a scale < 1.0
		// and it zooms in, > 1.0 and it zooms out.
		public void Zoom(double scale)
		{
			if (scale <= 0.0)
				return;

			if (VisibleTimeSpan.Scale(scale) > TotalTime) {
				TimePerPixel = TimeSpan.FromTicks(TotalTime.Ticks / timelineGrid.Width);
			} else {
				TimePerPixel = TimePerPixel.Scale(scale);
				if (VisibleTimeEnd > TotalTime)
					VisibleTimeEnd = TotalTime;
			}
		}

		private void AddRowToControls(TimelineRow row, TimelineRowLabel label)
		{
			timelineGrid.Rows.Add(row);
			timelineRowList.AddRowLabel(label);
		}

		// adds a given row to the control, optionally as a child of the given parent
		public void AddRow(TimelineRow row, TimelineRow parent = null)
		{
			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);
		}

		// adds a new, empty row with a default label with the given name, as a child of the (optional) given parent
		public TimelineRow AddRow(string name, TimelineRow parent = null, int height = 50)
		{
			TimelineRow row = new TimelineRow();

			row.Name = name;
			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}

		// adds a new, empty row with the given label, as a child of the (optional) given parent
		public TimelineRow AddRow(TimelineRowLabel label, TimelineRow parent = null, int height = 50)
		{
			TimelineRow row = new TimelineRow(label);

			row.Height = height;

			if (parent != null)
				parent.AddChildRow(row);

			AddRowToControls(row, row.RowLabel);

			return row;
		}


		public bool AddSnapTime(TimeSpan time, int level)
		{
			return timelineGrid.AddSnapPoint(time, level);
		}

		public bool RemoveSnapTime(TimeSpan time)
		{
			return timelineGrid.RemoveSnapPoint(time);
		}


		public void AlignSelectedElementsLeft()
		{
			timelineGrid.AlignSelectedElementsLeft();
		}


		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementDoubleClicked
		{
			add { timelineGrid.ElementDoubleClicked += value; }
			remove { timelineGrid.ElementDoubleClicked -= value; }
		}

		public event EventHandler<MultiElementEventArgs> ElementsMoved
		{
			add { timelineGrid.ElementsMoved += value; }
			remove { timelineGrid.ElementsMoved -= value; }
		}

		public event EventHandler<TimeSpanEventArgs> CursorMoved
		{
			add { timelineGrid.CursorMoved += value; }
			remove { timelineGrid.CursorMoved -= value; }
		}

		#endregion


		#region Event Handlers

		private void GridScrollVerticalHandler(object sender, EventArgs e)
		{
			if (timelineRowList != null)
				timelineRowList.VerticalOffset = timelineGrid.VerticalOffset;
		}

		private void GridScrollHorizontalHandler(object sender, EventArgs e)
		{
			if (timelineHeader != null)
				timelineHeader.VisibleTimeStart = timelineGrid.VisibleTimeStart;
		}

		private void GridScrolledHandler(object sender, ScrollEventArgs e)
		{
		    if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
				GridScrollHorizontalHandler(sender, e);
		    } else {
				GridScrollVerticalHandler(sender, e);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			timelineRowList.VerticalOffset = timelineGrid.VerticalOffset;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (Form.ModifierKeys.HasFlag(Keys.Control)) {
				// holding the control key zooms the horizontal axis, by 10% per mouse wheel tick
				// TODO: should we zoom the vertial rows as well?
				Zoom(1.0 - ((double)e.Delta / 1200.0));
			} else if (Form.ModifierKeys.HasFlag(Keys.Shift)) {
				// holding the skift key moves the horizontal axis, by 10% of the visible time span per mouse wheel tick
				VisibleTimeStart += VisibleTimeSpan.Scale(-((double)e.Delta / 1200.0));
			} else {
				// moving the mouse wheel with no modifiers moves the display vertically, 30 pixels per mouse wheel tick
				VerticalOffset += -(e.Delta / 4);
			}
		}



		#endregion

	}
}
