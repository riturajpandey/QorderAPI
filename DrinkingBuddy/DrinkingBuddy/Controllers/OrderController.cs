﻿using System;
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
        public IHttpActionResult PlaceOrder(List<OrderModel> model, int PatronID)
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
                        { return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success" }); }
                        else { return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" }); }

                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
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

        [HttpPost]
        [Route("PatronsCardDetails")]
        public IHttpActionResult PatronsCardDetails(CardModel model)
        {
            try
            {

                SHA256 mySHA256 = SHA256Managed.Create();
                string password = "3sc3RLrpd17";
                byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
              


                byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                PatronsPaymentMethod data = new PatronsPaymentMethod();
                data.PatronID = model.PatronID;
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
                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
                }
                else
                {
                    return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }

        //For Testing Purpose Only
        [HttpGet]
        [Route("RetrivePayment")]
        public IHttpActionResult RetrivePayment()
        {
            var result = _context.PatronsPaymentMethods.Where(m=>m.PatronID==1).FirstOrDefault();



            SHA256 mySHA256 = SHA256Managed.Create();
            string password = "3sc3RLrpd17";
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
           


            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };


            PatronsPaymentMethod data = new PatronsPaymentMethod();

            data.PatronID = result.PatronID;
            data.PaymentCardNumberEncrypted = Encryption.DecryptString(result.PaymentCardNumberEncrypted, key, iv);
            data.PaymentCardCvvCodeEncrypted = Encryption.DecryptString(result.PaymentCardCvvCodeEncrypted, key, iv);
            data.PaymentCardExpiryEncrypted = Encryption.DecryptString(result.PaymentCardExpiryEncrypted, key, iv);
            data.PaymentCardholderName = result.PaymentCardholderName;
            data.PaymentCardType = result.PaymentCardType;
            data.PaymentType = result.PaymentType;

            return Ok(new ResponseModel { Message = "Request Execution Failed.", Status = "Success",Data=data });

        }
    }
}