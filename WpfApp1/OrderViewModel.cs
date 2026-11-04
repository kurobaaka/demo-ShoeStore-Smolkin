using System;

namespace WpfApp1
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public string Article { get; set; }

        public string Status { get; set; }

        public string PickupAddress { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }
}
