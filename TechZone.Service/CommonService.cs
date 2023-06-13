using System.Collections.Generic;
using TechZone.Common;
using TechZone.Data.Infracstructure;
using TechZone.Data.Repositories;
using TechZone.Model.Models;

namespace TechZone.Service
{
    public interface ICommonService
    {
        Footer GetFooter();

        IEnumerable<Slide> GetSlides();

        SystemConfig GetSystemConfig(string code);
    }

    public class CommonService : ICommonService
    {
        private IFooterRepository _footerRepository;
        private IUnitOfWork _unitOfWork;
        private ISlideRepository _slideRepository;
        ISystemConfigRepository _systemConfigRepository;

        public CommonService(IFooterRepository footerRepository, IUnitOfWork unitOfWork, ISlideRepository slideRepository, ISystemConfigRepository systemConfigRepository)
        {
            _footerRepository = footerRepository;
            _unitOfWork = unitOfWork;
            _slideRepository = slideRepository;
            _systemConfigRepository = systemConfigRepository;
        }

        public Footer GetFooter()
        {
            return _footerRepository.GetSingleByCondition(x => x.ID == CommonConstants.DefaultFooterId);
        }

        public IEnumerable<Slide> GetSlides()
        {
            return _slideRepository.GetMulti(x => x.Status);
        }

        public SystemConfig GetSystemConfig(string code)
        {
            return _systemConfigRepository.GetSingleByCondition(x => x.Code == code);
        }
    }
}