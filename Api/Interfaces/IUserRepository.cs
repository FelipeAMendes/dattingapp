using Api.DTOs;
using Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetByIdAsync(int id);
        Task<AppUser> GetByUsernameAsync(string username);
        void Add(AppUser user);
        void Update(AppUser user);
        Task<bool> SaveAllAsync();

        Task<MemberDTO> GetMemberByUserNameAsync(string username);
        Task<IEnumerable<MemberDTO>> GetMembersAsync();
    }
}