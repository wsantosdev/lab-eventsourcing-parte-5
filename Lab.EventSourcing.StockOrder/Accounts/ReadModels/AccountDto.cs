using System;

namespace Lab.EventSourcing.StockOrder
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public decimal Ballance { get; set; }
    }
}
