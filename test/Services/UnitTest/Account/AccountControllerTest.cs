using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Account
{
    using Xunit;
    using Moq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Authentication;
    using System.Security.Claims;

    public class AccountControllerTest
    {
        private readonly Mock<HttpContext> _httpContextMock;

        public AccountControllerTest()
        {
            this._httpContextMock = new Mock<HttpContext>();
        }

        public void Singh_with_token_success()
        {
            var fakeCP = this.GenerateFakeClaimsIdentity();
            var mockAuth = new Mock<AuthenticationManager>();

            this._httpContextMock.Setup(h => h.User)
                .Returns(new ClaimsPrincipal(fakeCP));
            this._httpContextMock.Setup(h => h.Authentication)
                .Returns(mockAuth.Object);
        }

        private ClaimsIdentity GenerateFakeClaimsIdentity()
        {
            var ci = new ClaimsIdentity();
            ci.AddClaim(new Claim("access_token", "fakeToken"));
            return ci;
        }
    }
}
