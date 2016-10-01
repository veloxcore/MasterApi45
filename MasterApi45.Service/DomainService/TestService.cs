using MasterApi45.Data.Repository;
using MasterApi45.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Service.DomainService
{
    public interface ITestService
    {
        List<Test> GetAllTestData();
        Test GetByID(int ID);
        void Delete(int ID);
        void AddTestData(Test data);
        void UpdateTestData(Test data);
    }
    public class TestService : ITestService
    {
        #region Private Members
        ITestRepository _testRepository;
        #endregion

        #region Constructor
        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public void AddTestData(Test data)
        {
            try
            {
                _testRepository.Insert(data);
                _testRepository.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        #endregion

        #region Public Methods
        public void Delete(int ID)
        {
            _testRepository.Delete(ID);
        }

        public List<Test> GetAllTestData()
        {
            return _testRepository.GetAll().ToList();
        }

        public Test GetByID(int ID)
        {
            return _testRepository.GetByID(ID);
        }

        public void UpdateTestData(Test data)
        {
            try
            {
                _testRepository.Update(data);
                _testRepository.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
    #endregion
}
