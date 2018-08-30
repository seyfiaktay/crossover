using System.Collections.Generic;
using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class PanelControllerTests
    {
        private readonly PanelController _panelController;

        private readonly Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();

        private readonly AnalyticsController _analyticsController;

        private readonly Mock<IAnalyticsRepository> _analyticsRepositoryMock = new Mock<IAnalyticsRepository>();

        public PanelControllerTests()
        {
            _panelController = new PanelController(_panelRepositoryMock.Object);
            _analyticsController = new AnalyticsController(_analyticsRepositoryMock.Object, _panelRepositoryMock.Object);
        }

        [Fact]
        public async Task Check_PanelInsert()
        {
            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.765543,
                Serial = "AAAA1111BBBB2222"
            };
            var result = await _panelController.Register(panel);
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
        }


        [Fact]
        public void Check_DailyAnalysis()
        {
            var lcFakeList = GenerateFakeData();
            var lcList = GetFakeHistoricalData();

            var result = _analyticsController.GetHistoricalData(lcFakeList);
            // Assert
            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(result[i].Sum, lcList[i].Sum);
                Assert.Equal(result[i].Maximum, lcList[i].Maximum);
                Assert.Equal(result[i].Minimum, lcList[i].Minimum);
                Assert.Equal(result[i].Average, lcList[i].Average);
                Assert.Equal(result[i].DateTime, lcList[i].DateTime);
            }
        }
        private List<OneDayElectricityModel> GetFakeHistoricalData()
        {
            List<OneDayElectricityModel> listMust = new List<OneDayElectricityModel>();
            OneDayElectricityModel lcOneDayElectricityModel = new OneDayElectricityModel
            {
                DateTime = new System.DateTime(2018, 8, 31),
                Sum = 4000,
                Average = 2000,
                Minimum = 2000,
                Maximum = 2000
            };
            listMust.Add(lcOneDayElectricityModel);


            OneDayElectricityModel lcOneDayElectricityMod = new OneDayElectricityModel
            {
                DateTime = new System.DateTime(2017, 8, 31),
                Sum = 2000,
                Average = 1000,
                Minimum = 1000,
                Maximum = 1000
            };
            listMust.Add(lcOneDayElectricityMod);
            return listMust;

        }
        private List<OneHourElectricity> GenerateFakeData()
        {

            List<OneHourElectricity> list = new List<OneHourElectricity>();
            OneHourElectricity lcOneHourElectricity = new OneHourElectricity
            {
                Id = 1,
                KiloWatt = 2000,
                PanelId = 1,
                DateTime = new System.DateTime(2018, 8, 31)
            };
            list.Add(lcOneHourElectricity);

            lcOneHourElectricity = new OneHourElectricity
            {
                Id = 2,
                KiloWatt = 2000,
                PanelId = 1,
                DateTime = new System.DateTime(2018, 8, 31)
            };
            list.Add(lcOneHourElectricity);

            lcOneHourElectricity = new OneHourElectricity
            {
                Id = 3,
                KiloWatt = 1000,
                PanelId = 1,
                DateTime = new System.DateTime(2017, 8, 31)
            };
            list.Add(lcOneHourElectricity);

            lcOneHourElectricity = new OneHourElectricity
            {
                Id = 4,
                KiloWatt = 1000,
                PanelId = 1,
                DateTime = new System.DateTime(2017, 8, 31)
            };
            list.Add(lcOneHourElectricity);
            return list;

        }

        [Fact]
        public async Task Check_PanelSerial()
        {
            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.765543,
                Serial = "12345678901234567891234"
            };
            var result = await _panelController.Register(panel);
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.Null(createdResult);
        }


        [Fact]
        public async Task Check_PanelDecimalPlaces()
        {
            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.11111111111,
                Longitude = 98.11111111111,
                Serial = "AAAA1111BBBB2222"
            };
            var result = await _panelController.Register(panel);
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.Null(createdResult);
        }

        [Fact]
        public async Task Check_PanelLatitudeLongitudeRequried()
        {
            var panel = new PanelModel
            {
                Brand = "Areva",
                Serial = "AAAA1111BBBB2222"
            };
            var result = await _panelController.Register(panel);
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.Null(createdResult);
        }


        [Fact]
        public async Task Check_PanelLatitudeLongitudeMaxMinValue()
        {
            var panel = new PanelModel
            {
                Brand = "Areva",
                Serial = "AAAA1111BBBB2222",
                Latitude = 1800,
                Longitude = 1800
            };
            var result = await _panelController.Register(panel);
            Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.Null(createdResult);
        }


       

    }
}