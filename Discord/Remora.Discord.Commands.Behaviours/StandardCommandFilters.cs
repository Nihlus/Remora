//
//  StandardCommandFilters.cs
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

using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using JetBrains.Annotations;

namespace Remora.Discord.Commands.Behaviours
{
    /// <summary>
    /// Contains various standard command filters for use with a command behaviour.
    /// </summary>
    [PublicAPI]
    public static class StandardCommandFilters
    {
        /// <summary>
        /// Determines whether the invoker is or is not an actual user.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull, Pure]
        public static async Task<bool> IsUserAsync([NotNull] SocketCommandContext context)
        {
            return !await IsBotAsync(context) && !await IsWebhookAsync(context);
        }

        /// <summary>
        /// Determines whether the invoker is a bot.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull, Pure]
        public static Task<bool> IsBotAsync([NotNull] SocketCommandContext context)
        {
            return Task.FromResult(context.User.IsBot);
        }

        /// <summary>
        /// Determines whether the invoker is a webhook.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull, Pure]
        public static Task<bool> IsWebhookAsync([NotNull] SocketCommandContext context)
        {
            return Task.FromResult(context.User.IsWebhook);
        }

        /// <summary>
        /// Determines whether the message is sufficiently long.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="minLength">The minimum length of the entire message, including the command prefix.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull, Pure]
        public static Task<bool> IsSufficientlyLong([NotNull] SocketCommandContext context, int minLength = 2)
        {
            return Task.FromResult(context.Message.Content.Length >= minLength);
        }

        /// <summary>
        /// Determines whether the message contains at least one letter or digit.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull, Pure]
        public static Task<bool> ContainsAtLeastOneLetterOrDigit([NotNull] SocketCommandContext context)
        {
            return Task.FromResult(context.Message.Content.Any(char.IsLetterOrDigit));
        }
    }
}
