// <copyright file="Dependencies.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    public class Dependencies
    {
        public IIdFactory IdFactory { get; set; }

        public IWineRackDbRepository Repository { get; set; }

        public IMessageService MessageService { get; set; }
    }
}