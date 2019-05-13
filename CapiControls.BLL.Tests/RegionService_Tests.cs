using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.BLL.Services;
using CapiControls.Common.Mapping.Profiles;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Interfaces.Units;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CapiControls.BLL.Tests
{
    public class RegionService_Tests
    {
        IRegionService RegionService;

        public RegionService_Tests()
        {
            var regionRepository = new Mock<IRegionRepository>();
            regionRepository
                .Setup(r => r.GetAll())
                .Returns(() =>
                {
                    return new List<Region>()
                    {
                        new Region()
                        {
                            Id = Guid.NewGuid(),
                            Name = "RegionName_1",
                            Title = "RegionTitle_1"
                        },
                        new Region()
                        {
                            Id = Guid.NewGuid(),
                            Name = "RegionName_2",
                            Title = "RegionName_2"
                        }
                    };
                });

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BLLProfile());
            }).CreateMapper();

            var mockUnitOfWork = new Mock<ILocalUnitOfWork>();
            mockUnitOfWork
                .Setup(uow => uow.RegionRepository)
                .Returns(regionRepository.Object);

            RegionService = new RegionService(mockUnitOfWork.Object, mockMapper);
        }

        [Fact]
        public void GetRegions_ReturnsRegionDTOList()
        {
            // Act
            var result = RegionService.GetRegions();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<RegionDTO>>(result);
            Assert.True(result.Count() > 0);
        }
    }
}
