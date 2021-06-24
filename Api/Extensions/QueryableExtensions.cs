using Api.Entities;
using System;
using System.Linq;

namespace Api.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<Message> MarkUnreadAsRead(this IQueryable<Message> query, string currentUsername)
        {
            var unreadMessages = query.Where(m =>
                m.DateRead == null &&
                m.RecipientUsername == currentUsername);

            if (!unreadMessages.Any()) return query;

            foreach (var message in unreadMessages)
                message.DateRead = DateTime.UtcNow;

            return query;
        }
    }
}
