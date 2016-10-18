using BetManager.Core.DbModels;
using BetManager.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BetManager.Web.ApiControllers
{
    public class ImportDataController : ApiController
    {
        private readonly IImportDataManager _importDataManager;
        public ImportDataController()
        {
            _importDataManager = new ImportDataManager();
        }
        // GET api/<controller>
        public IEnumerable<ImportData> Get()
        {
            return _importDataManager.GetAll();
        }

        // GET api/<controller>/5
        public ImportData Get(int id)
        {
            return _importDataManager.GetById(id);
        }

        // POST api/<controller>
        public HttpResponseMessage Post(ImportData value)
        {           
            if (value == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var ret = _importDataManager.Insert(value);
            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]ImportData value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}