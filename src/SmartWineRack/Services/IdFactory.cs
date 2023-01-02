// <copyright file="IdFactory.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    using RandomStringCreator;

    public class IdFactory: IIdFactory
    {
        private readonly StringCreator stringCreator = new ();

        public string CreateId()
        {
            return this.stringCreator.Get(10);
        }
    }
}