using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenGameList.ViewModels;
using Newtonsoft.Json;
using OpenGameList.Data;
using OpenGameList.Data.Items;
using AutoMapper;

namespace OpenGameList.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        #region Private Fields
        private ApplicationDbContext Context;
        #endregion Private Fields

        #region Constructor
        public ItemsController(ApplicationDbContext context)
        {
            // Dependency Injetion
            Context = context;
        }
        #endregion Constructor

        #region RESTful Conventions
        /// <summary>
        /// GET: api/items
        /// </summary>
        /// <returns>Nothing: this method will raise a HttpNotFound HTTP exception, since we're not supporting this API call.</returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return NotFound(new { Error = "not found" });
        }

        /// <summary>
        /// GET: api/items/{id}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>A Json-serialized object representing a single item.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = Context.Items.Where(i => i.Id == id).FirstOrDefault();
            if (item != null) return new JsonResult(Mapper.Map<ItemViewModel>(item), DefaultJsonSettings);
            return NotFound(new { Error = $"Item ID {id} has not been found" });
        }

        /// <summary>
        /// POST: api/items
        /// </summary>
        /// <returns>Create a new Item and return it accordingly.</returns>
        [HttpPost()]
        public IActionResult Add([FromBody]ItemViewModel ivm)
        {
            if (ivm != null)
            {
                // Create a new Item with the client-sent JSON data
                var item = Mapper.Map<Item>(ivm);
                // Override any property that could be wise to set from server-side only
                item.CreatedDate = item.LastModifiedDate = DateTime.Now;
                // TODO: replace the following with the current user's id when authentication will be available
                item.UserId = Context.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;
                // Add the new item
                Context.Items.Add(item);
                // Persist the change into the database
                Context.SaveChanges();
                // Return the newly-created Item to the client
                return new JsonResult(Mapper.Map<ItemViewModel>(item), DefaultJsonSettings);
            }
            // Return HTTP Status 500 if the payload is invalid
            return new StatusCodeResult(500);
        }

        /// <summary>
        /// PUT: api/items/{id}
        /// </summary>
        /// <returns>Update an existing Item and return it accordingly.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]ItemViewModel ivm)
        {
            if (ivm != null)
            {
                var item = Context.Items.Where(i => i.Id == id).FirstOrDefault();
                if (item != null)
                {
                    // Handle update
                    item.UserId = ivm.UserId;
                    item.Description = ivm.Description;
                    item.Flags = ivm.Flags;
                    item.Notes = ivm.Notes;
                    item.Text = ivm.Text;
                    item.Title = ivm.Title;
                    item.Type = ivm.Type;

                    // Override any property that could be wise to set from server-side only
                    item.LastModifiedDate = DateTime.Now;
                    // Persist the change into the database
                    Context.SaveChanges();
                    // Return the updated Item to the client
                    return new JsonResult(Mapper.Map<ItemViewModel>(item), DefaultJsonSettings);
                }
                // Return Not Found if we couldn't find the item
                return NotFound(new { Error = $"Item ID {id} has not been found" });
            }
            // Return HTTP Status 500 if the payload is invalid
            return new StatusCodeResult(500);
        }

        /// <summary>
        /// DELETE: api/items/{id}
        /// </summary>
        /// <returns>Delete an existing Item and return HTTP Status 200 when done.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = Context.Items.Where(i => i.Id == id).FirstOrDefault();
            if (item != null)
            {
                // Remove the item to delete from the DbContext
                Context.Items.Remove(item);
                // Persist the change into the database
                Context.SaveChanges();
                // Return HTTP Status 200
                return new OkResult();
            }
            // Return Not Found if we couldn't find the item
            return NotFound(new { Error = $"Item ID {id} has not been found" });
        }
        #endregion

        #region Attribute-based Routing
        /// <summary>
        /// GET: api/items/GetLatest
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing the last inserted items.</returns>
        [HttpGet("GetLatest")]
        public IActionResult GetLatest()
        {
            return GetLatest(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetLatest/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing the last inserted items.</returns>
        [HttpGet("GetLatest/{n}")]
        public IActionResult GetLatest(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = Context.Items.OrderByDescending(i => i.CreatedDate).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }

        /// <summary>
        /// GET: api/items/GetMostViewed
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing the items with most user views.</returns>
        [HttpGet("GetMostViewed")]
        public IActionResult GetMostViewed()
        {
            return GetMostViewed(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetMostViewed/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing the items with most user views.</returns>
        [HttpGet("GetMostViewed/{n}")]
        public IActionResult GetMostViewed(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = Context.Items.OrderByDescending(i => i.ViewCount).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }

        /// <summary>
        /// GET: api/items/GetRandom
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of a default number of Json-serialized objects representing some randomly-picked items.</returns>
        [HttpGet("GetRandom")]
        public IActionResult GetRandom()
        {
            return GetRandom(DefaultNumberOfItems);
        }

        /// <summary>
        /// GET: api/items/GetRandom/{n}
        /// ROUTING TYPE: attribute-based
        /// </summary>
        /// <returns>An array of {n} Json-serialized objects representing some randomly-picked items.</returns>
        [HttpGet("GetRandom/{n}")]
        public IActionResult GetRandom(int n)
        {
            if (n > MaxNumberOfItems) n = MaxNumberOfItems;
            var items = Context.Items.OrderBy(i => Guid.NewGuid()).Take(n).ToArray();
            return new JsonResult(ToItemViewModelList(items), DefaultJsonSettings);
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Map a collection of Item entities into a list of ItemViewModel objects.
        /// </summary>
        /// <param name="items">An IEnumerable collection of item entities</param>
        /// <returns>a mapped list of ItemViewModel objects</returns>
        private List<ItemViewModel> ToItemViewModelList(IEnumerable<Item> items)
        {
            var lst = new List<ItemViewModel>();
            foreach (var i in items) lst.Add(Mapper.Map<ItemViewModel>(i));
            return lst;
        }

        /// <summary>
        /// Returns a suitable JsonSerializerSettings object that can be used to generate the JsonResult return value for this Controller's methods.
        /// </summary>
        private JsonSerializerSettings DefaultJsonSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                };
            }
        }

        /// <summary>
        /// Returns the default number of items to retrieve when using the parameterless overloads of the API methods retrieving item lists.
        /// </summary>
        private int DefaultNumberOfItems
        {
            get
            {
                return 5;
            }
        }
        /// <summary>
        /// Returns the maximum number of items to retrieve when using the API methods retrieving item lists.
        /// </summary>
        private int MaxNumberOfItems
        {
            get
            {
                return 100;
            }
        }
        #endregion
    }
}
