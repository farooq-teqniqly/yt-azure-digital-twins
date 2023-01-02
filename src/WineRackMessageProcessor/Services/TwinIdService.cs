// <copyright file="TwinIdService.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace WineRackMessageProcessor.Services
{
    using RandomStringCreator;

    public class TwinIdService : ITwinIdService
    {
        private readonly StringCreator idCreator;

        public TwinIdService()
        {
            this.idCreator = new StringCreator("abcdefghijkmnpqrstuvwxyz123456789");
        }

        public string CreateId()
        {
            return this.idCreator.Get(10);
        }
    }
}