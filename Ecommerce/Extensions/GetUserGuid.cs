namespace Ecommerce.Extensions
{
    public class GetUserGuid
    {
        public string GetIdCode(string userId)
        {
            string[] parts = userId.Split('-');
            string secondPartWithHyphen = parts[1].ToUpper();
            return secondPartWithHyphen;
        }
    }
}
