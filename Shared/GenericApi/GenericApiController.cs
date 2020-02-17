using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Builder
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("api/[controller]")]
    public sealed class GenericApiController<TK, T> :
        ControllerBase
    {
        private readonly IStorage<T, TK> storage;
        private readonly IConfiguration configuration;

        public GenericApiController(
            IConfiguration configuration,
            IStorage<T, TK> storage)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TK>>> GetAll()
        {
            var data = await storage.Read();

            return new ActionResult<IEnumerable<TK>>(data.Any() ? Ok(data) : (ActionResult) NotFound());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TK>> Get(T id)
        {
            var (state, currentData) = await storage.ReadByKey(id);

            return new ActionResult<TK>(state ? Ok(currentData) : (ActionResult) NotFound());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] TK value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            var (state, _) = await storage.Create(value);

            return state ? Created(Request.GetEncodedUrl(), string.Empty) : (IActionResult) NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(T id, [FromBody] TK value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            var (state, _) = await storage.Update(id, value, configuration.ExcludeProperty(GetType(), typeof(TK)));

            return state ? Accepted() : (IActionResult) NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(T id)
        {
            var (state, _) = await storage.Delete(id);

            return state ? Accepted() : (ActionResult) NoContent();
        }
    }
}
