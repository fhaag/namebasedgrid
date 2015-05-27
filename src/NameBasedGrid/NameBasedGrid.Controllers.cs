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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NameBasedGrid
{
	partial class NameBasedGrid
	{
		/// <summary>
		/// The base class for list controllers linked to a <see cref="NameBasedGrid"/> instance.
		/// </summary>
		private abstract class ColumnOrRowListController : IColumnOrRowListController
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="owner">The grid the new instance is linked to.</param>
			/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
			protected ColumnOrRowListController(NameBasedGrid owner)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			/// <summary>
			/// The grid this instance is linked to.
			/// </summary>
			/// <seealso cref="Owner"/>
			private readonly NameBasedGrid owner;
			
			/// <summary>
			/// The grid this instance is linked to.
			/// </summary>
			public NameBasedGrid Owner {
				get {
					return owner;
				}
			}
			
			/// <summary>
			/// Processes the insertion of a column or row definition.
			/// </summary>
			/// <param name="index">The index of the newly inserted definition.</param>
			/// <param name="columnOrRow">The newly inserted column or row definition.</param>
			public void ColumnOrRowInserted(int index, ColumnOrRow columnOrRow)
			{
				InsertItem(index);
				SetSharedSizeGroup(index, columnOrRow.SharedSizeGroup);
				SetSize(index, columnOrRow.Size);
			}
			
			/// <summary>
			/// Processes the insertion of a new column or row.
			/// </summary>
			/// <param name="index">The index at which the definition is inserted.</param>
			protected abstract void InsertItem(int index);
			
			/// <summary>
			/// Processes the removal of a column or row definition.
			/// </summary>
			/// <param name="index">The index of the removed definition.</param>
			public abstract void ColumnOrRowRemoved(int index);
			
			/// <summary>
			/// Sets the shared size group of a column or row at a given index.
			/// </summary>
			/// <param name="index">The index of the column or row.</param>
			/// <param name="groupName">The shared size group name.</param>
			public abstract void SetSharedSizeGroup(int index, string groupName);
			
			/// <summary>
			/// Sets the width or height of a column or row at a given index.
			/// </summary>
			/// <param name="index">The index of the column or row.</param>
			/// <param name="size">The width or height.</param>
			public abstract void SetSize(int index, System.Windows.GridLength size);
			
			/// <summary>
			/// Retrieves the column or row names assigned to a visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="columnOrRow1">The first column or row name.</param>
			/// <param name="columnOrRow2">The second column or row name.</param>
			public abstract void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2);
			
			/// <summary>
			/// Sets the physical column or row indices of a given visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="index">The lower index.</param>
			/// <param name="span">The number of columns or rows to span.</param>
			public abstract void SetPhysicalIndex(UIElement element, int index, int span);
			
			/// <summary>
			/// Enumerates all elements in the <see cref="T:NameBasedGrid.NameBasedGrid"/>.
			/// </summary>
			public IEnumerable<UIElement> AllElements {
				get {
					return owner.Children.Cast<UIElement>();
				}
			}
		}
		
		/// <summary>
		/// A subclass of <see cref="ColumnOrRowListController"/> that wraps column definitions.
		/// </summary>
		/// <seealso cref="RowListController"/>
		private sealed class ColumnListController : ColumnOrRowListController
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="owner">The grid the new instance is linked to.</param>
			/// <param name="definitions">The wrapped list of column definitions.</param>
			/// <exception cref="ArgumentNullException">Any of the arguments is <see langword="null"/>.</exception>
			public ColumnListController(NameBasedGrid owner, ColumnDefinitionCollection definitions) : base(owner)
			{
				if (definitions == null) {
					throw new ArgumentNullException("definitions");
				}
				
				this.definitions = definitions;
			}
			
			/// <summary>
			/// The wrapped list of column definitions.
			/// </summary>
			private readonly ColumnDefinitionCollection definitions;
			
			/// <summary>
			/// Processes the insertion of a new column.
			/// </summary>
			/// <param name="index">The index at which the definition is inserted.</param>
			protected override void InsertItem(int index)
			{
				var newItem = new ColumnDefinition();
				definitions.Insert(index, newItem);
			}
			
			/// <summary>
			/// Processes the removal of a column or row definition.
			/// </summary>
			/// <param name="index">The index of the removed definition.</param>
			public override void ColumnOrRowRemoved(int index)
			{
				definitions.RemoveAt(index);
			}
			
			/// <summary>
			/// Sets the shared size group of a column at a given index.
			/// </summary>
			/// <param name="index">The index of the column.</param>
			/// <param name="groupName">The shared size group name.</param>
			public override void SetSharedSizeGroup(int index, string groupName)
			{
				definitions[index].SharedSizeGroup = groupName;
			}
			
			/// <summary>
			/// Sets the width or height of a column at a given index.
			/// </summary>
			/// <param name="index">The index of the column.</param>
			/// <param name="size">The width or height.</param>
			public override void SetSize(int index, GridLength size)
			{
				definitions[index].Width = size;
			}
			
			/// <summary>
			/// Retrieves the column names assigned to a visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="columnOrRow1">The first column name.</param>
			/// <param name="columnOrRow2">The second column name.</param>
			public override void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2)
			{
				columnOrRow1 = NameBasedGrid.GetColumn(element);
				columnOrRow2 = NameBasedGrid.GetExtendToColumn(element);
			}
			
			/// <summary>
			/// Sets the physical column indices of a given visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="index">The lower index.</param>
			/// <param name="span">The number of columns to span.</param>
			public override void SetPhysicalIndex(UIElement element, int index, int span)
			{
				Grid.SetColumn(element, index);
				Grid.SetColumnSpan(element, span);
			}
		}
		
		/// <summary>
		/// A subclass of <see cref="ColumnOrRowListController"/> that wraps row definitions.
		/// </summary>
		/// <seealso cref="ColumnListController"/>
		private sealed class RowListController : ColumnOrRowListController
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="owner">The grid the new instance is linked to.</param>
			/// <param name="definitions">The wrapped list of row definitions.</param>
			/// <exception cref="ArgumentNullException">Any of the arguments is <see langword="null"/>.</exception>
			public RowListController(NameBasedGrid owner, RowDefinitionCollection definitions) : base(owner)
			{
				if (definitions == null) {
					throw new ArgumentNullException("definitions");
				}
				
				this.definitions = definitions;
			}
			
			/// <summary>
			/// The wrapped list of row definitions.
			/// </summary>
			private readonly RowDefinitionCollection definitions;
			
			/// <summary>
			/// Processes the insertion of a new row.
			/// </summary>
			/// <param name="index">The index at which the definition is inserted.</param>
			protected override void InsertItem(int index)
			{
				var newItem = new RowDefinition();
				definitions.Insert(index, newItem);
			}
			
			/// <summary>
			/// Processes the removal of a column or row definition.
			/// </summary>
			/// <param name="index">The index of the removed definition.</param>
			public override void ColumnOrRowRemoved(int index)
			{
				definitions.RemoveAt(index);
			}
			
			/// <summary>
			/// Sets the shared size group of a row at a given index.
			/// </summary>
			/// <param name="index">The index of the row.</param>
			/// <param name="groupName">The shared size group name.</param>
			public override void SetSharedSizeGroup(int index, string groupName)
			{
				definitions[index].SharedSizeGroup = groupName;
			}
			
			/// <summary>
			/// Sets the width or height of a row at a given index.
			/// </summary>
			/// <param name="index">The index of the row.</param>
			/// <param name="size">The width or height.</param>
			public override void SetSize(int index, GridLength size)
			{
				definitions[index].Height = size;
			}
			
			/// <summary>
			/// Retrieves the row names assigned to a visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="columnOrRow1">The first row name.</param>
			/// <param name="columnOrRow2">The second row name.</param>
			public override void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2)
			{
				columnOrRow1 = NameBasedGrid.GetRow(element);
				columnOrRow2 = NameBasedGrid.GetExtendToRow(element);
			}
			
			/// <summary>
			/// Sets the physical row indices of a given visual element.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="index">The lower index.</param>
			/// <param name="span">The number of rows to span.</param>
			public override void SetPhysicalIndex(UIElement element, int index, int span)
			{
				Grid.SetRow(element, index);
				Grid.SetRowSpan(element, span);
			}
		}
	}
}
