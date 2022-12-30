using RandomStringCreator;

namespace SmartWineRack.Services;

public class IdFactory: IIdFactory
{
    private readonly StringCreator stringCreator = new();
        
    public string CreateId()
    {
        return stringCreator.Get(10);
    }
}