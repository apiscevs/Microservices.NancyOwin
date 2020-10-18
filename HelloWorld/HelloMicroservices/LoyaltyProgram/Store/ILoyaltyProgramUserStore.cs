namespace LoyaltyProgram.Store
{
    public interface ILoyaltyProgramUserStore
    {
        LoyalityProgramUser Get(int userId);
        void Save(LoyalityProgramUser shoppingCart);
    }
}