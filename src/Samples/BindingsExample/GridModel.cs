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
using System.ComponentModel;
using System.Windows.Input;

namespace NameBasedGrid.Examples.Bindings
{
	public class GridModel : INotifyPropertyChanged
	{
		private readonly ObservableCollection<ColumnOrRow> columns = new ObservableCollection<ColumnOrRow>();
		
		public IList<ColumnOrRow> Columns {
			get {
				return columns;
			}
		}
		
		private ColumnOrRow selectedColumn;
		
		public ColumnOrRow SelectedColumn {
			get {
				return selectedColumn;
			}
			set {
				if (selectedColumn != value) {
					selectedColumn = value;
					OnPropertyChanged("SelectedColumn");
					removeColumn.UpdateCanExecute();
				}
			}
		}
		
		private readonly ObservableCollection<ColumnOrRow> rows = new ObservableCollection<ColumnOrRow>();
		
		public IList<ColumnOrRow> Rows {
			get {
				return rows;
			}
		}
		
		private ColumnOrRow selectedRow;
		
		public ColumnOrRow SelectedRow {
			get {
				return selectedRow;
			}
			set {
				if (selectedRow != value) {
					selectedRow = value;
					OnPropertyChanged("SelectedRow");
					removeRow.UpdateCanExecute();
				}
			}
		}
		
		private sealed class AddItemCommand : ICommand
		{
			public AddItemCommand(ICollection<ColumnOrRow> list)
			{
				if (list == null) {
					throw new ArgumentNullException("list");
				}
				
				this.list = list;
			}
			
			private readonly ICollection<ColumnOrRow> list;
			
			public event EventHandler CanExecuteChanged;
			
			public void Execute(object parameter)
			{
				list.Add(new ColumnOrRow());
			}
			
			public bool CanExecute(object parameter)
			{
				return true;
			}
		}
		
		private sealed class RemoveItemCommand : ICommand
		{
			public RemoveItemCommand(ICollection<ColumnOrRow> list)
			{
				if (list == null) {
					throw new ArgumentNullException("list");
				}
				
				this.list = list;
			}
			
			private readonly ICollection<ColumnOrRow> list;
			
			public event EventHandler CanExecuteChanged;
			
			public void Execute(object parameter)
			{
				list.Remove((ColumnOrRow)parameter);
			}
			
			public bool CanExecute(object parameter)
			{
				return (parameter as ColumnOrRow) != null;
			}
			
			public void UpdateCanExecute()
			{
				if (CanExecuteChanged != null) {
					CanExecuteChanged(this, EventArgs.Empty);
				}
			}
		}
		
		private ICommand addColumn, addRow;
		
		private RemoveItemCommand removeColumn, removeRow;
		
		public ICommand AddColumn {
			get {
				if (addColumn == null) {
					addColumn = new AddItemCommand(columns);
				}
				return addColumn;
			}
		}
		
		public ICommand RemoveColumn {
			get {
				if (removeColumn == null) {
					removeColumn = new RemoveItemCommand(columns);
				}
				return removeColumn;
			}
		}
		
		public ICommand AddRow {
			get {
				if (addRow == null) {
					addRow = new AddItemCommand(rows);
				}
				return addRow;
			}
		}
		
		public ICommand RemoveRow {
			get {
				if (removeRow == null) {
					removeRow = new RemoveItemCommand(rows);
				}
				return removeRow;
			}
		}
		
		private string highlightedColumn, highlightedRow;
		
		public string HighlightedColumn {
			get {
				return highlightedColumn;
			}
			set {
				if (highlightedColumn != value) {
					highlightedColumn = value;
					OnPropertyChanged("HighlightedColumn");
				}
			}
		}
		
		public string HighlightedRow {
			get {
				return highlightedRow;
			}
			set {
				if (highlightedRow != value) {
					highlightedRow = value;
					OnPropertyChanged("HighlightedRow");
				}
			}
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
	}
}
