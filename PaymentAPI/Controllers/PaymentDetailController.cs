using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDetailController : ControllerBase
    {
        private readonly PaymentDetailContext _context;

        public PaymentDetailController(PaymentDetailContext context)
        {
            _context = context;
        }

        // GET: api/PaymentDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetail>>> GetPaymentDetails() // with this method we can retrieve all data we have in the db table Payment details
        {
            return await _context.PaymentDetails.ToListAsync();// in EF list of records from a table can be accessed with coresp db set property(here PaymentDetails)
        }                                                      //  and finally convert it into a list by using .ToListAsync()   

        // GET: api/PaymentDetail/5
        [HttpGet("{id}")] //this function is made possiible with the function CreatedAtAction (that makes an url) at the post method
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetail(int id)// with this method we will return a speccific record based on given id
        {
            var paymentDetail = await _context.PaymentDetails.FindAsync(id);//we will call this find function from the dbset

            if (paymentDetail == null)
            {
                return NotFound();//if its not there notfound error will be returned
            }

            return paymentDetail; //if there is an object it will be returned here
        }

        // PUT: api/PaymentDetail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")] //this method updates a given record
        public async Task<IActionResult> PutPaymentDetail(int id, PaymentDetail paymentDetail) 
        {
            if (id != paymentDetail.PaymentDetailId) // first of all compares the id inside first parameter with id inside the object(second parameter)
            {
                return BadRequest();
            }

            _context.Entry(paymentDetail).State = EntityState.Modified;//if match, we need to set the state of the model property(PaymentDetail) as modified

            try
            {
                await _context.SaveChangesAsync(); //finally call the save changes
            }
            catch (DbUpdateConcurrencyException) //there is a possibility for this exception, handled inside the following catch block
            {
                if (!PaymentDetailExists(id)) //supposing this app is used by a wide range o users, there is a possibility of deleting or modifying same record by multiple users
                {                             //that is called Concurrency, and can be handled in different ways                       
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); //if the update operation is a success it will return the response no content
        }

        // POST: api/PaymentDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaymentDetail>> PostPaymentDetail(PaymentDetail paymentDetail) //with this POST method we can insert a new record
        {                                                               //through this parameter(of type PaymentDetail Model) we can pass values for the new object to be added
            _context.PaymentDetails.Add(paymentDetail); //with the help of EF, to insert record we just need to use .Add method(from the corespondent db set property we defined in PaymentDetailContext)
            await _context.SaveChangesAsync(); //finally we call this method , and thereby the corespondent sql queries will be executed in background so that new record is inserted in db table

            return CreatedAtAction("GetPaymentDetail", new { id = paymentDetail.PaymentDetailId }, paymentDetail); // this return statement returns the newly inserted object
        }           //this function will create an url with first 2 params for the newly created object

        // DELETE: api/PaymentDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentDetail(int id) //this id that we have passed while making the request
        {
            var paymentDetail = await _context.PaymentDetails.FindAsync(id); //it will find the correspondent id and save it in this variable
            
            //if it's null
            if (paymentDetail == null)
            {
                return NotFound();
            }
            //if it's not null
            _context.PaymentDetails.Remove(paymentDetail);
            
            //finally calling the save changes function
            await _context.SaveChangesAsync();

            return NoContent();//if the delete operation is a success it will return the response no content
        }

        private bool PaymentDetailExists(int id)
        {
            return _context.PaymentDetails.Any(e => e.PaymentDetailId == id);
        }
    }
}
