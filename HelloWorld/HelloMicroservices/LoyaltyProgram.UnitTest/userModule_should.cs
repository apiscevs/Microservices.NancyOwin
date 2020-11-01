using Nancy.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace LoyaltyProgram.UnitTest
{
    public class UserModule_should
    {
        private Browser sut;
        public UserModule_should()
        {
            this.sut = new Browser(
            new Bootstrapper(),
            defaultsTo => defaultsTo.Accept("application/json"));
        }
        [Fact]
        public async void respond_not_found_when_queried_for_unregistered_user()
        {
            var actual = await sut.Get("/users/1000");
            Assert.Equal(HttpStatusCode.NotFound.ToString(), actual.StatusCode.ToString());
        }

        [Fact]
        public async void allow_to_register_new_user()
        {
            var expected =
            new LoyalityProgramUser() { Name = "Chr" };
            var registrationResponse = await
                sut.Post("/users", with => with.JsonBody(expected));
            var newUser =
            registrationResponse.Body.DeserializeJson<LoyalityProgramUser>();
            var actual = await sut.Get($"/users/{newUser.Id}");
            Assert.Equal(HttpStatusCode.OK.ToString(), actual.StatusCode.ToString());
            Assert.Equal(
            expected.Name,
            actual.Body.DeserializeJson<LoyalityProgramUser>().Name);
            // more assertions on the response from the GET
        }

        [Fact]
        public async void allow_modifying_users()
        {
            var expected = "jane";
            var user = new LoyalityProgramUser() { Name = "Chr" };
            var registrationResponse = await
            sut.Post("/users", with => with.JsonBody(user));
            var newUser =
            registrationResponse.Body.DeserializeJson<LoyalityProgramUser>();
            newUser.Name = expected;
            var actual = await
            sut.Put($"/users/{newUser.Id}", with => with.JsonBody(newUser));
            Assert.Equal(
            expected,
            actual.Body.DeserializeJson<LoyalityProgramUser>().Name);
        }
    }
}
