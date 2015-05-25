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
		private abstract class ColumnOrRowListController : IColumnOrRowListController
		{
			public ColumnOrRowListController(NameBasedGrid owner)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			private readonly NameBasedGrid owner;
			
			public NameBasedGrid Owner {
				get {
					return owner;
				}
			}
			
			public void ColumnOrRowInserted(int index, ColumnOrRow columnOrRow)
			{
				InsertItem(index);
				SetSharedSizeGroup(index, columnOrRow.SharedSizeGroup);
				SetSize(index, columnOrRow.Size);
			}
			
			protected abstract void InsertItem(int index);
			
			public abstract void SetSharedSizeGroup(int index, string groupName);
			
			public abstract void SetSize(int index, System.Windows.GridLength size);
			
			public abstract void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2);
			
			public abstract void SetPhysicalIndex(UIElement element, int index, int span);
			
			public IEnumerable<UIElement> AllElements {
				get {
					return owner.Children.Cast<UIElement>();
				}
			}
		}
		
		private sealed class ColumnListController : ColumnOrRowListController
		{
			public ColumnListController(NameBasedGrid owner, ColumnDefinitionCollection definitions) : base(owner)
			{
				if (definitions == null) {
					throw new ArgumentNullException("definitions");
				}
				
				this.definitions = definitions;
			}
			
			private readonly ColumnDefinitionCollection definitions;
			
			protected override void InsertItem(int index)
			{
				var newItem = new ColumnDefinition();
				definitions.Insert(index, newItem);
			}
			
			public override void SetSharedSizeGroup(int index, string groupName)
			{
				definitions[index].SharedSizeGroup = groupName;
			}
			
			public override void SetSize(int index, GridLength size)
			{
				definitions[index].Width = size;
			}
			
			public override void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2)
			{
				columnOrRow1 = NameBasedGrid.GetColumn(element);
				columnOrRow2 = NameBasedGrid.GetExtendToColumn(element);
			}
			
			public override void SetPhysicalIndex(UIElement element, int index, int span)
			{
				Grid.SetColumn(element, index);
				Grid.SetColumnSpan(element, span);
			}
		}
		
		private sealed class RowListController : ColumnOrRowListController
		{
			public RowListController(NameBasedGrid owner, RowDefinitionCollection definitions) : base(owner)
			{
				if (definitions == null) {
					throw new ArgumentNullException("definitions");
				}
				
				this.definitions = definitions;
			}
			
			private readonly RowDefinitionCollection definitions;
			
			protected override void InsertItem(int index)
			{
				var newItem = new RowDefinition();
				definitions.Insert(index, newItem);
			}
			
			public override void SetSharedSizeGroup(int index, string groupName)
			{
				definitions[index].SharedSizeGroup = groupName;
			}
			
			public override void SetSize(int index, GridLength size)
			{
				definitions[index].Height = size;
			}
			
			public override void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2)
			{
				columnOrRow1 = NameBasedGrid.GetRow(element);
				columnOrRow2 = NameBasedGrid.GetExtendToRow(element);
			}
			
			public override void SetPhysicalIndex(UIElement element, int index, int span)
			{
				Grid.SetRow(element, index);
				Grid.SetRowSpan(element, span);
			}
		}
	}
}
