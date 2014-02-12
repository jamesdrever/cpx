namespace CustomerPortalExtensions.Domain.SocialNetworking
{
    public class Tweet
    {

        public string Id { get; set; }

        public string TweetText { get; set; }

        public string Created { get; set; }

        public string Avatar { get; set; }

        public string Username { get; set; }


        public Tweet()
        {

        }


        public Tweet(string id, string text, string created, string avatar, string username)
        {

            this.Id = id;

            this.TweetText = text;

            this.Created = created;

            this.Avatar = avatar;

            this.Username = username;

        }

    }
}