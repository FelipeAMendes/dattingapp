namespace Api.Entities
{
    public class UserLike
    {
        public int SourceUserId { get; set; }
        public int LikedUserId { get; set; }

        public virtual AppUser SourceUser { get; set; }
        public virtual AppUser LikedUser { get; set; }
    }
}