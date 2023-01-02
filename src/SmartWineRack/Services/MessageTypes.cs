// <copyright file="MessageTypes.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

namespace SmartWineRack.Services
{
    public enum MessageTypes
    {
        OnboardTwin = 0,
        BottleAdded,
        BottleRemoved,
        BottleScanned,
        BottleReturned,
    }
}
