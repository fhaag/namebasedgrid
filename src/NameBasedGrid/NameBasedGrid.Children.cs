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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NameBasedGrid
{
	partial class NameBasedGrid
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
		/// Initializes a collection object for the children of the panel.
		/// </summary>
		/// <param name="logicalParent">The logical parent of the items in the collection.</param>
		/// <returns>The newly created collection.</returns>
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			return new ChildCollection(this, grid, logicalParent);
		}
		
		/// <summary>
		/// Returns an enumerator over all logical children of the panel.
		/// </summary>
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return this.grid.Children.GetEnumerator();
			}
		}
	}
}
