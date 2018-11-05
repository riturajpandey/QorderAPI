using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DrinkingBuddy.Models;
using DrinkingBuddy.Encryption;
using DrinkingBuddy.Entities;
using DrinkingBuddy.Filter;
using DrinkingBuddy.Providers;
using DrinkingBuddy.Results;
using DrinkingBuddy.Notification;
using AutoMapper;
using System.Text;
using System.Threading;

namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();
        EncryptionLibrary Encryption = new EncryptionLibrary();
        PushNotification push = new PushNotification();

        string Message;

        #region Order

        [HttpPost]
        [Route("PlaceOrder")]
        public IHttpActionResult PlaceOrder(OrderBindingModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<OrderBindingModel, PatronsOrder>();

                        //  cfg.CreateMap<OrderBindingModel, PatronsOrdersDetail>();
                        cfg.CreateMap<OrderMenu, PatronsOrdersDetail>();

                    });

                    IMapper mapper = config.CreateMapper();
                    var dataOrder = mapper.Map<PatronsOrder>(model);
                    var dataOrderDetail = mapper.Map<List<PatronsOrdersDetail>>(model.OrderMenus);

                    _context.PatronsOrders.Add(dataOrder);
                    int Rows = _context.SaveChanges();
                    if (Rows > 0)
                    {

                        var orders = _context.PatronsOrders.Where(m => m.PatronID == model.PatronID & m.HotelID == model.HotelID).ToList();
                        if (orders.Count() == 0)
                        {
                            return BadRequest("no Hotel found");
                        }
                        PatronsOrder _PatronsOrder = new PatronsOrder();
                        foreach (var item in orders)
                        {
                            if (item.DateTimeOfOrder == model.DateTimeOfOrder)
                            {
                                _PatronsOrder = item;
                            }
                        }

                        for (int i = 0; i < dataOrderDetail.Count(); i++)
                        {
                            dataOrderDetail[i].PatronsOrdersID = _PatronsOrder.PatronsOrdersID;

                        }
                        _context.PatronsOrdersDetails.AddRange(dataOrderDetail);
                        int DetailOrder = _context.SaveChanges();
                        if (DetailOrder > 0)
                        {
                            var devicetoken = push.FindDeviceToken(null, model.PatronID);
                            string Message = "Your Order Have been Placed Successfully.";
                            var status = push.SendNotification(devicetoken, Message);
                            NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                            notificationBindingModel.DateTimeSent = DateTime.Now;
                            notificationBindingModel.PatronID = model.PatronID;
                            notificationBindingModel.NotificationContent = Message;
                            notificationBindingModel.NotificationType = "Order";
                            push.InsertNotification(notificationBindingModel);

                            PlaceOrderResponse response = new PlaceOrderResponse();
                            response.OrderId = _PatronsOrder.PatronsOrdersID;

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = response });
                        }
                        else { return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" }); }

                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
                    }

                }
                else
                {
                    return BadRequest("Parameter's are Invalid");

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("OrderHistory")]
        public IHttpActionResult OrderHistory(int PatonID)
        {
            try
            {
                if (PatonID > 0)
                {

                    var Patrnsorder = _context.PatronsOrders.Where(m => m.PatronID == PatonID & m.OrderCollected == true).ToList();

                    if (Patrnsorder.Count() == 0)
                    {
                        // return BadRequest("No Order History found");
                        return Ok(new ResponseModel { Message = "No Orders found for this parton.", Status = "Success", });
                    }
                    List<OrderHistoryResponse> _ListOrderHistory = new List<OrderHistoryResponse>();


                    foreach (var item in Patrnsorder)
                    {
                        OrderHistoryResponse data = new OrderHistoryResponse();
                        data.DateTimeOfOrder = item.DateTimeOfOrder;
                        var result = _context.Hotels.Where(m => m.HotelID == item.HotelID).FirstOrDefault();
                        if (result == null)
                        {
                            return BadRequest("No Hotel found");
                        }
                        data.HotelName = result.HotelName;
                        data.PatronsOrdersID = item.PatronsOrdersID;
                        if (item.FinalAmountForOrder == null) { data.FinalAmount = 0; }
                        else
                        {
                            data.FinalAmount = item.FinalAmountForOrder;
                        }

                        _ListOrderHistory.Add(data);

                        var orderdetails = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == item.PatronsOrdersID & m.DeliveredByHotel == true).ToList();
                        if (orderdetails.Count() == 0)
                        {

                            // return BadRequest("No Order Details Found");
                            return Ok(new ResponseModel { Message = "No Order Details found for this parton.", Status = "Success", });
                        }
                        List<DrinkHistory> _ListDrink = new List<DrinkHistory>();
                        foreach (var order in orderdetails)
                        {
                            DrinkHistory drinkhistory = new DrinkHistory();
                            var drink = _context.HotelMenus.Where(m => m.HotelMenuID == order.HotelMenuItemID).FirstOrDefault();
                            if (drink == null)
                            {
                                return BadRequest("No Hotel found");
                            }
                            drinkhistory.DrinkName = drink.DrinkName;
                            if (order.QTYOrdered == null) { drinkhistory.QuantityOrdered = 0; }
                            else { drinkhistory.QuantityOrdered = order.QTYOrdered; }
                            if (order.AcceptedPricePerItem == null) { drinkhistory.Price = 0; }
                            else { drinkhistory.Price = order.AcceptedPricePerItem; }
                            drinkhistory.Size = order.SizeAtTimeOfBuying;

                            _ListDrink.Add(drinkhistory);
                        }

                        data.DrinkList = _ListDrink;
                        int? count = 0;
                        foreach (var num in _ListDrink)
                        {
                            count = count + num.QuantityOrdered;

                        }

                        data.DrinkCount = count;
                    }


                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = _ListOrderHistory });
                }
                else
                {
                    return BadRequest("Provided Data is Invalid.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("CurrentOrders")]
        public IHttpActionResult CurrentOrders(int PatronID)
        {
            try
            {
                if (PatronID > 0)
                {
                    var order = _context.PatronsOrders.Where(m => m.PatronID == PatronID & m.BarCompletedOrder != true & m.OrderCollected != true).ToList();
                    if (order.Count() == 0)
                    {
                        //  return BadRequest("No Current Order Exist.");
                        return Ok(new ResponseModel { Message = "No Current Orders found for this parton.", Status = "Success", });
                    }
                    List<PatronsOrdersDetail> orderDetails = new List<PatronsOrdersDetail>();

                    List<CurrentOrderResponse> _listcurrentorderreponse = new List<CurrentOrderResponse>();

                    foreach (var item in order)
                    {
                        var hotel = _context.Hotels.Where(m => m.HotelID == item.HotelID).FirstOrDefault();
                        if (hotel == null)
                        {
                            return BadRequest("No Hotel Found");
                        }
                        CurrentOrderResponse current = new CurrentOrderResponse();
                        current.PatronsOrdersID = item.PatronsOrdersID;
                        current.DateTimeOfOrder = item.DateTimeOfOrder;
                        if (item.FinalAmountForOrder == null) { current.FinalAmount = 0; }
                        else { current.FinalAmount = item.FinalAmountForOrder; }
                        current.HotelName = hotel.HotelName;
                        if (item.LinQ == null) { current.LinQ = 0; }
                        else { current.LinQ = item.LinQ; }
                        if (item.BarAcceptedOrder == true)
                        { current.Status = "Accepted"; }
                        else { current.Status = "Pending"; }
                        if (item.BarStartedOrder == true)
                        { current.Status = "Started"; }
                        if (item.BarCompletedOrder == true)
                        { current.Status = "Completed"; }
                        if (item.OrderCollected == true)
                        { current.Status = "Collected"; }

                        List<CurrentDrink> _listCurrentDrink = new List<CurrentDrink>();

                        orderDetails = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == item.PatronsOrdersID & m.BarCompleted != true & m.DeliveredByHotel != true).ToList();

                        if (orderDetails.Count != 0)
                        {
                            foreach (var data in orderDetails)
                            {
                                CurrentDrink single = new CurrentDrink();

                                var menus = _context.HotelMenus.Where(m => m.HotelMenuID == data.HotelMenuItemID).FirstOrDefault();

                                if (menus == null)
                                {
                                    return BadRequest("No Hotel Found");
                                }

                                single.DrinkName = menus.DrinkName;
                                if (data.QTYOrdered != null)
                                { single.QTYOrdered = data.QTYOrdered; }
                                else { single.QTYOrdered = 0; }
                                if (data.SizeAtTimeOfBuying == null) { single.Size = ""; }
                                else { single.Size = data.SizeAtTimeOfBuying; }
                                if (data.AcceptedPricePerItem != null)
                                { single.Price = data.AcceptedPricePerItem; }
                                else { single.Price = 0; }
                                if (menus.MinutesToServeItem == null) { single.EstMinutes = 0; }
                                else { single.EstMinutes = menus.MinutesToServeItem; }



                                _listCurrentDrink.Add(single);
                            }
                            int? count = 0;
                            int? minutes = 0;

                            foreach (var num in _listCurrentDrink)
                            {
                                count = count + num.QTYOrdered;
                                minutes = minutes + num.EstMinutes;

                            }

                            current.DrinkCount = count;
                            current.EstMinutes = minutes;



                        }
                        else
                        {
                            // _listCurrentDrink = null;
                            current.DrinkList = null;
                            current.DrinkCount = 0;
                            current.EstMinutes = 0;
                        }

                        current.DrinkList = _listCurrentDrink;
                        _listcurrentorderreponse.Add(current);
                    }

                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = _listcurrentorderreponse });
                }
                else
                {
                    return Ok(new ResponseModel { Message = "Not a vaild OrerId.", Status = "Success" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("TrackOrders")]
        public IHttpActionResult TrackOrders(int OrderId)
        {
            try
            {
                if (OrderId != 0)
                {
                    TrackingResponse response = new TrackingResponse();
                    var patronsdetails = _context.PatronsOrders.Where(m => m.PatronsOrdersID == OrderId).FirstOrDefault();
                    if (patronsdetails == null)
                    {
                        return BadRequest("No Orders found");
                    }

                    if (patronsdetails.BarAcceptedOrder == true)
                    { response.Status = "Accepted"; }
                    else { response.Status = "Pending"; }
                    if (patronsdetails.BarStartedOrder == true)
                    { response.Status = "Bar started Order"; }
                    if (patronsdetails.BarCompletedOrder == true)
                    { response.Status = "Bar Completed Order"; }
                    if (patronsdetails.OrderCollected == true)
                    { response.Status = "Order collected"; }

                    if (patronsdetails.BarCompletedOrder == true || patronsdetails.OrderCollected == true)
                    {
                        return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = response.Status });
                    }
                    else
                    {
                        var patronsorderdetail = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == OrderId).ToList();
                        if (patronsorderdetail.Count() == 0)
                        {
                            return BadRequest("No Order Details are found");

                        }
                        int? serveminuts = 0;
                        response.EstMinutes = 0;
                        foreach (var item in patronsorderdetail)
                        {
                            var menus = _context.HotelMenus.Where(m => m.HotelMenuID == item.HotelMenuItemID).FirstOrDefault();
                            if (menus == null)
                            {
                                return BadRequest("No Menu found");
                            }
                            serveminuts = menus.MinutesToServeItem;
                            response.EstMinutes = response.EstMinutes + serveminuts;

                        }

                        return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = response });
                    }
                }
                else
                {
                    return Ok(new ResponseModel { Message = "Something went wrong.", Status = "Success" });

                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        #endregion


        #region GroupOrders

        [HttpPost]
        [Route("GroupOrder")]
        public IHttpActionResult GroupOrder(GroupOrderBindingModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<GroupOrderBindingModel, TrackGroupOrder>();
                        cfg.CreateMap<GroupOrderMenu, TrackGroupOrderDetail>();

                    });

                    IMapper mapper = config.CreateMapper();
                    var dataOrder = mapper.Map<TrackGroupOrder>(model);
                    var dataOrderDetail = mapper.Map<List<TrackGroupOrderDetail>>(model.GroupOrderMenus);

                    _context.TrackGroupOrders.Add(dataOrder);
                    int i = _context.SaveChanges();
                    if (i > 0)
                    {
                        var OrderInitiater = new TrackGroupOrder();

                        if (model.OpenMinutes > 0)
                        {
                            OrderInitiater = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == model.PatronsGroupID & m.OpenMinutes == model.OpenMinutes).FirstOrDefault();
                            if (OrderInitiater == null)
                            {
                                return BadRequest("No Tracking Order found for this group and open minutes.");
                            }

                        }
                        else
                        {

                            var initialpatron = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == model.PatronsGroupID).ToList();

                            if (initialpatron.Count() == 0)
                            {
                                return BadRequest("No Tracking Order found for this Group.");
                            }

                            foreach (var item in initialpatron)
                            {
                                if (item.OpenMinutes > 0)
                                {
                                    OrderInitiater = item;
                                }
                            }
                        }

                        List<TrackGroupOrderDetail> trackGroupOrderDetail = new List<TrackGroupOrderDetail>();
                        foreach (var item in dataOrderDetail)
                        {

                            item.TrackGroupOrderID = OrderInitiater.TrackGroupOrderID;
                            trackGroupOrderDetail.Add(item);

                        }

                        _context.TrackGroupOrderDetails.AddRange(trackGroupOrderDetail);
                        int DetailOrder = _context.SaveChanges();
                        if (DetailOrder > 0)
                        {
                            if (model.OpenMinutes > 0)
                            {
                                var groups = _context.PatronsGroupsMembers.Where(m => m.PatronsGroupID == OrderInitiater.PatronsGroupID & m.DateTimeLeftGroup == null).ToList();

                                if (groups.Count() == 0)
                                {
                                    return BadRequest("No member found for this group.");
                                }
                                List<NotificationMember> listmembers = new List<NotificationMember>();
                                foreach (var item in groups)
                                {
                                    NotificationMember single = new NotificationMember();
                                    single.PatronGroupID = item.PatronsGroupID;
                                    single.PatronID = item.MemberPatronID;
                                    listmembers.Add(single);
                                }
                                var ifmaster = _context.PatronsGroups.Where(m => m.PatronsGroupID == OrderInitiater.PatronsGroupID).FirstOrDefault();
                                NotificationMember singlemember = new NotificationMember();
                                singlemember.PatronGroupID = ifmaster.PatronsGroupID;
                                singlemember.PatronID = (int)ifmaster.MasterPatronID;

                                listmembers.Add(singlemember);
                                List<string> devicetoken = new List<string>();
                                foreach (var item in listmembers)
                                {
                                    var patrons = _context.Patrons.Where(m => m.PatronsID == item.PatronID).FirstOrDefault();
                                    if (patrons == null)
                                    {
                                        return BadRequest("No Patron Found");
                                    }
                                    if (patrons.PatronsID != OrderInitiater.PatronID)
                                    {
                                        string token = patrons.DeviceToken;
                                        devicetoken.Add(token);
                                    }
                                }

                                var starterpatron = _context.Patrons.Where(m => m.PatronsID == OrderInitiater.PatronID).FirstOrDefault();
                                if (starterpatron == null)
                                {
                                    return BadRequest("No Patron Found");
                                }

                                //Messages to be sent to the group members.
                                Message = " Has Initiated an Order,Please Join to Complete the Order.";
                                push.SendNotification(devicetoken, starterpatron.FirstName + " " + starterpatron.LastName + Message);
                                //  NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                                //  notificationBindingModel.DateTimeSent = DateTime.Now;
                                //   notificationBindingModel.PatronID = model.PatronID;
                                //   notificationBindingModel.NotificationContent = Message;
                                //   notificationBindingModel.NotificationType = "Order";
                                //    push.InsertNotification();


                                //Message to be sent to the person initiated the order.
                                Message = " You have Initiated an Order,You group member will be Notified Shortly...";
                                push.SendNotification(devicetoken, Message);
                                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                                notificationBindingModel.DateTimeSent = DateTime.Now;
                                notificationBindingModel.PatronID = starterpatron.PatronsID;
                                notificationBindingModel.NotificationContent = Message;
                                notificationBindingModel.NotificationType = "Order";
                                push.InsertNotification(notificationBindingModel);

                            }
                            else
                            {

                                var memberpatron = _context.Patrons.Where(m => m.PatronsID == model.PatronID).FirstOrDefault();
                                if (memberpatron == null)
                                {
                                    return BadRequest("No Patrons Found");
                                }

                                // var Initiaterdetails = _context.PatronsGroups.Where(m => m.PatronsGroupID == OrderInitiater.PatronsGroupID).FirstOrDefault();

                                var Initiaterpatron = _context.Patrons.Where(m => m.PatronsID == OrderInitiater.PatronID).FirstOrDefault();

                                if (Initiaterpatron == null)
                                {
                                    return BadRequest("No Patrons Found");

                                }
                                List<string> devicetoken = new List<string>();
                                string token = Initiaterpatron.DeviceToken;
                                devicetoken.Add(token);
                                //Message to be sent to the Order Initiater.
                                push.SendNotification(devicetoken, memberpatron.FirstName + " " + memberpatron.LastName + " Has joined your group Order.");

                                List<string> tokens = new List<string>();
                                string tkn = memberpatron.DeviceToken;
                                tokens.Add(tkn);
                                Message = "Your Order have been Merge with group Order.";
                                //Message to be sent to the individual member.
                                push.SendNotification(tokens, Message);
                                NotificationBindingModel notificationBindingModel = new NotificationBindingModel();
                                notificationBindingModel.DateTimeSent = DateTime.Now;
                                notificationBindingModel.PatronID = memberpatron.PatronsID;
                                notificationBindingModel.NotificationContent = Message;
                                notificationBindingModel.NotificationType = "Order";
                                push.InsertNotification(notificationBindingModel);
                            }

                            return Ok(new ResponseModel { Message = "Order Added Successfully.", Status = "Success" });

                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Order's Details Addition Failed.", Status = "Success" });
                        }
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Order Addition Failed.", Status = "Success" });
                    }
                }
                else
                {
                    return BadRequest("Models Data Invalid.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet]
        [Route("FinalOrderOfGroup")]
        public IHttpActionResult FinalOrderOfGroup(int PatronID, int PatronsGroupID, int OpenMinutes)
        {
            try
            {
                if (PatronID != 0 & PatronsGroupID != 0 & OpenMinutes != 0)
                {
                    //var PatronsOrders = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == PatronsGroupID & m.OpenMinutes == OpenMinutes).FirstOrDefault();
                    //var PatronsOrderDetais = _context.TrackGroupOrderDetails.Where(m => m.TrackGroupOrderID == PatronsOrders.TrackGroupOrderID).ToList();

                    //var config = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<TrackGroupOrder, OrderBindingModel>();
                    //    cfg.CreateMap<TrackGroupOrderDetail, OrderMenu>();

                    //});

                    //IMapper mapper = config.CreateMapper();
                    //var dataOrder = mapper.Map<OrderBindingModel>(PatronsOrders);
                    //var dataOrderDetail = mapper.Map<List<OrderMenu>>(PatronsOrderDetais);


                    //OrderBindingModel data = new OrderBindingModel();
                    //data = dataOrder;
                    //data.OrderMenus = dataOrderDetail;

                    // code to wait for the time set by patron.
                    var autoevent = new AutoResetEvent(true);
                    var timer =new Timer(
                            e => InsertInOrders(PatronID, PatronsGroupID, OpenMinutes),
                     autoevent,
                     TimeSpan.FromMinutes(OpenMinutes),
                     TimeSpan.Zero);

                    return Ok(new ResponseModel { Message = "The group order has initiated.", Status = "Success" });
                }
                else
                {
                    return BadRequest("The Passed parameter's are not valid.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public object InsertInOrders(int PatronID, int PatronsGroupID, int OpenMinutes)
        {
            try
            {
                if (PatronID > 0 & PatronsGroupID > 0 & OpenMinutes > 0)
                {

                    var trackorders = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == PatronsGroupID & m.OpenMinutes == OpenMinutes & m.PatronID == PatronID).FirstOrDefault();

                    var trackorderdetails = _context.TrackGroupOrderDetails.Where(m => m.TrackGroupOrderID == trackorders.TrackGroupOrderID).ToList();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<TrackGroupOrder, PatronsOrder>();

                        //  cfg.CreateMap<OrderBindingModel, PatronsOrdersDetail>();
                        cfg.CreateMap<TrackGroupOrderDetail, PatronsOrdersDetail>();

                    });

                    IMapper mapper = config.CreateMapper();
                    var dataOrder = mapper.Map<PatronsOrder>(trackorders);
                    var dataOrderDetail = mapper.Map<List<PatronsOrdersDetail>>(trackorderdetails);

                    _context.PatronsOrders.Add(dataOrder);
                    int Rows = _context.SaveChanges();
                    if (Rows > 0)
                    {

                        var orders = _context.PatronsOrders.Where(m => m.DateTimeOfOrder == trackorders.DateTimeOfOrder).FirstOrDefault();

                        for (int i = 0; i < dataOrderDetail.Count(); i++)
                        {
                            dataOrderDetail[i].PatronsOrdersID = orders.PatronsOrdersID;

                        }
                        _context.PatronsOrdersDetails.AddRange(dataOrderDetail);
                        int DetailOrder = _context.SaveChanges();
                        if (DetailOrder > 0)
                        {
                            PlaceOrderResponse response = new PlaceOrderResponse();
                            response.OrderId = orders.PatronsOrdersID;
                            return response;
                        }
                        else { return false; }

                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("CheckInitial")]
        public IHttpActionResult CheckInitial(int PatronID, int GroupId, int HotelID)
        {
            try
            {
                if (PatronID > 0 & GroupId > 0 & HotelID > 0)
                {

                    DateTime todaysdate = System.DateTime.Today.Date;
                    string today = todaysdate.ToString("yyyy-MM-dd");
                    CheckInitialResponse response = new CheckInitialResponse();


                    var trackorder = _context.TrackGroupOrders.Where(m => m.PatronID == PatronID & m.PatronsGroupID == GroupId & m.HotelID == HotelID).ToList();

                    if (trackorder.Count() == 0)
                    {
                        response.IsInitiated = false;
                        return Ok(new ResponseModel { Message = "This Group has not initiated any Order.", Status = "Success", Data = response });

                    }
                    List<string> listsqldate = new List<string>();

                    foreach (var item in trackorder)
                    {
                        DateTime junk = DateTime.Parse(item.DateTimeOfOrder.ToString());
                        string sqldate = junk.ToString("yyyy-MM-dd");

                        listsqldate.Add(sqldate);
                    }

                    foreach (var item in listsqldate)
                    {

                        if (item == today)
                        {

                            response.IsInitiated = true;
                            return Ok(new ResponseModel { Message = "Order initiated.", Status = "Success", Data = response });
                        }
                        else
                        {
                            response.IsInitiated = false;
                            return Ok(new ResponseModel { Message = "This Group has not initiated any Order.", Status = "Success", Data = response });

                        }


                    }

                }

                return BadRequest("Provided parameters are not valid.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }


        #endregion

        #region Card Details

        //TODO:-To Insert the Card Details for the Patron's The Encryption use Patron's Login Password to Encrypt.
        [HttpPost]
        [Route("PatronsCardDetails")]
        public IHttpActionResult PatronsCardDetails(CardModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    SHA256 mySHA256 = SHA256Managed.Create();
                    var patronsDetails = _context.Patrons.Where(m => m.PatronsID == model.PatronsID).FirstOrDefault();
                    if (patronsDetails == null)
                    {
                        return BadRequest("No Patron Found");
                    }
                    string PatronsPassword = patronsDetails.Gassword;
                    if (patronsDetails.Gassword != null)
                    {

                        //string PatronsPassword = "3sc3RLrpd17";
                        byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(PatronsPassword));



                        byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                        PatronsPaymentMethod data = new PatronsPaymentMethod();
                        data.PatronID = model.PatronsID;
                        data.PaymentType = model.PaymentType;
                        data.PaymentCardholderName = model.PaymentCardholderName;
                        data.PaymentCardType = model.PaymentCardType;
                        data.PaymentCardNumberEncrypted = Encryption.EncryptString(model.PaymentCardNumberEncrypted, key, iv);
                        data.PaymentCardCvvCodeEncrypted = Encryption.EncryptString(model.PaymentCardCvvCodeEncrypted, key, iv);
                        data.PaymentCardExpiryEncrypted = Encryption.EncryptString(model.PaymentCardExpiryEncrypted, key, iv);


                        _context.PatronsPaymentMethods.Add(data);
                        int Rows = _context.SaveChanges();
                        if (Rows > 0)
                        {
                            return Ok(new ResponseModel { Message = "Request Execution successfully.", Status = "Success" });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
                        }
                    }
                    else
                    {
                        return BadRequest("Patrond Deos not exist");
                    }
                }
                else
                {
                    return BadRequest("Passed Paramter's Invalid");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }

        [HttpPost]
        [Route("UpdatePatronsCardDetails")]
        public IHttpActionResult UpdatePatronsCardDetails(CardModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cardDetails = _context.PatronsPaymentMethods.Where(m => m.PatronID == model.PatronsID).FirstOrDefault();
                    if (cardDetails != null)
                    {

                        SHA256 mySHA256 = SHA256Managed.Create();
                        var patronsDetails = _context.Patrons.Where(m => m.PatronsID == model.PatronsID).FirstOrDefault();
                        if (patronsDetails == null)
                        {
                            return BadRequest("No Patron Found with this Details");
                        }
                        string PatronsPassword = patronsDetails.Gassword;
                        if (patronsDetails.Gassword != null)
                        {

                            //string PatronsPassword = "3sc3RLrpd17";
                            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(PatronsPassword));

                            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                            cardDetails.PatronID = model.PatronsID;
                            cardDetails.PaymentType = model.PaymentType;
                            cardDetails.PaymentCardholderName = model.PaymentCardholderName;
                            cardDetails.PaymentCardType = model.PaymentCardType;
                            cardDetails.PaymentCardNumberEncrypted = Encryption.EncryptString(model.PaymentCardNumberEncrypted, key, iv);
                            cardDetails.PaymentCardCvvCodeEncrypted = Encryption.EncryptString(model.PaymentCardCvvCodeEncrypted, key, iv);
                            cardDetails.PaymentCardExpiryEncrypted = Encryption.EncryptString(model.PaymentCardExpiryEncrypted, key, iv);

                            _context.Entry(cardDetails).State = EntityState.Modified;
                            int Rows = _context.SaveChanges();
                            if (Rows > 0)
                            {
                                return Ok(new ResponseModel { Message = "The Details Updated  successfully.", Status = "Success" });
                            }
                            else
                            {
                                return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
                            }
                        }
                        else
                        {
                            return BadRequest("Patron Deos not exist.");
                        }
                    }
                    else
                    {
                        return BadRequest("The patron Card Details are not available.");
                    }
                }
                else
                {
                    return BadRequest("Passed Paramter's Invalid");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        //TODO:-Method to check the Encryption.(Only for testing purpose.)
        [HttpGet]
        [Route("ShowCard")]
        public IHttpActionResult ShowCard(int PatronsID)
        {
            var CardDetails = _context.PatronsPaymentMethods.Where(m => m.PatronID == PatronsID).FirstOrDefault();

            SHA256 mySHA256 = SHA256Managed.Create();
            var patronsDetails = _context.Patrons.Where(m => m.PatronsID == PatronsID).FirstOrDefault();
            string PatronsPassword = patronsDetails.Gassword;
            //string PatronsPassword = "3sc3RLrpd17";
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(PatronsPassword));



            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            PatronsPaymentMethod data = new PatronsPaymentMethod();
            data.PatronsPaymentMethodID = CardDetails.PatronsPaymentMethodID;
            data.PatronID = CardDetails.PatronID;
            data.PaymentType = CardDetails.PaymentType;
            data.PaymentCardholderName = CardDetails.PaymentCardholderName;
            data.PaymentCardType = CardDetails.PaymentCardType;
            data.PaymentCardNumberEncrypted = Encryption.DecryptString(CardDetails.PaymentCardNumberEncrypted, key, iv);
            data.PaymentCardCvvCodeEncrypted = Encryption.DecryptString(CardDetails.PaymentCardCvvCodeEncrypted, key, iv);
            data.PaymentCardExpiryEncrypted = Encryption.DecryptString(CardDetails.PaymentCardExpiryEncrypted, key, iv);

            return Ok(new ResponseModel { Message = "Request Execution successfully.", Status = "Success", Data = data });
        }

        #endregion
    }

}
