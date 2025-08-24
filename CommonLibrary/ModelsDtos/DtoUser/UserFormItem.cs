using CommonLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.ModelsDtos.DtoUser
{
    public class UserFormItem
    {
        public UserFormItem() { }
        public UserFormItem(User user)
        {
            Identifier = user.Identifier;
            Email = user.Email;
        }

        public string? Identifier { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
