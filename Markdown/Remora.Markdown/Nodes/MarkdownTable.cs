//
//  MarkdownTable.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Remora.Markdown
{
    /// <summary>
    /// Represents a table of rows.
    /// </summary>
    [PublicAPI]
    public class MarkdownTable : IMarkdownNode
    {
        /// <summary>
        /// Gets the list of rows in the table.
        /// </summary>
        [PublicAPI, ItemNotNull]
        public List<MarkdownTableRow> Rows { get; } = new List<MarkdownTableRow>();

        /// <summary>
        /// Gets the list of columns in the table.
        /// </summary>
        [PublicAPI, ItemNotNull]
        public List<MarkdownTableColumn> Columns { get; } = new List<MarkdownTableColumn>();

        /// <summary>
        /// Appends a column to the table.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The table, with the column appended.</returns>
        [PublicAPI]
        public MarkdownTable AppendColumn(MarkdownTableColumn column)
        {
            this.Columns.Add(column);
            return this;
        }

        /// <summary>
        /// Appends a row to the table.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The table, with the row appended.</returns>
        [PublicAPI]
        public MarkdownTable AppendRow(MarkdownTableRow row)
        {
            this.Rows.Add(row);
            return this;
        }

        /// <inheritdoc />
        public string Compile()
        {
            var sb = new StringBuilder();
            sb.Append("|");

            // Build the header
            foreach (var column in this.Columns)
            {
                sb.Append($" {column.Title} |");
            }

            sb.AppendLine();
            sb.Append("|");
            foreach (var column in this.Columns)
            {
                switch (column.Alignment)
                {
                    case ColumnAlignment.Left:
                    {
                        sb.Append(" --- |");
                        break;
                    }
                    case ColumnAlignment.Right:
                    {
                        sb.Append(" ---: |");
                        break;
                    }
                    case ColumnAlignment.Centered:
                    {
                        sb.Append(" :--- |");
                        break;
                    }
                }
            }

            foreach (var row in this.Rows)
            {
                sb.AppendLine();
                sb.Append("|");
                for (var i = 0; i < this.Columns.Count; ++i)
                {
                    if (i < row.Cells.Count)
                    {
                        sb.Append($" {row.Cells[i].Compile()} |");
                    }
                    else
                    {
                        sb.Append(" |");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
