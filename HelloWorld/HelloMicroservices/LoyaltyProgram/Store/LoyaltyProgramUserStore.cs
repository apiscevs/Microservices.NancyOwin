using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyProgram.Store
{
    public class LoyaltyProgramUserStore : ILoyaltyProgramUserStore
    {
        private static readonly Dictionary<int, LoyalityProgramUser> database = new Dictionary<int, LoyalityProgramUser>();

        public LoyalityProgramUser Get(int userId)
        {
            if (database.ContainsKey(userId))
                return database[userId];
            return null;
        }

        public LoyalityProgramUser Save(LoyalityProgramUser loyaltyUProgramUser)
        {
            
            if (!database.ContainsKey(loyaltyUProgramUser.Id))
            {
                var userId = new Random(1).Next(1, 1000000);
                var newUser = new LoyalityProgramUser(userId);
                newUser.Name = loyaltyUProgramUser.Name;
                newUser.LoyaltyPoints = loyaltyUProgramUser.LoyaltyPoints;
                newUser.Settings = loyaltyUProgramUser.Settings;
                newUser.Id = userId;

                database[newUser.Id] = newUser;
                return newUser;
            } else
            {
                var user = database[loyaltyUProgramUser.Id];
                user.LoyaltyPoints = loyaltyUProgramUser.LoyaltyPoints;
                user.Name = loyaltyUProgramUser.Name;
                user.Settings = loyaltyUProgramUser.Settings;
                database[loyaltyUProgramUser.Id] = user;
                return user;
            }
        }

    }
}
