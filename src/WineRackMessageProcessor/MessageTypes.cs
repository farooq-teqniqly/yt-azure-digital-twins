using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineRackMessageProcessor
{
    public static class MessageTypes
    {
        public static string OnboardTwin = "onboardtwin";
    }

    public static class Relationships
    {
        public static string OwnedBy = "ownedBy";
        public static string PartOf = "partOf";
        public static string AttachedTo = "attachedTo";
    }
}
