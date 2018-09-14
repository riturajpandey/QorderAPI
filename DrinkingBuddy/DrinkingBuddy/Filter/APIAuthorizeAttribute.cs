using DrinkingBuddy.Entities;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;


namespace DrinkingBuddy.Filter
{
    public class APIAuthorizeAttribute : AuthorizeAttribute
    {
        private DrinkingBuddyEntities db = new DrinkingBuddyEntities();
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            if (Authorize(filterContext))
            {
                return;
            }
            HandleUnauthorizedRequest(filterContext);
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        private bool Authorize(HttpActionContext actionContext)
        {
            try
            {
                var encodedString = actionContext.Request.Headers.GetValues("Token").First();
                var userdetails = actionContext.Request.Headers.GetValues("Email").First();


                bool validFlag = false;

                if (!string.IsNullOrEmpty(encodedString))
                {
                    var token = db.PatronsSessionTokens.Where(m => m.SessionToken.ToString() == encodedString).FirstOrDefault();

                    var UserID = token.PatronID;                  // UserID
                    DateTime IssuedOn = token.DateTimeGiven;      // Issued Time

                    var patron = db.Patrons.Where(m => m.PatronsID == UserID).FirstOrDefault();

                    //validating The Patron
                    if (patron.EmailAddress==userdetails)
                    {
                        // Validating Time
                        var ExpiresOn = token.DateTimeExpiry;

                        if ((DateTime.Now > ExpiresOn))
                        {
                            validFlag = false;
                        }
                        else
                        {
                            validFlag = true;
                        }

                    }
                    else
                    {
                        validFlag = false;

                    }
                   
                }

                else
                {
                    validFlag = false;
                }

               return validFlag;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }


}
