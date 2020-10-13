using Lab.EventSourcing.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab.EventSourcing.StockOrder
{
    public class Order : EventSourcingModel<Order>
    {
        public Guid AccountId { get; private set; }
        public OrderSide Side { get; private set; }
        public string Symbol { get; private set; }
        public uint Quantity { get; private set; }
        public decimal Price { get; private set; }
        public OrderStatus Status { get; private set; }
        
        private Queue<Trade> _trades = new Queue<Trade>();
        public IReadOnlyCollection<Trade> Trades { get => _trades; }
        public uint ExecutedQuantity { get => (uint)_trades.Sum(t => t.Quantity); }
        public uint LeavesQuantity { get => Quantity - ExecutedQuantity; }
        
        protected Order(IEnumerable<ModelEventBase> persistedEvents = null) 
            : base(persistedEvents) { }

        public static Order Create(Guid accountId, OrderSide side, string symbol, uint quantity, decimal price)
        {
            var order = new Order();
            order.RaiseEvent(new OrderCreated(Guid.NewGuid(), accountId, side, symbol, quantity, price));
            if (side == OrderSide.Buy)
                order.AddDomainEvent(new BuyOrderCreatedDomainEvent(accountId, quantity * price));

            return order;
        }

        public void Execute(Trade trade)
        {
            if (trade.Quantity > LeavesQuantity)
                throw new InvalidOperationException("This trade quantity overwhelms the order's quantity");

            RaiseEvent(new OrderExecuted(Id, NextVersion, trade));
        }

        public void Cancel()
        {
            if (Status != OrderStatus.New)
                throw new InvalidOperationException("Only new orders can be cancelled.");

            RaiseEvent(new OrderCancelled(Id, NextVersion));

            if(Side == OrderSide.Buy)
                AddDomainEvent(new BuyOrderCancelledDomainEvent(AccountId, Price * Quantity));
        }

        protected override void Apply(IEvent pendingEvent)
        {
            switch (pendingEvent)
            {
                case OrderCreated created:
                    Apply(created);
                    break;
                case OrderExecuted executed:
                    Apply(executed);
                    break;
                case OrderCancelled executed:
                    Apply(executed);
                    break;
                default:
                    throw new ArgumentException($"Unsuported event type {pendingEvent.GetType()}", nameof(pendingEvent));
            }
        }

        private void Apply(OrderCreated created)
        {
            Id = created.ModelId;
            AccountId = created.AccountId;
            Side = created.Side;
            Symbol = created.Symbol;
            Quantity = created.Quantity;
            Price = created.Price;
            Status = OrderStatus.New;
        }

        private void Apply(OrderExecuted executed) =>
            _trades.Enqueue(executed.Trade);

        private void Apply(OrderCancelled cancelled) =>
            Status = OrderStatus.Cancelled;
    }
}