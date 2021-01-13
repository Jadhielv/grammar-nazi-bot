﻿using System.Threading.Tasks;

namespace GrammarNazi.Domain.Services
{
    public interface ITwitterLogService
    {
        Task<long> GetLastTweetId();

        Task LogReply(long tweetId, long replyTweetId);

        Task LogTweet(long tweetId);

        Task<bool> TweetExist(long tweetId);
    }
}