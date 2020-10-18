using LoyaltyProgram.Store;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyProgram
{
    public class UsersModule : NancyModule
    {
        public UsersModule(ILoyaltyProgramUserStore loyaltyProgramUserStore) : base("/users")
        {
            Post("/", _ =>
            {
                var newUser = this.Bind<LoyalityProgramUser>();
                this.AddRegisteredUSer(newUser, loyaltyProgramUserStore);
                return this.CreatedResponse(newUser);
            });

            Put("/{userId:int}", parameters =>
            {
                int userId = parameters.userId;
                var updatedUser = this.Bind<LoyalityProgramUser>();
                //store.update
                return updatedUser;
            });
        }

        private object CreatedResponse(LoyalityProgramUser newUser)
        {
            return this.Negotiate
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Location",
                    this.Request.Url.SiteBase + "/users/" + newUser.Id)
                .WithModel(newUser);
        }

        private void AddRegisteredUSer(LoyalityProgramUser newUser,
            ILoyaltyProgramUserStore loyaltyProgramUserStore)
        {
            loyaltyProgramUserStore.Save(newUser);
        }
    }
}
