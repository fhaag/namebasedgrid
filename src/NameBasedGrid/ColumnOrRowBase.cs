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
using System.ComponentModel;
using System.Windows;

namespace NameBasedGrid
{
	/// <summary>
	/// The base class for a column or row definition.
	/// </summary>
	/// <remarks>
	/// <para>Instances of this class serve as a definition of a column or row in which a <see cref="UIElement"/> may be positioned.
	///   The positioning takes place by using the attached properties defined in <see cref="T:NameBasedGrid.NameBasedGrid"/> to refer to the <see cref="Name"/> assigned to a <see cref="ColumnOrRowBase"/> instance.</para>
	/// <para>As columns and rows behave conceptually analogously, they are not any further distinguished by different types.
	///   Instead, the subtypes of this class are <see cref="ColumnOrRow"/> and <see cref="VirtualColumnOrRow"/>.
	///   The former is an atomic column or row that takes some space in the layout of its own.
	///   The latter is an alias that can be defined based on other columns or rows, and then be used like a column or row, respectively, itself.</para>
	/// </remarks>
	public abstract class ColumnOrRowBase : DependencyObject, INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal ColumnOrRowBase()
		{
		}
		
		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		/// <seealso cref="Name"/>
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name",
		                                                                                     typeof(string),
		                                                                                     typeof(ColumnOrRowBase),
		                                                                                     new FrameworkPropertyMetadata(OnNameChanged));
		
		/// <summary>
		/// Processes a change of the <see cref="Name"/> property.
		/// </summary>
		/// <param name="source">The instance whose <see cref="Name"/> property was changed.</param>
		/// <param name="e">An object providing some information about the change.</param>
		private static void OnNameChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((ColumnOrRowBase)source).OnNameChanged(e);
		}
		
		/// <summary>
		/// Processes a change of the <see cref="Name"/> property.
		/// </summary>
		/// <param name="e">An object providing some information about the change.</param>
		private void OnNameChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.Name);
		}
		
		/// <summary>
		/// The name of the column or row.
		/// </summary>
		/// <value>
		/// <para>Gets or sets a name for the column or row.
		///   The name can be an arbitrary string.
		///   As long as it is unique and not <see langword="null"/>, it may be used to place elements in a <see cref="T:NameBasedGrid.NameBasedGrid"/>.</para>
		/// </value>
		public string Name {
			get {
				return (string)GetValue(NameProperty);
			}
			set {
				SetValue(NameProperty, value);
			}
		}
		
		#region property change notification
		/// <summary>
		/// The list of property change listeners.
		/// </summary>
		private readonly List<WeakReference> propertyChangeListeners = new List<WeakReference>();
		
		/// <summary>
		/// Adds a property change listener.
		/// </summary>
		/// <param name="listener">The new listener.</param>
		/// <exception cref="ArgumentNullException"><paramref name="listener"/> is <see langword="null"/>.</exception>
		/// <remarks>
		/// <para>This method adds a property change listener.
		///   The listener will be added by a weak reference, hence no memory leaks are created by adding items to this list.</para>
		/// </remarks>
		/// <seealso cref="RemovePropertyChangeListener"/>
		/// <seealso cref="OnPropertyChanged(ColumnOrRowProperty)"/>
		/// <seealso cref="IColumnOrRowPropertyChangeListener"/>
		internal void AddPropertyChangeListener(IColumnOrRowPropertyChangeListener listener)
		{
			if (listener == null) {
				throw new ArgumentNullException("listener");
			}
			
			propertyChangeListeners.Add(new WeakReference(listener));
		}
		
		/// <summary>
		/// Removes a property change listener.
		/// </summary>
		/// <param name="listener">The listener to remove.</param>
		/// <seealso cref="AddPropertyChangeListener"/>
		/// <seealso cref="OnPropertyChanged(ColumnOrRowProperty)"/>
		/// <seealso cref="IColumnOrRowPropertyChangeListener"/>
		internal void RemovePropertyChangeListener(IColumnOrRowPropertyChangeListener listener)
		{
			for (int i = propertyChangeListeners.Count - 1; i >= 0; i--) {
				var l = propertyChangeListeners[i].Target as IColumnOrRowPropertyChangeListener;
				if (l == null) {
					propertyChangeListeners.RemoveAt(i);
				} else {
					if (l == listener) {
						propertyChangeListeners.RemoveAt(i);
						return;
					}
				}
			}
		}
		
		/// <summary>
		/// Notifies all registered property change listeners about a property change.
		/// </summary>
		/// <param name="property">The changed proeprty.</param>
		/// <exception cref="InvalidEnumArgumentException"><paramref name="property"/> has an invalid value.</exception>
		/// <seealso cref="AddPropertyChangeListener"/>
		/// <seealso cref="RemovePropertyChangeListener"/>
		/// <seealso cref="IColumnOrRowPropertyChangeListener"/>
		internal void OnPropertyChanged(ColumnOrRowProperty property)
		{
			switch (property) {
				case ColumnOrRowProperty.Name:
				case ColumnOrRowProperty.Size:
				case ColumnOrRowProperty.MinSize:
				case ColumnOrRowProperty.MaxSize:
				case ColumnOrRowProperty.SharedSizeGroup:
				case ColumnOrRowProperty.StartAt:
				case ColumnOrRowProperty.ExtendTo:
					break;
				default:
					throw new InvalidEnumArgumentException("property", (int)property, typeof(ColumnOrRowProperty));
			}
			
			for (int i = propertyChangeListeners.Count - 1; i >= 0; i--) {
				var l = propertyChangeListeners[i].Target as IColumnOrRowPropertyChangeListener;
				if (l == null) {
					propertyChangeListeners.RemoveAt(i);
				} else {
					l.PropertyChanged(this, property);
				}
			}
			
			OnPropertyChanged(property.ToString());
		}
		
		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the changed property, or <see langword="null"/> if all properties have changed.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
		
		/// <summary>
		/// Fires the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="e">An object that contains some information about the event.</param>
		/// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		
		/// <summary>
		/// Notifies subscribers when a property has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}
