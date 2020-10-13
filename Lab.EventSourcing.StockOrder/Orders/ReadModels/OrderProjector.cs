using Lab.EventSourcing.Core;

namespace Lab.EventSourcing.StockOrder
{
    public class OrderProjector : IProjector<Order>
    {
        private readonly MemoryCache _dtoStore;

        public OrderProjector(MemoryCache dtoStore) =>
            _dtoStore = dtoStore;

        public void Execute(Order model)
        {
            _dtoStore.AddOrUpdate(model.Id, new OrderDto
            {
                Id = model.Id,
                AccountId = model.AccountId,
                ExecutedQuantity = model.ExecutedQuantity,
                LeavesQuantity = model.LeavesQuantity,
                Price = model.Price,
                Quantity = model.Quantity,
                Side = model.Side,
                Status = model.Status,
                Symbol = model.Symbol
            });
        }
    }
}
