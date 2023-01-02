// <copyright file="IMessageService.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(dynamic body, MessageTypes messageType);

        Task SendBottleMessageAsync(int slotNumber, string upcCode, MessageTypes messageType);
    }
}
