// <copyright file="Command.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Commands
{
    using SmartWineRack.Data.Repositories;

    /// <summary>
    /// The base class from which other commands are derived.
    /// </summary>
    /// <typeparam name="TResult">The type of the resulting object returned by the command's execution.</typeparam>
    public abstract class Command<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TResult}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected Command(IRepository repository)
        {
            this.Repository = repository;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        protected IRepository Repository { get; }

        /// <summary>
        /// Executes the command and returns a result.
        /// </summary>
        /// <param name="parameters">The command's parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing the result of the execution.</returns>
        public abstract Task<TResult> Execute(IDictionary<string, object>? parameters = null);
    }
}