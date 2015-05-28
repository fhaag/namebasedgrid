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
using System.Windows;

namespace NameBasedGrid
{
	/// <summary>
	/// An interface for objects that apply column or row definitions to elements in a <see cref="T:NameBasedGrid.NameBasedGrid"/>.
	/// </summary>
	internal interface IColumnOrRowListController
	{
		/// <summary>
		/// Processes the insertion of a column or row definition.
		/// </summary>
		/// <param name="index">The index of the newly inserted definition.</param>
		/// <param name="columnOrRow">The newly inserted column or row definition.</param>
		void ColumnOrRowInserted(int index, ColumnOrRow columnOrRow);
		
		/// <summary>
		/// Processes the removal of a column or row definition.
		/// </summary>
		/// <param name="index">The index of the removed definition.</param>
		void ColumnOrRowRemoved(int index);
		
		/// <summary>
		/// Sets the shared size group of a column or row at a given index.
		/// </summary>
		/// <param name="index">The index of the column or row.</param>
		/// <param name="groupName">The shared size group name.</param>
		void SetSharedSizeGroup(int index, string groupName);
		
		/// <summary>
		/// Sets the width or height of a column or row at a given index.
		/// </summary>
		/// <param name="index">The index of the column or row.</param>
		/// <param name="size">The width or height.</param>
		void SetSize(int index, GridLength size);
		
		/// <summary>
		/// Sets the minimum width or height of a column or row at a given index.
		/// </summary>
		/// <param name="index">The index of the column or row.</param>
		/// <param name="minSize">The minimum width or height.</param>
		void SetMinSize(int index, double minSize);
		
		/// <summary>
		/// Sets the maximum width or height of a column or row at a given index.
		/// </summary>
		/// <param name="index">The index of the column or row.</param>
		/// <param name="maxSize">The maximum width or height.</param>
		void SetMaxSize(int index, double maxSize);
		
		/// <summary>
		/// Retrieves the column or row names assigned to a visual element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="columnOrRow1">The first column or row name.</param>
		/// <param name="columnOrRow2">The second column or row name.</param>
		void GetAssignedColumnOrRow(UIElement element, out string columnOrRow1, out string columnOrRow2);
		
		/// <summary>
		/// Sets the physical column or row indices of a given visual element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="index">The lower index.</param>
		/// <param name="span">The number of columns or rows to span.</param>
		void SetPhysicalIndex(UIElement element, int index, int span);
		
		/// <summary>
		/// Enumerates all elements in the <see cref="T:NameBasedGrid.NameBasedGrid"/>.
		/// </summary>
		IEnumerable<UIElement> AllElements { get; }
	}
}
