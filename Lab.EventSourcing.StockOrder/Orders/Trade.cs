using System;

namespace Lab.EventSourcing.StockOrder
{
    public class Trade
    {
        public Guid TadeId { get;  private set; }
        public Guid OrderId { get; private set; }
        public uint Quantity { get; private set; }

        public static Trade Create(Guid tradeId, Guid orderId, uint quantity) =>
            new Trade
            {
                TadeId = tradeId,
                OrderId = orderId,
                Quantity = quantity
            };
    }
}
