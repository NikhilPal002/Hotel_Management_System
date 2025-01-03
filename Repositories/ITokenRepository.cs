using Hotel_Management.Models;

namespace Hotel_Management.Repositories
{
    public interface ITokenRepository{
       string CreateJwtToken(User user,string role);
    }
}