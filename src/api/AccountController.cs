using Auth0.AspNetCore.Authentication;
﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AccountController : ControllerBase
{
  public async Task Login(string returnUrl = "/")
  {
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
      // Indicate here where Auth0 should redirect the user after a login.
      // Note that the resulting absolute Uri must be added to the
      // **Allowed Callback URLs** settings for the app.
      .WithRedirectUri(returnUrl)
      .Build();

    await HttpContext.ChallengeAsync(
      Auth0Constants.AuthenticationScheme,
      authenticationProperties
    );
  }


  [Authorize]
  public async Task Logout()
  {
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
      // Indicate here where Auth0 should redirect the user after a logout.
      // Note that the resulting absolute Uri must be added to the
      // **Allowed Logout URLs** settings for the app.
      .WithRedirectUri("/")
      .Build();

    // Logout from Auth0
    await HttpContext.SignOutAsync(
      Auth0Constants.AuthenticationScheme,
      authenticationProperties
    );
    // Logout from the application
    await HttpContext.SignOutAsync(
      CookieAuthenticationDefaults.AuthenticationScheme
    );
  }
}