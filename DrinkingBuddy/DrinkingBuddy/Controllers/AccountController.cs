using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DrinkingBuddy.Models;
using DrinkingBuddy.Entities;
using DrinkingBuddy.Providers;
using DrinkingBuddy.Results;
using AutoMapper;
using System.IO;
using System.Net;
using System.Text;

namespace DrinkingBuddy.Controllers
{

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(LoginModel model)
        {
            try
            {
                if (model.Password != null && model.Email != null)
                {
                    var user = _context.Patrons.Where(m => m.EmailAddress == model.Email && m.Gassword == model.Password).FirstOrDefault();
                    if (user != null)
                    {
                        var token = GetTokenForAPI(user);
                        var config = new MapperConfiguration(cfg =>
                        {

                            cfg.CreateMap<Patron, UserInformationModel>()
                            .ForMember(m => m.Token, option => option.Ignore());

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<UserInformationModel>(user);

                        if (data.Address == null)
                        { data.Address = ""; }
                        else { data.Address = data.Address;}
                        if (data.Suburb == null)
                        { data.Suburb = ""; }
                        else { data.Suburb = data.Suburb; }
                        if (data.PostCode == null)
                        { data.PostCode = ""; }
                        else { data.PostCode = data.PostCode; }
                        data.PhoneNumber = user.PhoneMobile;
                        data.Token = token;
                        if (data.StateId == null)
                        {
                            data.StateId = 0;
                        }
                        else
                        {
                            data.StateId = data.StateId;
                        }

                        Patron _Patron = new Patron();
                        _Patron = user;
                        _Patron.DeviceToken = model.DeviceToken;
                        _Patron.DeviceType = model.DeviceType;
                        _Patron.LastLogOn = model.LastLogOn;

                        _context.Entry(_Patron).State = EntityState.Modified;
                        int result = _context.SaveChanges();

                        return Ok(new ResponseModel { Message = "Login succeeded", Status = "Success", Data = data });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "User Does not Exist", Status = "Failed" });
                    }

                }
                else
                {
                    return BadRequest("Passed Parameter Are Not valid");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
               

            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        [HttpPost]
        [Route("UserById/{id:int}")]
        public IHttpActionResult UserById(int PatronsId)
        {
            if (PatronsId != 0)
            {
                var result = _context.Patrons.Where(m => m.PatronsID == PatronsId).FirstOrDefault();
                if (result != null)
                {

                    return Ok(new ResponseModel { Message = "Request Executed Successfully", Status = "Success", Data = result });
                }
                else
                {
                    return Ok(new ResponseModel { Message = "Request Execution Failed", Status = "Failed", Data = result });

                }
            }
            else
            {
                return BadRequest();

            }

        }


        [HttpPost]
        [Route("Update")]
        public IHttpActionResult Update(UserUpdateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Isexist = _context.Patrons.Find(model.PatronsID);

                    if (Isexist != null)
                    {
                        Isexist.EmailAddress = model.EmailAddress;
                        Isexist.FirstName = model.FirstName;
                        Isexist.LastName = model.LastName;
                        Isexist.PhoneMobile = model.PhoneMobile;
                        Isexist.Address = model.Address;
                        Isexist.Suburb = model.Suburb;
                        Isexist.PostCode = model.PostCode;
                        Isexist.DateOfBirth = model.DateOfBirth;

                        _context.Entry(Isexist).State = EntityState.Modified;
                        int result = _context.SaveChanges();
                        if (result != 0)
                        {
                            var Updated = _context.Patrons.Where(m => m.PatronsID == model.PatronsID).FirstOrDefault();

                            UserInformationModel UserModel = new UserInformationModel();
                            UserModel.PatronsID = Updated.PatronsID;
                            UserModel.EmailAddress = Updated.EmailAddress;
                            UserModel.FirstName = Updated.FirstName;
                            UserModel.Address = Updated.Address;
                            UserModel.Suburb = Updated.Suburb;
                            UserModel.PostCode = Updated.PostCode;
                            UserModel.StateId = Updated.StateID;
                            UserModel.LastName = Updated.LastName;
                            UserModel.PhoneNumber = Updated.PhoneMobile;
                            return Ok(new ResponseModel { Message = "The User Updated Successfully", Status = "Success", Data = UserModel });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "The User Updated Successfully", Status = "Failed" });
                        }
                    }
                    else
                    {
                        return BadRequest("User Does not Exist");
                    }
                }
                else
                {
                    List<ModelState> Modelvalues = ModelState.Values.ToList();
                    List<string> mesages = new List<string>();
                    foreach (var item in Modelvalues)
                    {
                        var error = item.Errors.ToList();
                        foreach (var itom in error)
                        {
                            string message = itom.ErrorMessage;
                            mesages.Add(message);

                        }
                    }

                    return Ok(new ResponseModel { Message = "Validation Error", Status = "Failed", Data = mesages });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patron = _context.Patrons.Where(m => m.PatronsID == model.PatronsID).FirstOrDefault();
            var user = UserManager.Find(patron.EmailAddress,patron.Gassword);
            IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword,model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            else
            {
                Patron _patron = _context.Patrons.Where(m=>m.PatronsID==model.PatronsID).FirstOrDefault();
                patron.Gassword = model.NewPassword;

                _context.Entry(_patron).State = EntityState.Modified;
                int _result = _context.SaveChanges();
                if (_result>0)
                {
                    return Ok(new ResponseModel { Message = "Password Changed Successfully.", Status = "Success"});
                }
                else
                {
                    return BadRequest("The Password could not be updated.");
                }

                
            }

           
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        [HttpGet]
        [Route("GetVersion")]
        public IHttpActionResult GetVersion()
        {
            var version = _context.TempVersions.FirstOrDefault();
            if (version != null)
            {

                return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = version });
            }
            else
            {
                return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var applicationuser = new ApplicationUser { Email = model.EmailAddress, FirstName = model.FirstName, LastName = model.LastName, UserName = model.EmailAddress, PhoneNumber = model.PhoneNumber };
                    var resultuser = await UserManager.CreateAsync(applicationuser, model.Password);

                    if (resultuser.Succeeded)
                    {

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<RegisterBindingModel, Patron>()
                            .ForMember(m => m.Gassword, option => option.Ignore());
                            cfg.CreateMap<Patron, UserInformationModel>()
                            .ForMember(m => m.Token, option => option.Ignore());

                        });

                        IMapper mapper = config.CreateMapper();
                        var data = mapper.Map<Patron>(model);
                        data.RegisterOn = System.DateTime.Now;
                        data.Gassword = model.Password;
                        data.PhoneMobile = model.PhoneNumber;
                        _context.Patrons.Add(data);
                        int result = _context.SaveChanges();

                        if (result > 0)
                        {

                            var user = _context.Patrons.Where(m => m.EmailAddress == model.EmailAddress).FirstOrDefault();
                            if (user != null)
                            {
                                var Usermodel = mapper.Map<UserInformationModel>(user);
                                Usermodel.Address = "";
                                Usermodel.Suburb = "";
                                Usermodel.PostCode = "";
                                if (user.PhoneMobile == null)
                                { Usermodel.PhoneNumber = ""; }
                                else { Usermodel.PhoneNumber = user.PhoneMobile; }
                                Usermodel.Token = "";
                                Usermodel.StateId = 0;                              
                                return Ok(new ResponseModel { Message = "User have been Registered Sucessgully", Status = "Success", Data = Usermodel });
                            }
                            else
                            {
                                return Ok(new ResponseModel { Message = "User Registration Failed", Status = "Failed" });
                            }
                        }
                        else
                        {
                            return BadRequest("SomeThing Went Wrong");
                        }

                    }
                    else
                    {
                        List<string> mesages = new List<string>();
                        foreach (string error in resultuser.Errors)
                        {
                            mesages.Add(error);
                        }
                        return Ok(new ResponseModel { Message = "Validation Error", Status = "Failed", Data = mesages });
                    }

                }
                else
                {
                    List<ModelState> Modelvalues = ModelState.Values.ToList();
                    List<string> mesages = new List<string>();
                    foreach (var item in Modelvalues)
                    {
                        var error = item.Errors.ToList();
                        foreach (var itom in error)
                        {
                            string message = itom.ErrorMessage;
                            mesages.Add(message);

                        }
                    }

                    return Ok(new ResponseModel { Message = "Validation Error", Status = "Failed", Data = mesages });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        public string GetTokenForAPI(Patron objAuthModel)
        {
            try
            {
                PatronsSessionToken Token = new PatronsSessionToken();
                Token.SessionToken = Guid.NewGuid();
                Token.PatronID = objAuthModel.PatronsID;
                Token.DateTimeGiven = DateTime.Now;
                Token.DateTimeExpiry = Token.DateTimeGiven.AddDays(1);

                _context.PatronsSessionTokens.Add(Token);
                _context.SaveChanges();

                return Token.SessionToken.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion
    }
}

