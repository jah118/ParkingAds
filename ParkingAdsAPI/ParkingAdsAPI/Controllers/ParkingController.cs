using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParkingAdsAPI.Data;
using ParkingAdsAPI.DTO;
using ParkingAdsAPI.RabbitMQs;
using ParkingAdsAPI.Util;

namespace ParkingAdsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {

        private readonly IMessageProducer _messagePublisher;

        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;




        public ParkingController(IMessageProducer messagePublisher, IOptionsMonitor<AppSettings> optionsMonitor) 
        {
            _messagePublisher = messagePublisher;
            _appSettings = optionsMonitor.CurrentValue;
            //_config = config;

        }


        [HttpGet("{place}", Name = "GetParking")]
        public async Task<IActionResult> GetParking(string place)
        {

            //TODO build MSG that follow Schema 

            _messagePublisher.SendMessage(place);

            //TODO: call cache 
            //TODO: Await svar  og build return model 

            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> BookParking([FromBody] ParkingDTO modelDTO)
        {
            ParkingModel model = new()
            {
                Data = modelDTO.Data,
            };



            _messagePublisher.SendMessage(model.Data);


            //TODO: call cache 
            //TODO: Await svar  og build return model 

            return Ok(model);
        }


    }
}
