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
	public sealed class ColumnOrRowList : Collection<ColumnOrRowBase>
	{
		private sealed class PropertyChangeListener : IColumnOrRowPropertyChangeListener
		{
			public PropertyChangeListener(ColumnOrRowList owner)
			{
				if (owner == null) {
					throw new ArgumentNullException("owner");
				}
				
				this.owner = owner;
			}
			
			private readonly ColumnOrRowList owner;
			
			public void PropertyChanged(ColumnOrRowBase columnOrRow, ColumnOrRowProperty property)
			{
				switch (property) {
					case ColumnOrRowProperty.Name:
						owner.InvalidateMaps();
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
		
		internal ColumnOrRowList(IColumnOrRowListController controller)
		{
			if (controller == null) {
				throw new ArgumentNullException("controller");
			}
			
			this.controller = controller;
			this.propertyChangeListener = new PropertyChangeListener(this);
		}
		
		private readonly IColumnOrRowListController controller;
		
		private Dictionary<string, ColumnOrRowBase> namesMap;
		
		private Dictionary<string, int> indexMap;
		
		private void InvalidateMaps()
		{
			namesMap = null;
			indexMap = null;
		}
		
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
		
		private readonly PropertyChangeListener propertyChangeListener;
		
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
			} else {
				var virtColOrRow = item as VirtualColumnOrRow;
				if (virtColOrRow != null) {
					UpdatePlacement();
				} else {
					throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					                                          "Column or row type {0} is not supported.",
					                                          item.GetType()));
				}
			}
		}
		
		protected override void SetItem(int index, ColumnOrRowBase item)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			base.SetItem(index, item);
		}
		
		protected override void ClearItems()
		{
			base.ClearItems();
		}
		
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
		}
		
		internal void UpdatePlacement()
		{
			if (namesMap == null) {
				BuildMaps();
			}
			
			foreach (var child in controller.AllElements) {
				DoUpdatePlacement(child);
			}
		}
		
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
		
		private void DoUpdatePlacement(UIElement element)
		{
			string cr1, cr2;
			controller.GetAssignedColumnOrRow(element, out cr1, out cr2);
			
			int fromIndex, toIndex;
			GetPhysicalRange(cr1, cr2, out fromIndex, out toIndex);
			
			controller.SetPhysicalIndex(element, fromIndex, toIndex - fromIndex + 1);
		}
		
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
