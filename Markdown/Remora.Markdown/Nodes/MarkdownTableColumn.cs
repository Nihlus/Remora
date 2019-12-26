//
//  MarkdownTableColumn.cs
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

using JetBrains.Annotations;

namespace Remora.Markdown
{
    /// <summary>
    /// Represents a column in a table.
    /// </summary>
    [PublicAPI]
    public class MarkdownTableColumn
    {
        /// <summary>
        /// Gets or sets the title of the column.
        /// </summary>
        [PublicAPI, NotNull]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the alignment of the column.
        /// </summary>
        [PublicAPI]
        public ColumnAlignment Alignment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownTableColumn"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        [PublicAPI]
        public MarkdownTableColumn([NotNull] string title)
        {
            this.Title = title;
        }
    }
}
