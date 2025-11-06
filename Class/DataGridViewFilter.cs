// Create a new file: Class/DataGridViewFilter.cs
using System.Data;
using System.Windows.Forms;

namespace ServiceDesk.Class
{
    public class DataGridViewFilter
    {
        private DataTable _sourceData;
        private DataGridView _gridView;

        public DataGridViewFilter(DataGridView gridView)
        {
            _gridView = gridView;
        }

        /// <summary>
        /// Stores the complete dataset that was loaded from the database.
        /// This is our "master" data that we filter from.
        /// </summary>
        public void SetSourceData(DataTable data)
        {
            _sourceData = data.Copy(); // Keep a copy of the original data
        }

        /// <summary>
        /// Filters the DataGridView based on search text across all columns.
        /// This happens entirely in memory without touching the database.
        /// </summary>
        public void ApplyFilter(string searchText)
        {
            if (_sourceData == null || _sourceData.Rows.Count == 0)
                return;

            // Build a filter expression that searches across all columns
            string filterExpression = BuildFilterExpression(searchText);

            // Use DataView for efficient filtering
            DataView dataView = new DataView(_sourceData);
            dataView.RowFilter = filterExpression;

            // Clear the grid and populate with filtered results
            _gridView.Rows.Clear();

            foreach (DataRowView rowView in dataView)
            {
                DataRow row = rowView.Row;
                // Add row to DataGridView
                // The exact columns depend on which form you're using
                AddRowToGrid(row);
            }
        }

        /// <summary>
        /// Builds a filter expression that searches for the text in all columns.
        /// Example: "Column1 LIKE '%search%' OR Column2 LIKE '%search%' OR ..."
        /// </summary>
        private string BuildFilterExpression(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return ""; // No filter, show all data

            // Escape single quotes in search text to prevent SQL injection-like issues
            searchText = searchText.Replace("'", "''");

            var filterParts = new System.Collections.Generic.List<string>();

            foreach (DataColumn column in _sourceData.Columns)
            {
                // Only search string columns to avoid type conversion errors
                if (column.DataType == typeof(string))
                {
                    filterParts.Add($"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchText}%'");
                }
            }

            return string.Join(" OR ", filterParts);
        }

        /// <summary>
        /// Override this method in derived classes to handle specific grid layouts
        /// </summary>
        protected virtual void AddRowToGrid(DataRow row)
        {
            // This will be overridden per form
        }
    }
}