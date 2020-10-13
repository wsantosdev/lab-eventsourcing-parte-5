using Lab.EventSourcing.Core;
using Lab.EventSourcing.DomainEvents;
using System;
using Xunit;

namespace Lab.EventSourcing.StockOrder.Test
{
    public class CqrsCase
    {
        [Fact(DisplayName = "Should retrieve Account DTO with initial deposit")]
        public void ShouldRetrieveAccountDtoWithInitialDeposit()
        {
            //Arrange
            var projectionHost = new ProjectorHost();
            var eventStore = EventStore.Create(projectionHost, new DomainEventDispatcher());
            var dtoStore = MemoryCache.Create();

            projectionHost.Add(new AccountProjector(dtoStore));
            
            //Act
            var account = Account.Create(1_000);
            eventStore.Commit(account);

            //Assert
            var accountDto = dtoStore.Get<AccountDto>(account.Id);
            Assert.Equal(account.Ballance, accountDto.Ballance);
        }

        [Fact(DisplayName = "Should debit Account DTO ballance on Order creation")]
        public void ShouldDebitAccountDtoOnOrderCreation()
        {
            //Arrange
            var projectionHost = new ProjectorHost();
            var domainEventDispatcher = new DomainEventDispatcher();

            var eventStore = EventStore.Create(projectionHost, domainEventDispatcher);
            var dtoStore = MemoryCache.Create();

            projectionHost.Add(new AccountProjector(dtoStore));
            projectionHost.Add(new OrderProjector(dtoStore));

            domainEventDispatcher.RegisterHandler<BuyOrderCreatedDomainEvent>(new BuyOrderCreatedHandler(eventStore));
            domainEventDispatcher.RegisterHandler<BuyOrderCancelledDomainEvent>(new BuyOrderCancelledHandler(eventStore));

            //Act
            var account = Account.Create(1_000);
            eventStore.Commit(account);

            var order = Order.Create(account.Id, OrderSide.Buy, "PETR4", 10, 10M);
            eventStore.Commit(order);

            //Assert
            var orderDto = dtoStore.Get<OrderDto>(order.Id);
            Assert.Equal(order.Symbol, orderDto.Symbol);
            Assert.Equal(order.Quantity, orderDto.Quantity);
            Assert.Equal(order.Price, orderDto.Price);

            var accountDto = dtoStore.Get<AccountDto>(account.Id);
            Assert.Equal(900, accountDto.Ballance);
        }

        [Fact(DisplayName = "Should update leaves quantity on Order DTO after execution")]
        public void ShouldUpdateOrderDtoLeavesQuantityAfterExecution()
        {
            //Arrange
            var projectionHost = new ProjectorHost();

            var domainEventDispatcher = new DomainEventDispatcher();
            var eventStore = EventStore.Create(projectionHost, domainEventDispatcher);
            var dtoStore = MemoryCache.Create();

            domainEventDispatcher.RegisterHandler<BuyOrderCreatedDomainEvent>(new BuyOrderCreatedHandler(eventStore));

            projectionHost.Add(new AccountProjector(dtoStore));
            projectionHost.Add(new OrderProjector(dtoStore));

            //Act
            var account = Account.Create(10_000);
            eventStore.Commit(account);

            var order = Order.Create(account.Id, OrderSide.Buy, "PETR4", 100, 10M);
            order.Execute(Trade.Create(Guid.NewGuid(), order.Id, 75));
            eventStore.Commit(order);

            //Assert
            var accountDto = dtoStore.Get<AccountDto>(account.Id);
            Assert.Equal(9_000, accountDto.Ballance);

            var orderDto = dtoStore.Get<OrderDto>(order.Id);
            Assert.Equal(order.Symbol, orderDto.Symbol);
            Assert.Equal<uint>(75, orderDto.ExecutedQuantity);
            Assert.Equal<uint>(25, orderDto.LeavesQuantity);
        }
    }
}
