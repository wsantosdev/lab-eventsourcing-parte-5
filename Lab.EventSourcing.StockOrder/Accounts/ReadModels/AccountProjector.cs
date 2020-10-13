using Lab.EventSourcing.Core;

namespace Lab.EventSourcing.StockOrder
{
    public class AccountProjector : IProjector<Account>
    {
        private readonly MemoryCache _dtoStore;

        public AccountProjector(MemoryCache dtoStore) =>
            _dtoStore = dtoStore;

        public void Execute(Account model)
        {
            _dtoStore.AddOrUpdate(model.Id, new AccountDto
            {
                Id = model.Id,
                Ballance = model.Ballance
            });
        }
    }
}
