//
//  Result.cs
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

#pragma warning disable SA1402

namespace Remora.Results
{
    /// <inheritdoc />
    [PublicAPI]
    public readonly struct Result : IResult
    {
        /// <inheritdoc />
        public bool IsSuccess { get; }

        /// <inheritdoc />
        public IResult? InnerResult { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> struct.
        /// </summary>
        /// <param name="isSuccess">Whether the result was successful.</param>
        /// <param name="innerResult">The inner result, if any.</param>
        private Result(bool isSuccess, IResult? innerResult)
        {
            this.IsSuccess = isSuccess;
            this.InnerResult = innerResult;
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <returns>The successful result.</returns>
        public static Result FromSuccess() => new (true, default);

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        /// <param name="innerResult">The inner result, if any.</param>
        /// <returns>The failed result.</returns>
        public static Result FromError(IResult? innerResult = default) => new (false, innerResult);

        /// <summary>
        /// Converts a boolean value into a result.
        /// </summary>
        /// <param name="isSuccess">Whether the result was successful.</param>
        /// <returns>The result.</returns>
        public static implicit operator Result(bool isSuccess)
        {
            return new (isSuccess, default);
        }
    }

    /// <inheritdoc />
    [PublicAPI]
    public readonly struct Result<TResultError> : IResult<TResultError> where TResultError : IResultError
    {
        /// <inheritdoc />
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public IResult? InnerResult { get; }

        /// <inheritdoc />
        public TResultError? Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TResultError}"/> struct.
        /// </summary>
        /// <param name="isSuccess">Whether the result was successful.</param>
        /// <param name="error">The error information, if any.</param>
        /// <param name="innerResult">The inner result, if any.</param>
        private Result(bool isSuccess, TResultError? error, IResult? innerResult)
        {
            this.IsSuccess = isSuccess;
            this.Error = error;
            this.InnerResult = innerResult;
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <returns>The successful result.</returns>
        public static Result<TResultError> FromSuccess() => new (true, default, default);

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        /// <param name="error">The error information.</param>
        /// <param name="innerResult">The inner result, if any.</param>
        /// <returns>The failed result.</returns>
        public static Result<TResultError> FromError(TResultError error, IResult? innerResult = default)
            => new (false, error, innerResult);

        /// <summary>
        /// Converts an error instance to a failed result.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The failed result.</returns>
        public static implicit operator Result<TResultError>(TResultError error)
        {
            return FromError(error);
        }
    }

    /// <inheritdoc />
    [PublicAPI]
    public readonly struct Result<TResultError, TResultValue> : IResult<TResultError, TResultValue>
        where TResultError : IResultError
    {
        /// <inheritdoc />
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public IResult? InnerResult { get; }

        /// <inheritdoc />
        public TResultError? Error { get; }

        /// <inheritdoc />
        public TResultValue? Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TResultError, TResultValue}"/> struct.
        /// </summary>
        /// <param name="isSuccess">Whether the result was successful.</param>
        /// <param name="error">The error information, if any.</param>
        /// <param name="value">The produced value, if any.</param>
        /// <param name="innerResult">The inner result, if any.</param>
        private Result(bool isSuccess, TResultError? error, TResultValue? value, IResult? innerResult)
        {
            this.IsSuccess = isSuccess;
            this.Error = error;
            this.Value = value;
            this.InnerResult = innerResult;
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="value">The produced value.</param>
        /// <returns>The successful result.</returns>
        public static Result<TResultError, TResultValue> FromSuccess(TResultValue value)
            => new (true, default, value, default);

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        /// <param name="error">The error information.</param>
        /// <param name="innerResult">The inner result, if any.</param>
        /// <returns>The failed result.</returns>
        public static Result<TResultError, TResultValue> FromError(TResultError error, IResult? innerResult = default)
            => new (false, error, default, innerResult);

        /// <summary>
        /// Converts a value instance to a successful result.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The successful result.</returns>
        public static implicit operator Result<TResultError, TResultValue>(TResultValue value)
        {
            return FromSuccess(value);
        }

        /// <summary>
        /// Converts an error instance to a failed result.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The failed result.</returns>
        public static implicit operator Result<TResultError, TResultValue>(TResultError error)
        {
            return FromError(error);
        }
    }
}
