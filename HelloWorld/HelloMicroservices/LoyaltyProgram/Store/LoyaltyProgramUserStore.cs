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
            if (!database.ContainsKey(userId))
                database[userId] = new LoyalityProgramUser(userId);
            return database[userId];
        }

        public void Save(LoyalityProgramUser shoppingCart)
        {
            // Nothing needed. Saving would be needed with a real DB
        }
    }
}
