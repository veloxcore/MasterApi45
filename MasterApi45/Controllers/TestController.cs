using MasterApi45.Model.Entity;
using MasterApi45.Service.DomainService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MasterApi45.Controllers
{
    [Authorize]
    public class TestController : ApiController
    {
        #region Private Members
        private ITestService _testService;
        #endregion
        public TestController(ITestService testService)
        {
            _testService = testService;
        }
        // GET: api/Test
        public HttpResponseMessage Get()
        {
            List<Test> result = _testService.GetAllTestData();
            return Request.CreateResponse<List<Test>>(HttpStatusCode.OK, result);
        }

        // GET: api/Test/5
        public HttpResponseMessage Get(int id)
        {
            var result = _testService.GetByID(id);
            return Request.CreateResponse<Test>(HttpStatusCode.OK, result);
        }

        // POST: api/Test
        public HttpResponseMessage Post([FromBody]Test data)
        {
            Test dataExist = _testService.GetByID(data.ID);
            if (dataExist != null)
            {
                _testService.UpdateTestData(data);
            }
            else
            {
                _testService.AddTestData(data);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Test/5
        public HttpResponseMessage Delete(int id)
        {
            _testService.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
