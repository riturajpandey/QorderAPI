using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
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

        [HttpPost]
        [Route("PlaceOrder")]
        public IHttpActionResult PlaceOrder(List<OrderModel> model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<OrderModel, PatronsOrder>();
                        cfg.CreateMap<OrderModel, PatronsOrdersDetail>();
                    });

                    IMapper mapper = config.CreateMapper();
                    var dataOrder = mapper.Map<List<PatronsOrder>>(model);
                    var dataOrderDetail = mapper.Map<List<PatronsOrdersDetail>>(model);

                    _context.PatronsOrders.AddRange(dataOrder);
                    int Rows = _context.SaveChanges();
                    if (Rows > 0)
                    {
                        var jugad = model.FirstOrDefault();
                        var orders = _context.PatronsOrders.Where(m => m.PatronID == jugad.PatronID & m.DateTimeOfOrder == jugad.DateTimeOfOrder).ToList();
                        for (int i = 0; i >= orders.Count(); i++)
                        {
                            dataOrderDetail[i].PatronsOrdersID = orders[i].PatronsOrdersID;

                        }
                        _context.PatronsOrdersDetails.AddRange(dataOrderDetail);
                        int DetailOrder = _context.SaveChanges();
                        if (DetailOrder > 0)
                        {
                            List<int> OrderIds = new List<int>();

                            foreach (var item in orders)
                            {
                                int orderid;

                                orderid = item.PatronsOrdersID;
                                OrderIds.Add(orderid);
                            }

                            return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = OrderIds });
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

        //TODO:-To Insert the Card Details for the Patron's The Encryption use Patron's Login Password to Encrypt.
        [HttpPost]
        [Route("PatronsCardDetails")]
        public IHttpActionResult PatronsCardDetails(CardModel model)
        {
            try
            {

                SHA256 mySHA256 = SHA256Managed.Create();
                var patronsDetails = _context.Patrons.Where(m => m.PatronsID == model.PatronsID).FirstOrDefault();
                string PatronsPassword = patronsDetails.Gassword;
                //string PatronsPassword = "3sc3RLrpd17";
                byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(PatronsPassword));
              


                byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                PatronsPaymentMethod data = new PatronsPaymentMethod();
                data.PatronID = model.PatronsID;
                data.PaymentType = model.PaymentType;
                data.PaymentCardholderName = model.PaymentCardholderName;
                data.PaymentCardType = model.PaymentCardType;
                data.PaymentCardNumberEncrypted = Encryption.EncryptString(model.PaymentCardNumberEncrypted,key,iv);
                data.PaymentCardCvvCodeEncrypted = Encryption.EncryptString(model.PaymentCardCvvCodeEncrypted, key, iv);
                data.PaymentCardExpiryEncrypted = Encryption.EncryptString(model.PaymentCardExpiryEncrypted, key, iv);

               
                _context.PatronsPaymentMethods.Add(data);
                int Rows = _context.SaveChanges();
                if(Rows>0)
                {
                    return Ok(new ResponseModel { Message = "Request Execution successfully.", Status = "Success" });
                }
                else
                {
                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Failed" });
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
                if (ModelState.IsValid)
                {

                    var Patrnsorder = _context.PatronsOrders.Where(m => m.PatronID == PatonID).ToList();
                    List<OrderHistoryRespones> _ListOrderHistory = new List<OrderHistoryRespones>();
                    foreach (var item in Patrnsorder)
                    {
                        OrderHistoryRespones data = new OrderHistoryRespones();
                        data.DateTimeOfOrder = item.DateTimeOfOrder;
                        var result = _context.Hotels.Where(m => m.HotelID == item.HotelID).FirstOrDefault();
                        data.HotelName = result.HotelName;
                        data.PatronsOrdersID = item.PatronsOrdersID;
                        _ListOrderHistory.Add(data);
                    }
                    for (int i = 0; i<_ListOrderHistory.Count(); i++)
                    {
                        int PatronsOrderId = _ListOrderHistory[i].PatronsOrdersID;
                        var PatronsDetails = _context.PatronsOrdersDetails.Where(m => m.PatronsOrdersID == PatronsOrderId).FirstOrDefault();
                        if(PatronsDetails.ItemNameAtTimeOfBuying == null)
                        { _ListOrderHistory[i].DrinkName = ""; }
                        else { _ListOrderHistory[i].DrinkName = PatronsDetails.ItemNameAtTimeOfBuying; }
                        if(PatronsDetails.QTYOrdered == null)
                        { _ListOrderHistory[i].QTYOrdered = 0; }
                        else{ _ListOrderHistory[i].QTYOrdered = PatronsDetails.QTYOrdered; }
                        if (PatronsDetails.SizeAtTimeOfBuying == null)
                        { _ListOrderHistory[i].Size = ""; }
                        else { _ListOrderHistory[i].Size = PatronsDetails.SizeAtTimeOfBuying; }
                        if(PatronsDetails.PriceAtTimeOfBuying == null)
                        { _ListOrderHistory[i].Price =0;  }
                        else { _ListOrderHistory[i].Price = PatronsDetails.PriceAtTimeOfBuying; }
                       
                    }

                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success",Data= _ListOrderHistory });
                }
                else
                {
                   return  BadRequest("Provided Data is Invalid.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
