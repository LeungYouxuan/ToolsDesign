using QFramework;
using Systems;
using Test;

namespace Queries
{
    public class GetSupermarketQuery:AbstractQuery<BigRunFaSupermarket>
    {
        protected override BigRunFaSupermarket OnDo()
        {
            return this.GetSystem<InventorySystem>().bigRunFaSupermarket;
        }
    }
}