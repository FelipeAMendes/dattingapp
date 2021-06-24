using Api.DTOs;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void Add(Message message)
        {
            _context.Messages.Add(message);
        }

        public void Delete(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetById(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u =>
                    u.RecipientUsername == messageParams.Username &&
                    !u.RecipientDeleted),
                "Outbox" => query.Where(u =>
                    u.SenderUsername == messageParams.Username &&
                    !u.SenderDeleted),
                _ => query.Where(u =>
                    u.RecipientUsername == messageParams.Username &&
                    !u.RecipientDeleted &&
                    u.DateRead == null)
            };

            return await PagedList<MessageDTO>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            // var messages = await _context.Messages
            //     .Where(m => m.Recipient.UserName == currentUsername && !m.RecipientDeleted
            //             && m.Sender.UserName == recipientUsername
            //             || m.Recipient.UserName == recipientUsername
            //             && m.Sender.UserName == currentUsername && !m.SenderDeleted
            //     )
            //     .MarkUnreadAsRead(currentUsername)
            //     .OrderBy(m => m.MessageSent)
            //     .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
            //     .ToListAsync();

            // return messages;

            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m =>
                    m.Recipient.UserName == currentUsername &&
                    !m.RecipientDeleted &&
                    m.Sender.UserName == recipientUsername ||
                    m.Recipient.UserName == recipientUsername &&
                    m.Sender.UserName == currentUsername &&
                    !m.SenderDeleted
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m =>
                m.DateRead == null &&
                m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
