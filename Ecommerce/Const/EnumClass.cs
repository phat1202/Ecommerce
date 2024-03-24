using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Const
{
    public class EnumClass
    {
        public enum Gender
        {
            [Display(Name = "Male")]
            Male = 0,
            [Display(Name = "Female")]
            Female = 1,
            [Display(Name = "Other")]
            Other = 2,
        }
        public enum Role
        {
            [Display(Name = "Admin")]
            Admin = 0,
            [Display(Name = "Staff")]
            Staff = 1,
            [Display(Name = "User")]
            User = 2,
        }
        public enum OrderStatus
        {
            [Display(Name = "Pending")]
            Pending = 0,
            [Display(Name = "Processing")]
            Processing = 1,
            [Display(Name = "Shipped")]
            Shipped = 2,
            [Display(Name = "Delivered")]
            Delivered = 3,
            [Display(Name = "Canceled")]
            Canceled = 4,

        }
        public enum PaymentMethod
        {
            [Display(Name = "Paypal")]
            Paypal = 0,
            [Display(Name = "Stripe")]
            Stripe = 1
        }
    }
}
