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
		#region columns
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
		
		public static readonly DependencyProperty ColumnDefinitionsSourceProperty = DependencyProperty.Register("ColumnDefinitionsSource",
		                                                                                                        typeof(System.Collections.IEnumerable),
		                                                                                                        typeof(NameBasedGrid),
		                                                                                                        new FrameworkPropertyMetadata(OnColumnDefinitionsSourceChanged));
		
		private static void OnColumnDefinitionsSourceChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((NameBasedGrid)source).OnColumnDefinitionsSourceChanged(e);
		}
		
		private void OnColumnDefinitionsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			columnDefinitions.SetSourceList(e.NewValue as System.Collections.IEnumerable);
		}
		
		public System.Collections.IEnumerable ColumnDefinitionsSource {
			get {
				return (System.Collections.IEnumerable)GetValue(ColumnDefinitionsSourceProperty);
			}
			set {
				if (value == null) {
					ClearValue(ColumnDefinitionsSourceProperty);
				} else {
					SetValue(ColumnDefinitionsSourceProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Identifies the <see cref="DefaultColumnWidth"/> property.
		/// </summary>
		/// <seealso cref="DefaultColumnWidth"/>
		public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register("DefaultColumnWidth",
		                                                                                                   typeof(GridLength),
		                                                                                                   typeof(NameBasedGrid),
		                                                                                                   new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), OnDefaultColumnWidthChanged));
		
		/// <summary>
		/// Processes a change of the <see cref="DefaultColumnWidth"/> property.
		/// </summary>
		/// <param name="source">The instance whose <see cref="DefaultColumnWidth"/> property was changed.</param>
		/// <param name="e">An object providing some information about the change.</param>
		private static void OnDefaultColumnWidthChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((NameBasedGrid)source).OnDefaultColumnWidthChanged(e);
		}
		
		/// <summary>
		/// Processes a change of the <see cref="DefaultColumnWidth"/> property.
		/// </summary>
		/// <param name="e">An object providing some information about the change.</param>
		private void OnDefaultColumnWidthChanged(DependencyPropertyChangedEventArgs e)
		{
			columnDefinitions.UpdateSize();
		}
		
		/// <summary>
		/// The default width of a column.
		/// </summary>
		/// <value>
		/// <para>This property specifies the default width of a column.
		///   The default value is a <see cref="GridLength"/> with a <see cref="GridLength.Value"/> of <c>1</c> and a <see cref="GridLength.GridUnitType"/> of <see cref="GridUnitType.Auto"/>.</para>
		/// <para>The default column width will be used for columns whose <see cref="ColumnOrRow.Size"/> property is <see langword="null"/>.</para>
		/// </value>
		/// <seealso cref="DefaultRowHeight"/>
		public GridLength DefaultColumnWidth {
			get {
				return (GridLength)GetValue(DefaultColumnWidthProperty);
			}
			set {
				SetValue(DefaultColumnWidthProperty, value);
			}
		}
		#endregion
		
		#region rows
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
		
		public static readonly DependencyProperty RowDefinitionsSourceProperty = DependencyProperty.Register("RowDefinitionsSource",
		                                                                                                     typeof(System.Collections.IEnumerable),
		                                                                                                     typeof(NameBasedGrid),
		                                                                                                     new FrameworkPropertyMetadata(OnRowDefinitionsSourceChanged));
		
		private static void OnRowDefinitionsSourceChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((NameBasedGrid)source).OnRowDefinitionsSourceChanged(e);
		}
		
		private void OnRowDefinitionsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			rowDefinitions.SetSourceList(e.NewValue as System.Collections.IEnumerable);
		}
		
		public System.Collections.IEnumerable RowDefinitionsSource {
			get {
				return (System.Collections.IEnumerable)GetValue(RowDefinitionsSourceProperty);
			}
			set {
				if (value == null) {
					ClearValue(RowDefinitionsSourceProperty);
				} else {
					SetValue(RowDefinitionsSourceProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Identifies the <see cref="DefaultRowHeight"/> property.
		/// </summary>
		/// <seealso cref="DefaultRowHeight"/>
		public static readonly DependencyProperty DefaultRowHeightProperty = DependencyProperty.Register("DefaultRowHeight",
		                                                                                                 typeof(GridLength),
		                                                                                                 typeof(NameBasedGrid),
		                                                                                                 new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Auto), OnDefaultRowHeightChanged));
		
		/// <summary>
		/// Processes a change of the <see cref="DefaultRowHeight"/> property.
		/// </summary>
		/// <param name="source">The instance whose <see cref="DefaultRowHeight"/> property was changed.</param>
		/// <param name="e">An object providing some information about the change.</param>
		private static void OnDefaultRowHeightChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((NameBasedGrid)source).OnDefaultRowHeightChanged(e);
		}
		
		/// <summary>
		/// Processes a change of the <see cref="DefaultRowHeight"/> property.
		/// </summary>
		/// <param name="e">An object providing some information about the change.</param>
		private void OnDefaultRowHeightChanged(DependencyPropertyChangedEventArgs e)
		{
			rowDefinitions.UpdateSize();
		}
		
		/// <summary>
		/// The default height of a row.
		/// </summary>
		/// <value>
		/// <para>This property specifies the default height of a row.
		///   The default value is a <see cref="GridLength"/> with a <see cref="GridLength.Value"/> of <c>1</c> and a <see cref="GridLength.GridUnitType"/> of <see cref="GridUnitType.Auto"/>.</para>
		/// <para>The default row height will be used for rows whose <see cref="ColumnOrRow.Size"/> property is <see langword="null"/>.</para>
		/// </value>
		/// <seealso cref="DefaultColumnWidth"/>
		public GridLength DefaultRowHeight {
			get {
				return (GridLength)GetValue(DefaultRowHeightProperty);
			}
			set {
				SetValue(DefaultRowHeightProperty, value);
			}
		}
		#endregion
		#endregion
	}
}
