// <copyright file="IRepository.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Data.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SmartWineRack.Data.Dto;

    /// <summary>
    /// The interface for repositories.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets the bottles in the database.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<BottleDto>> GetBottles();

        /// <summary>
        /// Adds a bottle to the database.
        /// </summary>
        /// <param name="bottle">The bottle to add.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AddBottle(BottleDto bottle);

        /// <summary>
        /// Removes a bottle from the database.
        /// </summary>
        /// <param name="slot">The slot number the bottle is stored in.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RemoveBottle(int slot);

        /// <summary>
        /// Saves the wine rack's configuration to the database.
        /// </summary>
        /// <param name="configDto">The DTO representing the configuration.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task SaveConfig(WineRackConfigDto configDto);

        /// <summary>
        /// Gets the wine rack's configuration.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing the DTO representing the configuration.</returns>
        Task<WineRackConfigDto> GetConfig();
    }
}
