using Models;
using QFramework;
using Systems;

public class GameArchitecture:Architecture<GameArchitecture>
{
    protected override void Init()
    {
        
    }
    
    public void RegisterModelCustom()
    {
        this.RegisterModel(new InventoryModel());
    }
    public void RegisterSystemCustom()
    {
        this.RegisterSystem(new InventorySystem());
    }
}