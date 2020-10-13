using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class OrderExecuted : ModelEventBase
    {
        public Trade Trade { get; private set; }

        public OrderExecuted(Guid modelId, int modelVersion, Trade trade)
            : base(modelId, modelVersion) =>
                Trade = trade;
    }
}
