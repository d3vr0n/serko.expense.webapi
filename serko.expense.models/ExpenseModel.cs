using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace serko.expense.models
{
    /// <summary>
    /// Expense model holds a single    
    /// </summary>
    [XmlRoot("expense",Namespace = "")]
    public class ExpenseModel
    {
        /// <summary>Cost center of this expense </summary>
        [XmlElement(ElementName = "cost_centre")]
        public string CostCentre { get; set; }
        /// <summary>Total Cost of this expense including GST (input)</summary>
        [XmlElement(ElementName = "total")]
        public decimal? Total { get; set; }
        /// <summary>GST for this expense (calculated)</summary>
        public decimal GST { get; set; }
        /// <summary>Total Value excluding GST for this expense (calculated)</summary>
        public decimal TotalExcludingGST { get; set; }
        /// <summary>Payment for this expense (ex. Personal card, Paypal, SWIFT)</summary>
        [XmlElement(ElementName = "payment_method")]
        public string PaymentMethod { get; set; }
    }
}
