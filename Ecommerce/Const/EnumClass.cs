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
    }
}
