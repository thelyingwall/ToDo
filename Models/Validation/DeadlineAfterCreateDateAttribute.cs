using System.ComponentModel.DataAnnotations;

namespace ToDo.Models.Validation
{
    public class DeadlineAfterCreateDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return false;
        }
    }
}
