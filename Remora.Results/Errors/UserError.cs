//
//  UserError.cs
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

namespace Remora.Results
{
    /// <summary>
    /// Represents a simple human-readable error message.
    /// </summary>
    public class UserError : ResultError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserError"/> class.
        /// </summary>
        /// <param name="message">The human-readable error message.</param>
        public UserError(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a user error from a human-readable message.
        /// </summary>
        /// <param name="message">The human-readable error message.</param>
        /// <returns>The error.</returns>
        public static implicit operator UserError(string message) => new (message);
    }
}