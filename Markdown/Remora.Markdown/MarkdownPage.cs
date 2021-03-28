//
//  MarkdownPage.cs
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
    /// Represents a page of markdown content.
    /// </summary>
    [PublicAPI]
    public class MarkdownPage
    {
        private readonly List<MarkdownSection> _sections = new List<MarkdownSection>();

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the page footer.
        /// </summary>
        public string? Footer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPage"/> class.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="title">The title of the page.</param>
        public MarkdownPage(string name, string title)
        {
            this.Name = name;
            this.Title = title;
        }

        /// <summary>
        /// Appends a section to the page.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns>The page, with the section appended.</returns>
        public MarkdownPage AppendSection(MarkdownSection section)
        {
            _sections.Add(section);
            return this;
        }

        /// <summary>
        /// Compiles the contents of the page into Markdown text.
        /// </summary>
        /// <returns>The page, as markdown text.</returns>
        public string Compile()
        {
            var sb = new StringBuilder();

            sb.AppendLine(new MarkdownHeader(this.Title, 1, true).Compile());
            foreach (var section in _sections)
            {
                sb.AppendLine(section.Compile());
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(this.Footer))
            {
                sb.AppendLine(this.Footer);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
