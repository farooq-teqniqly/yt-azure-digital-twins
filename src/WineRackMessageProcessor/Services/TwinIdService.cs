using RandomStringCreator;

namespace WineRackMessageProcessor.Services;

public class TwinIdService : ITwinIdService
{
    private readonly StringCreator idCreator;

    public TwinIdService()
    { 
        idCreator = new StringCreator("abcdefghijkmnpqrstuvwxyz123456789");
    }
    public string CreateId()
    {
        return idCreator.Get(10);
    }
}