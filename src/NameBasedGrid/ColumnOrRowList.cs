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
using System.Collections.ObjectModel;
using System.Windows;

namespace NameBasedGrid
{
	/// <summary>
	/// A collection of <see cref="ColumnOrRowBase">column or row definitions</see>.
	/// </summary>
	public sealed class ColumnOrRowList : ObservableCollection<ColumnOrRowBase>
	{
		/// <summary>
		/// An object that listens to changes of properties of <see cref="ColumnOrRowBase"/> instances.
		/// </summary>
		private sealed class PropertyChangeListener : IColumnOrRowPropertyChangeListener
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="owner">The object that gets notified about the processed changes.</param>
			/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
			public PropertyChangeListener(ColumnOrRowList owner)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			/// <summary>
			/// The object that gets notified about the processed changes.
			/// </summary>
			private readonly ColumnOrRowList owner;
			
			/// <summary>
			/// Processes a change notification.
			/// </summary>
			/// <param name="columnOrRow">The <see cref="ColumnOrRowBase"/> instance whose property value was changed.
			///   This must not be <see langword="null"/>.</param>
			/// <param name="property">The modified property.</param>
			public void PropertyChanged(ColumnOrRowBase columnOrRow, ColumnOrRowProperty property)
			{
				switch (property) {
					case ColumnOrRowProperty.Name:
						owner.InvalidateMaps();
						goto case ColumnOrRowProperty.ExtendTo;
					case ColumnOrRowProperty.StartAt:
					case ColumnOrRowProperty.ExtendTo:
						owner.UpdatePlacement();
						break;
					case ColumnOrRowProperty.Size:
						{
							int physicalIndex = 0;
							for (int i = 0; i < owner.Count; i++) {
								var cr = owner[i] as ColumnOrRow;
								if (cr != null) {
									if (cr == columnOrRow) {
										owner.controller.SetSize(physicalIndex, cr.Size);
									}
									physicalIndex++;
								}
							}
						}
						break;
					case ColumnOrRowProperty.SharedSizeGroup:
						{
							int physicalIndex = 0;
							for (int i = 0; i < owner.Count; i++) {
								var cr = owner[i] as ColumnOrRow;
								if (cr != null) {
									if (cr == columnOrRow) {
										owner.controller.SetSharedSizeGroup(physicalIndex, cr.SharedSizeGroup);
									}
									physicalIndex++;
								}
							}
						}
						break;
				}
			}
		}
		
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="controller">The controller used by the new instance.</param>
		/// <exception cref="ArgumentNullException"><paramref name="controller"/> is <see langword="null"/>.</exception>
		internal ColumnOrRowList(IColumnOrRowListController controller)
		{
			if (controller == null) {
				throw new ArgumentNullException("controller");
			}
			
			this.controller = controller;
			this.propertyChangeListener = new PropertyChangeListener(this);
		}
		
		/// <summary>
		/// The controller used by this instance.
		/// </summary>
		private readonly IColumnOrRowListController controller;
		
		/// <summary>
		/// Maps column or row names to their respective <see cref="ColumnOrRowBase"/> definition objects. 
		/// </summary>
		/// <seealso cref="indexMap"/>
		/// <seealso cref="BuildMaps"/>
		/// <seealso cref="InvalidateMaps"/>
		private Dictionary<string, ColumnOrRowBase> namesMap;
		
		/// <summary>
		/// Maps column or row names to the physical indices in the internal <see cref="System.Windows.Controls.Grid"/>.
		/// </summary>
		/// <seealso cref="namesMap"/>
		/// <seealso cref="BuildMaps"/>
		/// <seealso cref="InvalidateMaps"/>
		private Dictionary<string, int> indexMap;
		
		/// <summary>
		/// Invalidates <see cref="namesMap"/> and <see cref="indexMap"/>.
		/// </summary>
		/// <seealso cref="BuildMaps"/>
		private void InvalidateMaps()
		{
			namesMap = null;
			indexMap = null;
		}
		
		/// <summary>
		/// Rebuilds <see cref="namesMap"/> and <see cref="indexMap"/> based on the current content of the list.
		/// </summary>
		/// <seealso cref="InvalidateMaps"/>
		private void BuildMaps()
		{
			namesMap = new Dictionary<string, ColumnOrRowBase>();
			indexMap = new Dictionary<string, int>();
			int currentIndex = 0;
			for (int i = 0; i < this.Count; i++) {
				var cr = this[i];
				string name = cr.Name;
				
				var physicalColumnOrRow = cr as ColumnOrRow;
				if (physicalColumnOrRow != null) {
					if (name != null) {
						indexMap[name] = currentIndex;
					}
					currentIndex++;
				}
				
				if (name != null) {
					namesMap[name] = cr;
				}
			}
		}
		
		/// <summary>
		/// Retrieves the physical index of a column or row based on its index in this list.
		/// </summary>
		/// <param name="index">The index of the column or row.</param>
		/// <returns>The physical index.</returns>
		/// <remarks>
		/// <para>This method retrieves the physical index of a column or row.
		///   The physical index is the index of the actual column or row definition in the internal <see cref="System.Windows.Controls.Grid"/> control.</para>
		/// </remarks>
		private int GetPhysicalIndex(int index)
		{
			int result = 0;
			for (int i = index - 1; i >= 0; i--) {
				if (this[i] is ColumnOrRow) {
					result++;
				}
			}
			return result;
		}
		
		/// <summary>
		/// The property change listener connected to this instance.
		/// </summary>
		private readonly PropertyChangeListener propertyChangeListener;
		
		/// <summary>
		/// Processes the insertion of an item.
		/// </summary>
		/// <param name="index">The index at which the item was inserted.</param>
		/// <param name="item">The newly inserted item.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
		protected override void InsertItem(int index, ColumnOrRowBase item)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			base.InsertItem(index, item);
			
			item.AddPropertyChangeListener(propertyChangeListener);
			
			var colOrRow = item as ColumnOrRow;
			if (colOrRow != null) {
				controller.ColumnOrRowInserted(GetPhysicalIndex(index), colOrRow);
			}
			InvalidateMaps();
			UpdatePlacement();
		}
		
		/// <summary>
		/// Processes the assignment of an item.
		/// </summary>
		/// <param name="index">The index to which the item was assigned.</param>
		/// <param name="item">The newly assigned item.</param>
		/// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid position in the collection.</exception>
		protected override void SetItem(int index, ColumnOrRowBase item)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			this[index].RemovePropertyChangeListener(propertyChangeListener);
			
			int physicalIndex = GetPhysicalIndex(index);
			
			var oldColumnOrRow = this[index] as ColumnOrRow;
			if (oldColumnOrRow != null) {
				controller.ColumnOrRowRemoved(physicalIndex);
			}
			
			base.SetItem(index, item);
			
			this[index].AddPropertyChangeListener(propertyChangeListener);
			
			var newColumnOrRow = this[index] as ColumnOrRow;
			if (newColumnOrRow != null) {
				controller.ColumnOrRowInserted(physicalIndex, newColumnOrRow);
			}
			
			InvalidateMaps();
			UpdatePlacement();
		}
		
		/// <summary>
		/// Processes the removal of all items.
		/// </summary>
		protected override void ClearItems()
		{
			int physicalColumnOrRowCount = 0;
			for (int i = this.Count - 1; i >= 0; i--) {
				this[i].RemovePropertyChangeListener(propertyChangeListener);
				
				if (this[i] is ColumnOrRow) {
					physicalColumnOrRowCount++;
				}
			}
			for (int i = physicalColumnOrRowCount - 1; i >= 0; i--) {
				controller.ColumnOrRowRemoved(i);
			}
			
			base.ClearItems();
			
			InvalidateMaps();
		}
		
		/// <summary>
		/// Processes the removal of an item at a specific position.
		/// </summary>
		/// <param name="index">The index to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid position in the collection.</exception>
		protected override void RemoveItem(int index)
		{
			this[index].RemovePropertyChangeListener(propertyChangeListener);
			
			if (this[index] is ColumnOrRow) {
				controller.ColumnOrRowRemoved(GetPhysicalIndex(index));
			}
			
			base.RemoveItem(index);
			
			InvalidateMaps();
			UpdatePlacement();
		}
		
		/// <summary>
		/// Updates the placement of all visual elements in the <see cref="T:NameBasedGrid.NameBasedGrid"/>.
		/// </summary>
		/// <remarks>
		/// <para>This method updates the positioning of all visual elements in the <see cref="T:NameBasedGrid.NameBasedGrid"/> along the dimension specified by this list.
		///   The name maps are refreshed before this operation takes place.</para>
		/// </remarks>
		internal void UpdatePlacement()
		{
			if (namesMap == null) {
				BuildMaps();
			}
			
			foreach (var child in controller.AllElements) {
				DoUpdatePlacement(child);
			}
		}
		
		/// <summary>
		/// Updates the placement of a given visual element.
		/// </summary>
		/// <param name="element">The element whose position needs to be updated.</param>
		/// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
		/// <remarks>
		/// <para>This method updates the positioning of <paramref name="element"/> along the dimension specified by this list.
		///   The name maps are refreshed before this operation takes place.</para>
		/// </remarks>
		internal void UpdatePlacement(UIElement element)
		{
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			
			if (namesMap == null) {
				BuildMaps();
			}
			
			DoUpdatePlacement(element);
		}
		
		/// <summary>
		/// Updates the placement of a given visual element based on the current state.
		/// </summary>
		/// <param name="element">The element whose position needs to be updated.
		///   This must not be <see langword="null"/>.</param>
		/// <remarks>
		/// <para>This method updates the positioning of <paramref name="element"/> along the dimension specified by this list.
		///   The name maps are assumed to be up to date.</para>
		/// <para>This method is used by both <see cref="UpdatePlacement()"/> and <see cref="UpdatePlacement(UIElement)"/>.</para>
		/// </remarks>
		private void DoUpdatePlacement(UIElement element)
		{
			string cr1, cr2;
			controller.GetAssignedColumnOrRow(element, out cr1, out cr2);
			
			int fromIndex, toIndex;
			GetPhysicalRange(cr1, cr2, out fromIndex, out toIndex);
			
			controller.SetPhysicalIndex(element, fromIndex, toIndex - fromIndex + 1);
		}
		
		/// <summary>
		/// Determines the physical column or row indices spanned by a range of two column or row names.
		/// </summary>
		/// <param name="columnOrRow1">A column or row name.</param>
		/// <param name="columnOrRow2">Another column or row name.</param>
		/// <param name="fromIndex">The lower retrieved index.</param>
		/// <param name="toIndex">The higher retrieved index.</param>
		private void GetPhysicalRange(string columnOrRow1, string columnOrRow2, out int fromIndex, out int toIndex)
		{
			if (columnOrRow1 != null) {
				if ((columnOrRow2 != null) && (columnOrRow1 != columnOrRow2)) {
					int fromIndex1, fromIndex2, toIndex1, toIndex2;
					GetPhysicalRange(columnOrRow1, out fromIndex1, out toIndex1);
					GetPhysicalRange(columnOrRow2, out fromIndex2, out toIndex2);
					fromIndex = Math.Min(fromIndex1, fromIndex2);
					toIndex = Math.Max(toIndex1, toIndex2);
				} else {
					GetPhysicalRange(columnOrRow1, out fromIndex, out toIndex);
				}
			} else {
				if (columnOrRow2 != null) {
					GetPhysicalRange(columnOrRow2, out fromIndex, out toIndex);
				} else {
					fromIndex = 0;
					toIndex = 0;
				}
			}
		}
		
		/// <summary>
		/// Determines the physical column or row indices spanned by a given column or row name.
		/// </summary>
		/// <param name="columnOrRow">The column or row name.</param>
		/// <param name="fromIndex">The lower retrieved index.</param>
		/// <param name="toIndex">The higher retrieved index.</param>
		private void GetPhysicalRange(string columnOrRow, out int fromIndex, out int toIndex)
		{
			if (namesMap == null) {
				throw new InvalidOperationException("The internal maps are not ready.");
			}
			
			var foundNames = new HashSet<string>();
			var physicalIndices = new List<int>();
			
			var nextNames = new Queue<string>();
			nextNames.Enqueue(columnOrRow);
			
			while (nextNames.Count > 0) {
				string cr = nextNames.Dequeue();
				if (!foundNames.Contains(cr)) {
					int physicalIndex;
					if (indexMap.TryGetValue(cr, out physicalIndex)) {
						physicalIndices.Add(physicalIndex);
					} else {
						ColumnOrRowBase crDef;
						if (namesMap.TryGetValue(cr, out crDef)) {
							var virtualDef = crDef as VirtualColumnOrRow;
							if (virtualDef != null) {
								string start = virtualDef.StartAt;
								if (start != null) {
									nextNames.Enqueue(start);
								}
								
								string end = virtualDef.ExtendTo;
								if ((end != null) && (end != start)) {
									nextNames.Enqueue(end);
								}
							}
						}
					}
					
					foundNames.Add(cr);
				}
			}
			
			if (physicalIndices.Count > 0) {
				fromIndex = int.MaxValue;
				toIndex = int.MinValue;
				
				foreach (int idx in physicalIndices) {
					if (idx < fromIndex) {
						fromIndex = idx;
					}
					if (idx > toIndex) {
						toIndex = idx;
					}
				}
			} else {
				fromIndex = 0;
				toIndex = 0;
			}
		}
	}
}
