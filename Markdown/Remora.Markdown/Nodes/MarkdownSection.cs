//
//  MarkdownSection.cs
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
    /// Represents a section of markdown content with a header.
    /// </summary>
    [PublicAPI]
    public class MarkdownSection : IMarkdownNode
    {
        private readonly List<IMarkdownNode> _content = new List<IMarkdownNode>();

        /// <summary>
        /// Gets or sets the header of the section.
        /// </summary>
        public MarkdownHeader Header { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownSection"/> class.
        /// </summary>
        /// <param name="title">The title of the section.</param>
        /// <param name="level">The level of the section header.</param>
        public MarkdownSection(string title, int level = 1)
        {
            this.Header = new MarkdownHeader(title, level);
        }

        /// <summary>
        /// Appends a piece of content to the section.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The section, with the content appended.</returns>
        public MarkdownSection AppendContent(IMarkdownNode content)
        {
            _content.Add(content);
            return this;
        }

        /// <summary>
        /// Appends a range of content to the section.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The section, with all the content appended.</returns>
        public MarkdownSection AppendContentRange(IEnumerable<IMarkdownNode> content)
        {
            foreach (var node in content)
            {
                AppendContent(node);
            }

            return this;
        }

        /// <inheritdoc />
        public string Compile()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this.Header.Compile());
            foreach (var contentNode in _content)
            {
                sb.AppendLine(contentNode.Compile());
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }
    }
}
