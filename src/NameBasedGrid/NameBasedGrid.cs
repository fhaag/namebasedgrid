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
	/// <summary>
	/// Defines a flexible grid area whose columns and rows can be named.
	/// </summary>
	/// <remarks>
	/// <para>A <see cref="T:NameBasedGrid.NameBasedGrid"/> works analogously to a <see cref="Grid"/>, but it uses named columns and rows.
	///   The columns and rows can be defined with the <see cref="ColumnDefinitions"/> and <see cref="RowDefinitions"/> properties.
	///   <see cref="SetColumn"/>, <see cref="SetExtendToColumn"/>, <see cref="SetRow"/>, <see cref="SetExtendToRow"/>, and the respective attached properties can then be used on the controls in the name-based grid.</para>
	/// </remarks>
	public partial class NameBasedGrid : Panel
	{
		/// <summary>
		/// A façade of a collection of children that actually forwards any accesses to the internal <see cref="Grid"/> control.
		/// </summary>
		private sealed class ChildCollection : UIElementCollection
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="owner">The <see cref="T:NameBasedGrid.NameBasedGrid"/> this collection belongs to.</param>
			/// <param name="visualParent">The <see cref="UIElement"/> parent of the collection.</param>
			/// <param name="logicalParent">The logical parent of the elements in the collection.</param>
			/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
			public ChildCollection(NameBasedGrid owner, UIElement visualParent, FrameworkElement logicalParent) : base(visualParent, logicalParent)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			/// <summary>
			/// The control that this collection belongs to.
			/// </summary>
			private readonly NameBasedGrid owner;
			
			/// <summary>
			/// Appends a new element to the end of the collection.
			/// </summary>
			/// <param name="element">The new element.</param>
			/// <returns>The index of the newly added item.</returns>
			/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
			public override int Add(UIElement element)
			{
				if (element == null) {
					throw new ArgumentNullException("element");
				}
				
				int result = owner.grid.Children.Add(element);
				InsertElement(element);
				return result;
			}
			
			/// <summary>
			/// Inserts a new element at an arbitrary position.
			/// </summary>
			/// <param name="index">The intended position of the new element.</param>
			/// <param name="element">The new element.</param>
			/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
			public override void Insert(int index, UIElement element)
			{
				if (element == null) {
					throw new ArgumentNullException("element");
				}
				
				base.Insert(index, element);
				InsertElement(element);
			}
			
			/// <summary>
			/// Removes an element from the collection.
			/// </summary>
			/// <param name="element">The element to remove.</param>
			public override void Remove(UIElement element)
			{
				int idx = owner.grid.Children.IndexOf(element);
				if (idx >= 0) {
					RemoveAt(idx);
				}
			}
			
			/// <summary>
			/// Removes all elements from the collection.
			/// </summary>
			public override void Clear()
			{
				var oldElements = owner.grid.Children.Cast<UIElement>().ToArray();
				owner.grid.Children.Clear();
				foreach (var el in oldElements) {
					RemoveElement(el);
				}
			}
			
			/// <summary>
			/// Removes an element at a specified index.
			/// </summary>
			/// <param name="index">The index of the item to remove.</param>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the collection.</exception>
			public override void RemoveAt(int index)
			{
				RemoveElement(owner.grid.Children[index]);
				owner.grid.Children.RemoveAt(index);
			}
			
			/// <summary>
			/// Remvoes a range of elements from the collection.
			/// </summary>
			/// <param name="index">The first index to remove.</param>
			/// <param name="count">The number of items to remove.</param>
			/// <exception cref="ArgumentOutOfRangeException">The range specified by <paramref name="index"/> and <paramref name="count"/> exceeds the boundaries of the collection.</exception>
			public override void RemoveRange(int index, int count)
			{
				for (int i = 0; i < count; i++) {
					RemoveElement(owner.grid.Children[index + i]);
				}
				owner.grid.Children.RemoveRange(index, count);
			}
			
			/// <summary>
			/// Returns the number of items in the collection.
			/// </summary>
			public override int Count {
				get {
					return owner.grid.Children.Count;
				}
			}
			
			/// <summary>
			/// Gets or sets an element at a given position.
			/// </summary>
			/// <param name="index">The position of the element.</param>
			/// <exception cref="ArgumentNullException">The assigned value is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the collection.</exception>
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
			
			/// <summary>
			/// Checks whether the collection contains a given item.
			/// </summary>
			/// <param name="element">The item to find.</param>
			/// <returns>A value that indicates whether <paramref name="element"/> was found.</returns>
			public override bool Contains(UIElement element)
			{
				return owner.grid.Children.Contains(element);
			}
			
			/// <summary>
			/// Retrieves the position of a given element.
			/// </summary>
			/// <param name="element">The item to find.</param>
			/// <returns>The zero-based index of the element in the collection, or a negative value if <paramref name="element"/> was not found.</returns>
			public override int IndexOf(UIElement element)
			{
				return owner.grid.Children.IndexOf(element);
			}
			
			/// <summary>
			/// Copies the contents of the collection into an array, starting at a given start index.
			/// </summary>
			/// <param name="array">The destination array.</param>
			/// <param name="index">The index of the element where copying begins.</param>
			public override void CopyTo(Array array, int index)
			{
				owner.grid.Children.CopyTo(array, index);
			}
			
			/// <summary>
			/// Copies the contents of the collection into an array, starting at a given start index.
			/// </summary>
			/// <param name="array">The destination array.</param>
			/// <param name="index">The index of the element where copying begins.</param>
			public override void CopyTo(UIElement[] array, int index)
			{
				owner.grid.Children.CopyTo(array, index);
			}
			
			/// <summary>
			/// Returns an enumerator over the whole collection.
			/// </summary>
			/// <returns>The enumerator.</returns>
			public override System.Collections.IEnumerator GetEnumerator()
			{
				return owner.grid.Children.GetEnumerator();
			}
			
			/// <summary>
			/// Processes the insertion of an element.
			/// </summary>
			/// <param name="element">The item being added.</param>
			/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
			private void InsertElement(UIElement element)
			{
				owner.columnDefinitions.UpdatePlacement(element);
				owner.rowDefinitions.UpdatePlacement(element);
			}
			
			/// <summary>
			/// Processes the removal of an element.
			/// </summary>
			/// <param name="element">The item being removed.</param>
			private void RemoveElement(UIElement element)
			{
				if (element != null) {
					element.ClearValue(Grid.ColumnProperty);
					element.ClearValue(Grid.ColumnSpanProperty);
					element.ClearValue(Grid.RowProperty);
					element.ClearValue(Grid.RowSpanProperty);
				}
			}
		}
		
		/// <summary>
		/// Initializes the class.
		/// </summary>
		static NameBasedGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NameBasedGrid), new FrameworkPropertyMetadata(typeof(NameBasedGrid)));
			FocusableProperty.OverrideMetadata(typeof(NameBasedGrid), new FrameworkPropertyMetadata(false));
		}
		
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public NameBasedGrid()
		{
			this.columnDefinitions = new ColumnOrRowList(new ColumnListController(this, grid.ColumnDefinitions));
			this.rowDefinitions = new ColumnOrRowList(new RowListController(this, grid.RowDefinitions));
			
			this.AddVisualChild(grid);
		}
		
		#region internal layout
		/// <summary>
		/// The grid control used internally for layouting.
		/// </summary>
		private readonly Grid grid = new Grid();
		
		/// <summary>
		/// Returns the number of visual children.
		/// </summary>
		protected override int VisualChildrenCount {
			get {
				return 1;
			}
		}
		
		/// <summary>
		/// Retrieves a visual child element at a specified position.
		/// </summary>
		/// <param name="index">The position of the child element.</param>
		/// <returns>The child element.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index of a visual child.</exception>
		protected override System.Windows.Media.Visual GetVisualChild(int index)
		{
			if (index == 0) {
				return grid;
			} else {
				throw new ArgumentOutOfRangeException("index");
			}
		}
		
		/// <summary>
		/// Initializes a collection object for the children of the panel.
		/// </summary>
		/// <param name="logicalParent">The logical parent of the items in the collection.</param>
		/// <returns>The newly created collection.</returns>
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			return new ChildCollection(this, grid, logicalParent);
		}
		
		/// <summary>
		/// Measures the size of child elements and determines the desired size for the panel.
		/// </summary>
		/// <param name="availableSize">The size available for the panel.</param>
		/// <returns>The size desired by the panel, based on its contents.</returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			grid.Measure(availableSize);
			return grid.DesiredSize;
		}
		
		/// <summary>
		/// Positions child elements and determines the size of the panel.
		/// </summary>
		/// <param name="finalSize">The final size available for the panel.</param>
		/// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			grid.Arrange(new Rect(new Point(), finalSize));
			return finalSize;
		}
		#endregion
		
		#region attached properties
		/// <summary>
		/// Finds the <see cref="T:NameBasedGrid.NameBasedGrid"/> parent of a given object, if any.
		/// </summary>
		/// <param name="obj">An object.</param>
		/// <returns>The <see cref="T:NameBasedGrid.NameBasedGrid"/> that <paramref name="obj"/> is contained in, or <see langword="null"/> if no such panel can be found.</returns>
		private static NameBasedGrid FindGrid(object obj)
		{
			var el = obj as UIElement;
			if (el != null) {
				var innerGrid = LogicalTreeHelper.GetParent(el) as Grid;
				if (innerGrid != null) {
					return VisualTreeHelper.GetParent(innerGrid) as NameBasedGrid;
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Identifies the <see cref="P:NameBasedGrid.NameBasedGrid.Column"/> attached property.
		/// </summary>
		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column",
		                                                                                               typeof(string),
		                                                                                               typeof(NameBasedGrid),
		                                                                                               new FrameworkPropertyMetadata(OnColumnChanged));
		
		/// <summary>
		/// Processes changes of the <see cref="P:NameBasedGrid.NameBasedGrid.Column"/> property.
		/// </summary>
		/// <param name="source">The object whose property value was changed.</param>
		/// <param name="e">An object that contains some information on the property change.</param>
		private static void OnColumnChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.columnDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		/// <summary>
		/// Retrieves the value of the <see cref="P:NameBasedGrid.NameBasedGrid.Column"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be retrieved.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static string GetColumn(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ColumnProperty);
		}
		
		/// <summary>
		/// Assigns a value to the <see cref="P:NameBasedGrid.NameBasedGrid.Column"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be modified.</param>
		/// <param name="column">The new property value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static void SetColumn(UIElement element, string column)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ColumnProperty, column);
		}
		
		/// <summary>
		/// Identifies the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToColumn"/> attached property.
		/// </summary>
		public static readonly DependencyProperty ExtendToColumnProperty = DependencyProperty.RegisterAttached("ExtendToColumn",
		                                                                                                       typeof(string),
		                                                                                                       typeof(NameBasedGrid),
		                                                                                                       new FrameworkPropertyMetadata(OnExtendToColumnChanged));
		
		/// <summary>
		/// Processes changes of the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToColumn"/> property.
		/// </summary>
		/// <param name="source">The object whose property value was changed.</param>
		/// <param name="e">An object that contains some information on the property change.</param>
		private static void OnExtendToColumnChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.columnDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		/// <summary>
		/// Retrieves the value of the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToColumn"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be retrieved.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static string GetExtendToColumn(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ExtendToColumnProperty);
		}
		
		/// <summary>
		/// Assigns a value to the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToColumn"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be modified.</param>
		/// <param name="extendToColumn">The new property value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static void SetExtendToColumn(UIElement element, string extendToColumn)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ExtendToColumnProperty, extendToColumn);
		}
		
		/// <summary>
		/// Identifies the <see cref="P:NameBasedGrid.NameBasedGrid.Row"/> attached property.
		/// </summary>
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row",
		                                                                                            typeof(string),
		                                                                                            typeof(NameBasedGrid),
		                                                                                            new FrameworkPropertyMetadata(OnRowChanged));
		
		/// <summary>
		/// Processes changes of the <see cref="P:NameBasedGrid.NameBasedGrid.Row"/> property.
		/// </summary>
		/// <param name="source">The object whose property value was changed.</param>
		/// <param name="e">An object that contains some information on the property change.</param>
		private static void OnRowChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.rowDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		/// <summary>
		/// Retrieves the value of the <see cref="P:NameBasedGrid.NameBasedGrid.Row"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be retrieved.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static string GetRow(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(RowProperty);
		}
		
		/// <summary>
		/// Assigns a value to the <see cref="P:NameBasedGrid.NameBasedGrid.Row"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be modified.</param>
		/// <param name="row">The new property value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static void SetRow(UIElement element, string row)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(RowProperty, row);
		}
		
		/// <summary>
		/// Identifies the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToRow"/> attached property.
		/// </summary>
		public static readonly DependencyProperty ExtendToRowProperty = DependencyProperty.RegisterAttached("ExtendToRow",
		                                                                                                    typeof(string),
		                                                                                                    typeof(NameBasedGrid),
		                                                                                                    new FrameworkPropertyMetadata(OnExtendToRowChanged));
		
		/// <summary>
		/// Processes changes of the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToRow"/> property.
		/// </summary>
		/// <param name="source">The object whose property value was changed.</param>
		/// <param name="e">An object that contains some information on the property change.</param>
		private static void OnExtendToRowChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			var grid = FindGrid(source);
			if (grid != null) {
				grid.rowDefinitions.UpdatePlacement((UIElement)source);
			}
		}
		
		/// <summary>
		/// Retrieves the value of the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToRow"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be retrieved.</param>
		/// <returns>The property value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static string GetExtendToRow(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			return (string)element.GetValue(ExtendToRowProperty);
		}
		
		/// <summary>
		/// Assigns a value to the <see cref="P:NameBasedGrid.NameBasedGrid.ExtendToRow"/> property for a given <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">The element whose property value should be modified.</param>
		/// <param name="extendToRow">The new property value.</param>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		public static void SetExtendToRow(UIElement element, string extendToRow)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			element.SetValue(ExtendToRowProperty, extendToRow);
		}
		#endregion
		
		#region columns and rows
		/// <summary>
		/// The list of column definitions for the grid.
		/// </summary>
		/// <seealso cref="ColumnDefinitions"/>
		private readonly ColumnOrRowList columnDefinitions;
		
		/// <summary>
		/// The list of column definitions for the grid.
		/// </summary>
		/// <seealso cref="RowDefinitions"/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnOrRowList ColumnDefinitions {
			get {
				return columnDefinitions;
			}
		}
		
		/// <summary>
		/// The list of row definitions for the grid.
		/// </summary>
		/// <seealso cref="RowDefinitions"/>
		private readonly ColumnOrRowList rowDefinitions;
		
		/// <summary>
		/// The list of row definitions for the grid.
		/// </summary>
		/// <seealso cref="ColumnDefinitions"/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnOrRowList RowDefinitions {
			get {
				return rowDefinitions;
			}
		}
		#endregion
	}
}
