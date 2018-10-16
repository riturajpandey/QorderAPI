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
using DrinkingBuddy.Entities;
using DrinkingBuddy.Filter;
using DrinkingBuddy.Providers;
using DrinkingBuddy.Results;
using AutoMapper;
using System.Data.Entity;


namespace DrinkingBuddy.Controllers
{
    [APIAuthorizeAttribute]
    [RoutePrefix("api/DrankBank")]
    public class DrankBankController : ApiController
    {
        DrinkingBuddyEntities _context = new DrinkingBuddyEntities();

        [HttpGet]
        [Route("StartGroup")]
        public IHttpActionResult GetCurrentBalance(int PatronID)
        {
            try
            {
                var balance = _context.PatronsWallets.Where(m => m.PatronID == PatronID).FirstOrDefault();
                if (balance != null)
                {
                    BalanceResponse _response = new BalanceResponse();
                    _response.PatronWalletID = balance.PatronsWalletID;
                    _response.Balance = balance.Balance;

                    return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = _response });


                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetTransections")]
        public IHttpActionResult GetTransections(int PatronID, int PatronWallteID)
        {
            try
            {
                if (PatronID > 0 & PatronWallteID > 0)
                {
                    List<TransectionsResponse> _listransectionsResponses = new List<TransectionsResponse>();
                    var patroncredit = _context.PatronCreditTransections.Where(m => m.PatronID == PatronID & m.PatronsWalletID == PatronWallteID).ToList();

                    List<TransectionsResponse> listtransectionsResponses = new List<TransectionsResponse>();

                    if (patroncredit.Count > 0)
                    {
                        foreach (var item in patroncredit)
                        {
                            TransectionsResponse single = new TransectionsResponse();
                            single.TransectionType = "Deposite";
                            single.TransectionDate = item.CreditDatetime;
                            single.Amount = item.Credit_Amount;

                            listtransectionsResponses.Add(single);
                        }

                    }


                    var patrontransfer = _context.PatronTransferTransections.Where(m => m.PatronID_Sender == PatronID & m.PatronWalletID_Sender == PatronWallteID).ToList();

                    if (patrontransfer.Count > 0)
                    {
                        foreach (var item in patrontransfer)
                        {
                            TransectionsResponse single = new TransectionsResponse();
                            single.TransectionType = "Deposite";
                            single.TransectionDate = item.TransferDateTime;
                            single.Amount = item.Amount_Transfer;

                            listtransectionsResponses.Add(single);
                        }

                    }

                    if (listtransectionsResponses.Count()>0)
                    {
                        return Ok(new ResponseModel { Message = "Request Executed successfully.", Status = "Success", Data = listtransectionsResponses });
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "No Transection found for this Patron.", Status = "Success" });

                    }

                }
                else
                {
                    return BadRequest("The passed Parameter");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}

