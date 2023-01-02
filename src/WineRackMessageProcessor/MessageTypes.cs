namespace WineRackMessageProcessor
{
    public static class MessageTypes
    {
        public static string OnboardTwin = "onboardtwin";
        public static string BottleAdded = "bottleadded";
        public static string BottleRemoved = "bottleremoved";
        public static string BottleScanned = "bottlescanned";
    }

    public static class Relationships
    {
        public static string OwnedBy = "ownedBy";
        public static string PartOf = "partOf";
        public static string AttachedTo = "attachedTo";
        public static string StoredIn = "storedIn";
    }
}
