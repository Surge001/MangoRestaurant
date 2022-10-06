using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Model.Dto;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mango.Services.ShoppingCartApi.Messages
{
    /// <summary>
    /// Members that correspond to CartHeaderDto on the front end.
    /// </summary>
    public class CheckoutHeaderDto : BaseMessage
    {
        /// <summary>
        /// Gets or sets Cart header Id.
        /// </summary>
        public int CartHeaderId { get; set; }

        public string UserId { get; set; }
        public string CouponCode { get; set; }

        public double OrderTotal { get; set; }

        public double DiscountTotal { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickupDateTime { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string CardNumber { get; set; }

        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }

        public int CartTotalItems { get; set; }

        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
