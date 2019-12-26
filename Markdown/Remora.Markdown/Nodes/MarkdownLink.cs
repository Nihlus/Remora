//
//  MarkdownLink.cs
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
    /// Represents a link.
    /// </summary>
    [PublicAPI]
    public class MarkdownLink : IMarkdownNode
    {
        /// <summary>
        /// Gets or sets the link destination.
        /// </summary>
        [PublicAPI, NotNull]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the visible text of the link.
        /// </summary>
        [PublicAPI, NotNull]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the link's hover tooltip.
        /// </summary>
        [PublicAPI, CanBeNull]
        public string? Tooltip { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownLink"/> class.
        /// </summary>
        /// <param name="destination">The link destination.</param>
        /// <param name="text">The link text.</param>
        [PublicAPI]
        public MarkdownLink([NotNull] string destination, [NotNull] string text)
        {
            this.Destination = destination;
            this.Text = text;
        }

        /// <inheritdoc />
        public virtual string Compile()
        {
            if (string.IsNullOrWhiteSpace(this.Tooltip))
            {
                return $"[{this.Text}]({this.Destination})";
            }

            return $"[{this.Text}]({this.Destination} \"{this.Tooltip}\")";
        }
    }
}
