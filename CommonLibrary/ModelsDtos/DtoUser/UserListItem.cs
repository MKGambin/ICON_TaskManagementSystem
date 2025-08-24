using CommonLibrary.Models;

namespace CommonLibrary.ModelsDtos.DtoUser
{
    public class UserListItem
    {
        public UserListItem() { }
        public UserListItem(User user)
        {
            Identifier = user.Identifier;
            Email = user.Email;
        }

        public string? Identifier { get; set; }
        public string? Email { get; set; }
    }
}
