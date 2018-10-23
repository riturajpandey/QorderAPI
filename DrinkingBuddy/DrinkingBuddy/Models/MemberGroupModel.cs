using System;
using System.Collections.Generic;
using System.Linq;
using DrinkingBuddy.Entities;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class StartGroupBinding
    {
        public int HotelID { set; get; }
        public int MasterPatronID { get; set; }
        public Nullable<System.DateTime> GroupStartedDateTime { get; set; }

    }

    public class StartGroupResponse
    {
        public int PatronsGroupID { get; set; }
        public string HotelName { get; set; }
        public DateTime? GroupStartDateTime { get; set; }
        public string GroupMasterPatron { get; set; }

    }

    public class AcceptInviteModel
    {
        public int PatronsGroupID { get; set; }
        public int MemberPatronID { get; set; }
        public Nullable<System.DateTime> DateTimeJoinedGroup { get; set; }
    }

    public class LeaveGroupModel
    {
        public int PatronsGroupID { get; set; }
        public int MemberPatronID { get; set; }
        public Nullable<System.DateTime> DateTimeLeftGroup { get; set; }

    }

    public class DeviceTokenResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
    }

    public class GroupByPatronResponse
    {
        private string _MasterPatronnName;
        public int GroupID { get; set; }
        public bool IsMaster { get; set; }
        public int? MasterPatronID { get; set; }
        public string MasterPetronName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Patrons = _context.Patrons.Where(m => m.PatronsID == this.MasterPatronID).FirstOrDefault();
                    if (Patrons != null)
                    {
                        return _MasterPatronnName = Patrons.FirstName + " " + Patrons.LastName;
                    }
                    else
                    {
                        return _MasterPatronnName = "";
                    }
                }
            }
        }
        public List<MemberByGroupResponse> MemeberPatrons { get; set; }
    }

    public class MemberByGroupResponse
    {
        private string _PatronName;
        public int? PatronID { get; set; }
        public string PatronName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.Patrons.Where(m => m.PatronsID == this.PatronID).FirstOrDefault();
                    if (Catagory != null)
                    {
                        _PatronName = Catagory.FirstName + " " + Catagory.LastName;
                    }
                    else
                    {
                        _PatronName = "";
                    }
                }

                return _PatronName;


            }


        }

    }

    public class PatronByHotelResponse
    {
        private string _HotelName;
        private string _PatronName;
        public int? PatronID { get; set; }
        public string PatronName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.Patrons.Where(m => m.PatronsID == this.PatronID).FirstOrDefault();
                    if (Catagory != null)
                    {
                        _PatronName = Catagory.FirstName + " " + Catagory.LastName;
                    }
                    else
                    {
                        _PatronName = "";
                    }
                }

                return _PatronName;


            }


        }
        public int HotelID { get; set; }
        public string HotelName
        {
            get
            {

                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.Hotels.Where(m => m.HotelID == this.HotelID).FirstOrDefault();
                    if (Catagory != null)
                    {
                        _HotelName = Catagory.HotelName;
                    }
                    else
                    {
                        _HotelName = "";
                    }
                }

                return _HotelName;


            }

        }
        public string InvitationStatus { get; set; }
       

    }

    public class InvitationResponse
    {
        private string _MasterPatronName;
        private string _HotelName;
        public int PatronsGroupID { get; set; }
        public int? MasterPatornID { get; set; }
        public string MasterPatronName
        {
            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.Patrons.Where(m => m.PatronsID == this.MasterPatornID).FirstOrDefault();
                    if (Catagory != null)
                    {
                        _MasterPatronName = Catagory.FirstName + " " + Catagory.LastName;
                    }
                    else
                    {
                        _MasterPatronName = "";
                    }
                }

                return _MasterPatronName;

            }

        }
        public int? HotelID { get; set; }
        public string HotelName
        {

            get
            {
                using (DrinkingBuddyEntities _context = new DrinkingBuddyEntities())
                {
                    var Catagory = _context.Hotels.Where(m => m.HotelID == this.HotelID).FirstOrDefault();
                    if (Catagory != null)
                    {
                        _HotelName = Catagory.HotelName;
                    }
                    else
                    {
                        _HotelName = "";
                    }
                }

                return _HotelName;

            }
        }
    }

    public class SendInviteBinding
    {
        public int HotelID { get; set; }
        public int MasterPatronID { get; set; }
        public int PatronsGroupID { get; set; }
        public int PatronID { get; set; }
    }
}
