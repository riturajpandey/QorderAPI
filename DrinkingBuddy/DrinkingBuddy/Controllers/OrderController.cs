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
using AutoMapper;
using System.Text;

namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();
        EncryptionLibrary Encryption = new EncryptionLibrary();

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

                    var Patrnsorder = _context.PatronsOrders.Where(m => m.PatronID == PatonID).ToList();
                    List<OrderHistoryResponse> _ListOrderHistory = new List<OrderHistoryResponse>();
                    foreach (var item in Patrnsorder)
                    {
                        OrderHistoryResponse data = new OrderHistoryResponse();
                        data.DateTimeOfOrder = item.DateTimeOfOrder;
                        var result = _context.Hotels.Where(m => m.HotelID == item.HotelID).FirstOrDefault();
                        data.HotelName = result.HotelName;
                        data.PatronsOrdersID = item.PatronsOrdersID;
                        _ListOrderHistory.Add(data);
                    }
                    for (int i = 0; i < _ListOrderHistory.Count(); i++)
                    {
                        int PatronsOrderId = _ListOrderHistory[i].PatronsOrdersID;
                        var PatronsDetails = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == PatronsOrderId).FirstOrDefault();
                        if (PatronsDetails != null)
                        {
                            if (PatronsDetails.ItemNameAtTimeOfBuying == null)
                            { _ListOrderHistory[i].DrinkName = ""; }
                            else { _ListOrderHistory[i].DrinkName = PatronsDetails.ItemNameAtTimeOfBuying; }
                            if (PatronsDetails.QTYOrdered == null)
                            { _ListOrderHistory[i].QTYOrdered = 0; }
                            else { _ListOrderHistory[i].QTYOrdered = PatronsDetails.QTYOrdered; }
                            if (PatronsDetails.SizeAtTimeOfBuying == null)
                            { _ListOrderHistory[i].Size = ""; }
                            else { _ListOrderHistory[i].Size = PatronsDetails.SizeAtTimeOfBuying; }
                            if (PatronsDetails.PriceAtTimeOfBuying == null)
                            { _ListOrderHistory[i].Price = 0; }
                            else { _ListOrderHistory[i].Price = PatronsDetails.PriceAtTimeOfBuying; }
                        }
                        else
                        {
                            _ListOrderHistory[i].DrinkName = "";
                            _ListOrderHistory[i].QTYOrdered = 0;
                            _ListOrderHistory[i].Size = "";
                            _ListOrderHistory[i].Price = 0;
                        }
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
        [Route("TrackOrders")]
        public IHttpActionResult TrackOrders(int OrderId)
        {
            try
            {
                if (OrderId != 0)
                {
                    TrackingResponse response = new TrackingResponse();
                    var patronsdetails = _context.PatronsOrders.Where(m => m.PatronsOrdersID == OrderId).FirstOrDefault();
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
                        int? serveminuts = 0;
                        response.EstMinutes = 0;
                        foreach (var item in patronsorderdetail)
                        {
                            var menus = _context.HotelMenus.Where(m => m.HotelMenuID == item.HotelMenuItemID).FirstOrDefault();

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

        [HttpGet]
        [Route("CurrentOrders")]
        public IHttpActionResult CurrentOrders(int OrderId)
        {
            try
            {
                if (OrderId > 0)
                {
                    var orderDetails = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == OrderId).ToList();
                    var order = _context.PatronsOrders.Where(m => m.PatronsOrdersID == OrderId).FirstOrDefault();
                    if (orderDetails == null & order == null)
                    {
                        return BadRequest();
                    }
                    List<OrderHistoryResponse> currentorderreponse = new List<OrderHistoryResponse>();

                    foreach (var item in orderDetails)
                    {
                        OrderHistoryResponse single = new OrderHistoryResponse();

                        var hotel = _context.Hotels.Where(m => m.HotelID == order.HotelID).FirstOrDefault();

                        single.DrinkName = item.ItemNameAtTimeOfBuying;
                        if (item.QTYOrdered != null)
                        { single.QTYOrdered = item.QTYOrdered; }
                        else { single.QTYOrdered = 0; }
                        single.Size = item.SizeAtTimeOfBuying;
                        if (item.PriceAtTimeOfBuying != null)
                        { single.Price = item.PriceAtTimeOfBuying; }
                        else { single.Price = 0; }
                        // single.Price = item.PriceAtTimeOfBuying;
                        single.PatronsOrdersID = OrderId;
                        single.HotelName = hotel.HotelName;
                        single.DateTimeOfOrder = order.DateTimeOfOrder;

                        currentorderreponse.Add(single);
                    }

                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = currentorderreponse });
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
                    var dataOrderDetail = mapper.Map<List<TrackGroupOrderDetail>>(model.OrderMenus);

                    _context.TrackGroupOrders.Add(dataOrder);
                    int i = _context.SaveChanges();
                    if (i > 0)
                    {
                        var trackorders = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == model.PatronsGroupID & m.OpenMinutes == model.OpenMinutes).FirstOrDefault();
                        List<TrackGroupOrderDetail> trackGroupOrderDetail = new List<TrackGroupOrderDetail>();
                        foreach (var item in dataOrderDetail)
                        {

                            item.TrackGroupOrderID = trackorders.TrackGroupOrderID;
                            trackGroupOrderDetail.Add(item);

                        }

                        _context.TrackGroupOrderDetails.AddRange(trackGroupOrderDetail);
                        int DetailOrder = _context.SaveChanges();
                        if (DetailOrder > 0)
                        {
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
                    var timer = new System.Threading.Timer(
                            e => InsertInOrders(PatronID, PatronsGroupID, OpenMinutes),
                      null,
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

                    var trackorders = _context.TrackGroupOrders.Where(m => m.PatronsGroupID == PatronsGroupID & m.OpenMinutes == OpenMinutes).FirstOrDefault();

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
