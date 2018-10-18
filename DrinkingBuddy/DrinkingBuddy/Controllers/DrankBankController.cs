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
        [Route("GetCurrentBalance")]
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
                    return Ok(new ResponseModel { Message = "No Record found for this Patron.", Status = "Success" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetTransections")]
        public IHttpActionResult GetTransections(int PatronID)
        {
            try
            {
                if (PatronID > 0)
                {
                    List<TransectionsResponse> _listransectionsResponses = new List<TransectionsResponse>();
                    var patroncredit = _context.PatronCreditTransections.Where(m => m.PatronID == PatronID).ToList();

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


                    var patrontransfer = _context.PatronTransferTransections.Where(m => m.PatronID_Sender == PatronID).ToList();

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

                    if (listtransectionsResponses.Count() > 0)
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

        [HttpPost]
        [Route("TransferByEmail")]
        public IHttpActionResult TransferByEmail(TransferModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var reciverdetails = _context.Patrons.Where(m => m.EmailAddress == model.Email).FirstOrDefault();
                    if (reciverdetails != null)
                    {
                        //Inserting the record to Transfer table on the name of sender.
                        PatronTransferTransection transfer = new PatronTransferTransection();
                        transfer.PatronID_Sender = model.SenderPatronID;
                        transfer.PatronID_Reciver = reciverdetails.PatronsID;
                        transfer.Amount_Transfer = model.Amount;
                        transfer.TransferDateTime = DateTime.Now;

                        _context.PatronTransferTransections.Add(transfer);
                        int i = _context.SaveChanges();
                        if (i > 0)
                        {
                            var currentwallet = _context.PatronsWallets.Where(m => m.PatronID == reciverdetails.PatronsID).FirstOrDefault();
                            if (currentwallet != null)
                            {
                                //one copy to be inserted in Credit table on the name of the reciver.
                                PatronCreditTransection patronCreditTransection = new PatronCreditTransection();
                                patronCreditTransection.PatronID = reciverdetails.PatronsID;
                                patronCreditTransection.Credit_Amount = model.Amount;
                                patronCreditTransection.CreditDatetime = transfer.TransferDateTime;
                                patronCreditTransection.IsCardDeposite = false;
                                patronCreditTransection.IsPatronTransfer = true;
                                patronCreditTransection.Previouse_Balance = currentwallet.Balance;
                                patronCreditTransection.Updated_Balance = currentwallet.Balance + model.Amount;

                                _context.PatronCreditTransections.Add(patronCreditTransection);
                                int row = _context.SaveChanges();
                                if (row > 0)
                                {
                                    //Updating reciver balance after transfer.
                                    currentwallet.Balance = currentwallet.Balance + model.Amount;
                                    currentwallet.LastTransDateTime = transfer.TransferDateTime;

                                    _context.Entry(currentwallet).State = EntityState.Modified;
                                    int j = _context.SaveChanges();
                                    if (j > 0)
                                    {
                                        //Updating Sender balance after transfer.
                                        var senderwallet = _context.PatronsWallets.Where(m => m.PatronID == model.SenderPatronID).FirstOrDefault();
                                        senderwallet.Balance = senderwallet.Balance - model.Amount;
                                        senderwallet.LastTransDateTime = transfer.TransferDateTime;
                                        _context.Entry(senderwallet).State = EntityState.Modified;
                                        int result = _context.SaveChanges();
                                        if (result > 0)
                                        {
                                            TransferResponsemodel response = new TransferResponsemodel();
                                            response.UpdatedBalace = senderwallet.Balance;

                                            return Ok(new ResponseModel { Message = "Transection Done Successfully.", Status = "Success", Data = response });
                                        }

                                    }

                                }

                            }


                        }



                    }

                    return Ok(new ResponseModel { Message = "No Patron was found with this detail.", Status = "Success" });
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
        [Route("FillWallet")]
        public IHttpActionResult FillWallet(int PatronID, decimal Amount)
        {
            try
            {
                if (PatronID > 0 & Amount > 0)
                {

                    var data = _context.PatronsWallets.Where(m => m.PatronID == PatronID).FirstOrDefault();
                    if (data == null)
                    {
                        return BadRequest("The patron does not have any wallet yet");

                    }
                    data.Balance = data.Balance + Amount;
                    data.LastTransDateTime = DateTime.Now;

                    _context.Entry(data).State = EntityState.Modified;
                    int row = _context.SaveChanges();
                    if (row > 0)
                    {
                        PatronCreditTransection credit = new PatronCreditTransection();
                        credit.PatronID = PatronID;
                        credit.Credit_Amount = Amount;
                        credit.IsCardDeposite = true;
                        credit.IsPatronTransfer = false;
                        credit.Previouse_Balance = data.Balance;
                        credit.Updated_Balance = data.Balance + Amount;

                        _context.PatronCreditTransections.Add(credit);
                        int i = _context.SaveChanges();
                        if (i > 0)
                        {
                            var result = _context.PatronsWallets.Where(m => m.PatronID == PatronID).FirstOrDefault();
                            TransferResponsemodel response = new TransferResponsemodel();
                            response.UpdatedBalace = result.Balance;

                            return Ok(new ResponseModel { Message = "Request Executed Successfully.", Status = "Success", Data = response });
                        }
                        else
                        {
                            return Ok(new ResponseModel { Message = "No Patron was found with this detail.", Status = "Success" });

                        }
                    }
                    else
                    {
                        return Ok(new ResponseModel { Message = "No Patron was found with this detail.", Status = "Success" });

                    }
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



    }
}

