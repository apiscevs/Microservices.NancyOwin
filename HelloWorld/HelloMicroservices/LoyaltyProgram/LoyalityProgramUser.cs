namespace LoyaltyProgram
{
    public class LoyalityProgramUser
    {
        private int userId;

        public LoyalityProgramUser()
        {

        }

        public LoyalityProgramUser(int userId)
        {
            this.userId = userId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int LoyaltyPoints { get; set; }
        public LoyaltyProgramSettings Settings { get; set; }
    }

    public class LoyaltyProgramSettings
    {
        public string[] Interests { get; set; }
    }
}