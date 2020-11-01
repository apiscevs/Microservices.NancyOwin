namespace LoyaltyProgram.Store
{
    public interface ILoyaltyProgramUserStore
    {
        LoyalityProgramUser Get(int userId);
        LoyalityProgramUser Save(LoyalityProgramUser shoppingCart);
    }
}