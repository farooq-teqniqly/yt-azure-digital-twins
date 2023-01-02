// <copyright file="DependenciesBinder.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using System.CommandLine.Binding;
    using Microsoft.EntityFrameworkCore;
    using SmartWineRack.Db;

    public class DependenciesBinder : BinderBase<Dependencies>
    {
        protected override Dependencies GetBoundValue(BindingContext bindingContext)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WineRackDbContext>();
            optionsBuilder.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "swr.db")}");

            var dbContext = new WineRackDbContext(optionsBuilder.Options);
            var idFactory = new IdFactory();

            dbContext.Database.Migrate();

            var repository = new WineRackDbRepository(dbContext, idFactory);
            var messageService = new MessageService(repository);

            return new Dependencies
            {
                IdFactory = idFactory,
                Repository = repository,
                MessageService = messageService,
            };
        }
    }
}