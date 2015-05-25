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
	public abstract class ColumnOrRowBase : DependencyObject, INotifyPropertyChanged
	{
		internal ColumnOrRowBase()
		{
		}
		
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name",
		                                                                                     typeof(string),
		                                                                                     typeof(ColumnOrRowBase),
		                                                                                     new FrameworkPropertyMetadata(OnNameChanged));
		private static void OnNameChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((ColumnOrRowBase)source).OnNameChanged(e);
		}
		
		private void OnNameChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.Name);
		}
		
		public string Name {
			get {
				return (string)GetValue(NameProperty);
			}
			set {
				SetValue(NameProperty, value);
			}
		}
		
		#region property change notification
		private readonly List<WeakReference> propertyChangeListeners = new List<WeakReference>();
		
		internal void AddPropertyChangeListener(IColumnOrRowPropertyChangeListener listener)
		{
			if (listener == null) {
				throw new ArgumentNullException("listener");
			}
			
			propertyChangeListeners.Add(new WeakReference(listener));
		}
		
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
		
		internal void OnPropertyChanged(ColumnOrRowProperty property)
		{
			switch (property) {
				case ColumnOrRowProperty.Name:
				case ColumnOrRowProperty.Size:
				case ColumnOrRowProperty.SharedSizeGroup:
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
		
		protected void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}
