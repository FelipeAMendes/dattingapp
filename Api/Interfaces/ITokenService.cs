using Api.Entities;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}