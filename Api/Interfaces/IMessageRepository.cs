using Api.DTOs;
using Api.Entities;
using Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IMessageRepository
    {
        void Add(Message message);
        void Delete(Message message);
        Task<Message> GetById(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}