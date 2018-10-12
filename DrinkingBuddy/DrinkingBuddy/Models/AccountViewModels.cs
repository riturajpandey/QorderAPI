using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrinkingBuddy.Entities;

namespace DrinkingBuddy.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }


    public class UserInformationModel
    {
        private int _HotelID;
        private string _HotelName;
        private bool _IsCardAvailable;
        private string _state;
        public int PatronsID { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public int? StateId { get; set; }
        public string State
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var state = _context.StatesGs.Where(m => m.StateID == this.StateId).FirstOrDefault();
                    if (state == null)
                    {
                        _state = "";
                    }
                    else
                    {
                        _state = state.StateName;
                    }
                }
                return _state;

            }


        }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public bool IsCardAvailable {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.PatronsPaymentMethods.Where(m => m.PatronID == this.PatronsID).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _IsCardAvailable = false;
                    }
                    else
                    {
                        _IsCardAvailable = true;
                    }
                }
                return _IsCardAvailable;

            }

        }
        public int HotelID
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.PatronsHotelLogIns.Where(m => m.PatronID == this.PatronsID&m.LogoutDateTime==null).FirstOrDefault();
                    if (Catagory == null)
                    {
                        _HotelID = 0;
                    }
                    else
                    {

                        _HotelID = Catagory.HotelID;
                    }
                }
                return _HotelID;

            }


        }
        public string HotelName
        {
            get
            {
                if (this.HotelID!=0)
                {
                    using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                    {
                        var Catagory = _context.Hotels.Where(m => m.HotelID == this.HotelID).FirstOrDefault();
                        if (Catagory == null)
                        {
                            _HotelName = "";
                        }
                        else
                        {

                            _HotelName = Catagory.HotelName;

                        }
                    }
                    return _HotelName;
                }
                else
                {
                   return  _HotelName = "";

                }
            }


        }

    }

    public class TokenViewModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
    }
}
