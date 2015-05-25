/*
------------------------------------------------------------------------------
This source file is a part of Name-Based Grid.

Copyright (c) 2015 Florian Haag

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
------------------------------------------------------------------------------
 */
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace NameBasedGrid
{
	public partial class NameBasedGrid : Panel//, IAddChild
	{
		private sealed class ChildCollection : UIElementCollection
		{
			public ChildCollection(NameBasedGrid owner, UIElement visualParent, FrameworkElement logicalParent) : base(visualParent, logicalParent)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			private readonly NameBasedGrid owner;
			
			public override int Add(UIElement element)
			{
				if (element == null) {
					throw new ArgumentNullException("element");
				}
				
				int result = owner.grid.Children.Add(element);
				InsertElement(element);
				return result;
			}
			
			public override void Insert(int index, UIElement element)
			{
				if (element == null) {
					throw new ArgumentNullException("element");
				}
				
				base.Insert(index, element);
				InsertElement(element);
			}
			
			public override void Remove(UIElement element)
			{
				owner.grid.Children.Remove(element);
				RemoveElement(element);
			}
			
			public override void Clear()
			{
				var oldElements = owner.grid.Children.Cast<UIElement>().ToArray();
				owner.grid.Children.Clear();
				foreach (var el in oldElements) {
					RemoveElement(el);
				}
			}
			
			public override void RemoveAt(int index)
			{
				RemoveElement(owner.grid.Children[index]);
				owner.grid.Children.RemoveAt(index);
			}
			
			public override void RemoveRange(int index, int count)
			{
				for (int i = 0; i < count; i++) {
					RemoveElement(owner.grid.Children[index + i]);
				}
				owner.grid.Children.RemoveRange(index, count);
			}
			
			public override int Count {
				get {
					return owner.grid.Children.Count;
				}
			}
			
			public override UIElement this[int index] {
				get {
					return owner.grid.Children[index];
				}
				set {
					if (value == null) {
						throw new ArgumentNullException("value");
					}
					
					if (value != owner.grid.Children[index]) {
						RemoveElement(owner.grid.Children[index]);
						owner.grid.Children[index] = value;
						InsertElement(value);
					}
				}
			}
			
			public override bool Contains(UIElement element)
			{
				return owner.grid.Children.Contains(element);
			}
			
			public override int IndexOf(UIElement element)
			{
				return owner.grid.Children.IndexOf(element);
			}
			
			public override void CopyTo(Array array, int index)
			{
				owner.grid.Children.CopyTo(array, index);
			}
			
			public override void CopyTo(UIElement[] array, int index)
			{
				owner.grid.Children.CopyTo(array, index);
			}
			
			public override System.Collections.IEnumerator GetEnumerator()
			{
				return owner.grid.Children.GetEnumerator();
			}
			
			private void InsertElement(UIElement element)
			{
				owner.columnDefinitions.UpdatePlacement(element);
			}
			
			private void RemoveElement(UIElement element)
			{
			}
		}
		
		static NameBasedGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NameBasedGrid), new FrameworkPropertyMetadata(typeof(NameBasedGrid)));
			FocusableProperty.OverrideMetadata(typeof(NameBasedGrid), new FrameworkPropertyMetadata(false));
		}
		
		public NameBasedGrid()
		{
			this.columnDefinitions = new ColumnOrRowList(new ColumnListController(this, grid.ColumnDefinitions));
			this.rowDefinitions = new ColumnOrRowList(new RowListController(this, grid.RowDefinitions));
			
			this.AddVisualChild(grid);
		}
		
		#region internal layout
		private readonly Grid grid = new Grid();
		
		protected override int VisualChildrenCount {
			get {
				return 1;
			}
		}
		
		protected override System.Windows.Media.Visual GetVisualChild(int index)
		{
			if (index == 0) {
				return grid;
			} else {
				throw new ArgumentOutOfRangeException("index");
			}
		}
		
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			return new ChildCollection(this, grid, logicalParent);
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			grid.Measure(availableSize);
			return grid.DesiredSize;
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			grid.Arrange(new Rect(new Point(), finalSize));
			return finalSize;
		}
		#endregion
		
		#region attached properties
		private static NameBasedGrid FindGrid(object source)
		{
			var el = source as UIElement;
			if (el != null) {
				var innerGrid = LogicalTreeHelper.GetParent(el) as Grid;
				if (innerGrid != null) {
					return VisualTreeHelper.GetParent(innerGrid) as NameBasedGrid;
				}
			}
			
			return null;
		}
		
		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column",
		                                                                                               typeof(string),
		                                                                                               typeof(NameBasedGrid),
		                                                                                               new FrameworkPropertyMetadata(OnColumnChanged));
		
		private static void OnColumnChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.columnDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		public static string GetColumn(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ColumnProperty);
		}
		
		public static void SetColumn(UIElement element, string column)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ColumnProperty, column);
		}
		
		public static readonly DependencyProperty ExtendToColumnProperty = DependencyProperty.RegisterAttached("ExtendToColumn",
		                                                                                                       typeof(string),
		                                                                                                       typeof(NameBasedGrid),
		                                                                                                       new FrameworkPropertyMetadata(OnExtendToColumnChanged));
		
		private static void OnExtendToColumnChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.columnDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		public static string GetExtendToColumn(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ExtendToColumnProperty);
		}
		
		public static void SetExtendToColumn(UIElement element, string column)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ExtendToColumnProperty, column);
		}
		
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row",
		                                                                                            typeof(string),
		                                                                                            typeof(NameBasedGrid),
		                                                                                            new FrameworkPropertyMetadata(OnRowChanged));
		
		private static void OnRowChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.rowDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		public static string GetRow(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(RowProperty);
		}
		
		public static void SetRow(UIElement element, string column)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(RowProperty, column);
		}
		
		public static readonly DependencyProperty ExtendToRowProperty = DependencyProperty.RegisterAttached("ExtendToRow",
		                                                                                                    typeof(string),
		                                                                                                    typeof(NameBasedGrid),
		                                                                                                    new FrameworkPropertyMetadata(OnExtendToRowChanged));
		
		private static void OnExtendToRowChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.rowDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		public static string GetExtendToRow(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ExtendToRowProperty);
		}
		
		public static void SetExtendToRow(UIElement element, string column)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ExtendToRowProperty, column);
		}
		#endregion
		
		#region columns and rows
		private readonly ColumnOrRowList columnDefinitions;
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnOrRowList ColumnDefinitions {
			get {
				return columnDefinitions;
			}
		}
		
		private readonly ColumnOrRowList rowDefinitions;
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnOrRowList RowDefinitions {
			get {
				return rowDefinitions;
			}
		}
		#endregion
		
		public void AddChild(object value)
		{
			throw new NotImplementedException();
		}
		
		public void AddText(string text)
		{
			throw new NotImplementedException();
		}
	}
}
