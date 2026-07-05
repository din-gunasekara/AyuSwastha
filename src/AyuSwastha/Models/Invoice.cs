using System;
using System.Collections.Generic;
using System.Linq;

namespace AyuSwastha.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public string Notes { get; set; }
        public string PatientName { get; set; }

        public string InvoiceCode => $"INV-{Id:D4}";

        public List<InvoiceItem> Items { get; } = new List<InvoiceItem>();

        public override string ToString()
        {
            string pName = string.IsNullOrEmpty(PatientName) ? $"Patient #{PatientId}" : PatientName;
            return $"INV-{Id:D4} | {pName} - {Total:C2} ({Status})";
        }
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public double UnitPrice { get; set; }
        public double LineTotal { get; set; }
    }

    public enum InvoiceStatus
    {
        Draft,
        Sent,
        Paid,
        Cancelled
    }
}
