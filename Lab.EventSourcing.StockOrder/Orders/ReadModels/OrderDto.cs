using System;

namespace Lab.EventSourcing.StockOrder
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public OrderSide Side { get; set; }
        public string Symbol { get; set; }
        public uint Quantity { get; set; }
        public decimal Price { get; set; }
        public OrderStatus Status { get; set; }
        public uint ExecutedQuantity { get; set; }
        public uint LeavesQuantity { get; set; }
    }
}
