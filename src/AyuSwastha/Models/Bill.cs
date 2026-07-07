using System;
using System.Collections.Generic;
using System.Linq;

namespace AyuSwastha.Models
{
    /// <summary>An invoice for a patient. Owns a list of <see cref="BillItem"/>s (composition).</summary>
    public class Bill
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime IssuedOn { get; set; } = DateTime.Now;
        public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
        public decimal AmountPaid { get; set; }

        public List<BillItem> Items { get; } = new List<BillItem>();

        /// <summary>Sum of every line total.</summary>
        public decimal Total => Items.Sum(i => i.LineTotal);

        public decimal Balance => Total - AmountPaid;

        public void AddItem(BillItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items.Add(item);
        }
    }

    public class BillItem
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string Description { get; set; }  // Consultation, Shirodhara, Triphala...
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        public decimal LineTotal => Quantity * UnitPrice;
    }
}
