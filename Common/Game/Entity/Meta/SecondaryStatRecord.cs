using System;
using System.Timers;
using NineToFive.Util;

namespace NineToFive.Game.Entity.Meta {
    public class SecondaryStatRecord {
        public User User { get; }
        public  SkillRecord SkillRecord { get; }
        public long TimeExpire { get; }
        public Timer ExpireTimer { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="skillRecord">Reference to record of buff supplying the temporary stat change</param>
        /// <param name="duration">duration in milliseconds</param>
        public SecondaryStatRecord(ref User user, ref SkillRecord skillRecord, int duration) {
            User = user;
            SkillRecord = skillRecord;
            int seconds = duration / 1000;
            TimeExpire = Time.GetFuture(seconds);
            
            (ExpireTimer = new Timer(seconds) {
                AutoReset = false,
                Enabled = true
            }).Elapsed += Expire;
        }

        public void Expire(object o, ElapsedEventArgs e) {
            
            
        }
    }
}