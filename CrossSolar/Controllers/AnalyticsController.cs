using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    [Route("panel")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [HttpGet("{banelId}/[controller]")]
        public async Task<IActionResult> Get([FromRoute] string panelId)
        {
            var panelIdint = Convert.ToInt32(panelId);
            var panel = await _panelRepository.Query().FirstOrDefaultAsync(x => x.Id == panelIdint);
            //var panel = await _panelRepository.GetAsync(panelId);
            if (panel == null) return NotFound();

            var analytics = await _analyticsRepository.Query()
                .Where(x => x.PanelId == panelIdint).ToListAsync();

            var result = new OneHourElectricityListModel
            {
                OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                {
                    Id = c.Id,
                    KiloWatt = c.KiloWatt,
                    DateTime = c.DateTime
                })
            };

            return Ok(result);
        }


        // GET panel/XXXX1111YYYY2222/analytics/day
        [HttpGet("{panelId}/[controller]/day")]
        public async Task<IActionResult> DayResults([FromRoute] string panelId)
        {
            var models = await fnGetOneHourElectricityList(panelId);
            var result = GetHistoricalData(models);
            return Ok(result);
        }

        private async Task<List<OneHourElectricity>> fnGetOneHourElectricityList(string argpanelId)
        {
            var lcpanelid = Convert.ToInt32(argpanelId);
            var model = await _analyticsRepository.Query().Where(x => x.PanelId == lcpanelid).ToListAsync();
            return model;
        }
        public List<OneDayElectricityModel> GetHistoricalData(List<OneHourElectricity> argmodels)
        {
            var lcmodel = argmodels.GroupBy(x => x.DateTime.ToShortDateString()).Select(value => new OneDayElectricityModel
            {
                Sum = value.Sum(pv => pv.KiloWatt),
                Average = value.Average(pv => pv.KiloWatt),
                Maximum = value.Max(pv => pv.KiloWatt),
                Minimum = value.Min(pv => pv.KiloWatt),
                DateTime = value.Last().DateTime
            }).OrderByDescending(value => value.DateTime).ToList();
            return lcmodel;
        }
       

        // POST panel/XXXX1111YYYY2222/analytics
        [HttpPost("{panelId}/[controller]")]
        public async Task<IActionResult> Post([FromRoute] string panelId, [FromBody] OneHourElectricityModel value)
        {
            var argcontext = new ValidationContext(value, null, null);
            var lcresult = new List<ValidationResult>();
            var lcvalid = Validator.TryValidateObject(value, argcontext, lcresult, true);
            if (!lcvalid) return BadRequest(ModelState);
            var lcpanelid = Convert.ToInt32(panelId);
            var oneHourElectricityContent = new OneHourElectricity
            {
                PanelId = lcpanelid,
                KiloWatt = value.KiloWatt,
                DateTime = DateTime.UtcNow
            };

            await _analyticsRepository.InsertAsync(oneHourElectricityContent);

            var result = new OneHourElectricityModel
            {
                Id = oneHourElectricityContent.Id,
                KiloWatt = oneHourElectricityContent.KiloWatt,
                DateTime = oneHourElectricityContent.DateTime
            };

            return Created($"panel/{panelId}/analytics/{result.Id}", result);
        }
    }
}